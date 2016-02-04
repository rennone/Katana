using System;
using UnityEngine;
using System.Collections;

namespace Katana
{

// Require a character controller to be attached to the same game object
    [RequireComponent(typeof (CharacterController))] //Component->phisicsにある.地面とのあたり判定なんかを勝手にやってくれる
    [AddComponentMenu("Character/Character Motor")]

    public partial class ActorMotor : MonoBehaviour
    {
        private const float Epsilon = 1.0e-6f;
        // このスクリプトを入力に応答させるかどうかのフラグ
        private bool canControl = true;
        private bool useFixedUpdate = false; //fixedUpdateでinput入力を受け取るのはよくないので切る

        // For the next variables, [System.NonSerialized] tells Unity to not serialize the variable or show it in the inspector view.
        // Very handy for organization!

        // The current global direction we want the character to move in.
        public Vector3 InputMoveDirection { get; set; }

        // Is the jump button held down? We use this interface instead of checking
        // for the jump button directly so this script can also be used by AIs.
        public bool InputJump { get; set; }

        public CharacterMotorMovement movement = new CharacterMotorMovement();
        public CharacterMotorJumping jumping = new CharacterMotorJumping();

        public CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();
        public CharacterMotorSliding sliding = new CharacterMotorSliding();

        private bool _grounded = true;

        [System.NonSerialized] public Vector3 groundNormal = Vector3.zero;
        private Vector3 lastGroundNormal = Vector3.zero;

        //  private Transform tr;
        private CharacterController controller;




        private CollisionFlags Move(Vector3 move)
        {
            // それぞれの軸のConstraint設定
            // http://forum.unity3d.com/threads/restrict-z-movement-on-character-controller-without-breaking-into-colliders.91358/
            float coef = 0.05f;
            if (movement.FreezePosition.X && Mathf.Abs(movement.ConstraintPosition.x - transform.position.x) > 0)
            {
                move.x = (movement.ConstraintPosition.x - transform.position.x)*coef;
            }

            if (movement.FreezePosition.Y && Mathf.Abs(movement.ConstraintPosition.y - transform.position.y) > 0)
            {
                move.y = (movement.ConstraintPosition.y - transform.position.y)*coef;
            }

            if (movement.FreezePosition.Z && Mathf.Abs(movement.ConstraintPosition.z - transform.position.z) > 0)
            {
                move.z = (movement.ConstraintPosition.z - transform.position.z)*coef;
            }

            var collisionFlag = controller.Move(move);

            return collisionFlag;
        }

        private void lockZAxis(CollisionFlags collisionFlags, Vector3 oldPosition, ref Vector3 position)
        {
            if (Math.Abs(position.z - oldPosition.z) > 0)
            {
                // is there any y-axis displacement during splice ? if yes assume the controlled object can slice on y-axis
                if (Mathf.Abs(oldPosition.y - position.y) > 0)
                {
                    // slice up if collision below
                    if ((collisionFlags & CollisionFlags.Below) != 0)
                    {
                        position.y += Mathf.Abs(position.z - oldPosition.z);

                        // slice down if collision above
                    }
                    else if ((collisionFlags & CollisionFlags.Above) != 0)
                    {
                        position.y -= Mathf.Abs(position.z - oldPosition.z);
                    }
                    // no y-axis slice possible : stuck
                }
                else
                {
                    position.x = oldPosition.x;
                }


                position.z = oldPosition.z;
            }
        }

        //最初に絶対呼び出される関数
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        //----------------------------------------------------------------------------

        // Moving platform support
        //Platform(踏み台,地面)による移動分
        private void MoveByPlatform()
        {
            Vector3 moveDistance = Vector3.zero;
            if (MoveWithPlatform())
            {
                Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
                moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
                Move(moveDistance);

                // Support moving platform rotation as well:
                Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation*movingPlatform.activeLocalRotation;
                Quaternion rotationDiff = newGlobalRotation*Quaternion.Inverse(movingPlatform.activeGlobalRotation);

                var yRotation = rotationDiff.eulerAngles.y;
                if (Mathf.Abs(yRotation) > Epsilon)
                {
                    // Prevent rotation of the local up vector
                    transform.Rotate(0, yRotation, 0);
                }
            }
        }

        private void UpdateDirection()
        {
            if (Mathf.Abs(InputMoveDirection.x) >= 1.0e-6)
            {
                var direction = InputMoveDirection.x < 0 ? Vector3.left : Vector3.right;
                var afterDirection = Vector3.Lerp(transform.forward, direction, 0.5f);
                transform.forward = afterDirection == Vector3.zero ? direction : afterDirection;
            }
        }

        private void UpdateFunction()
        {
            // 方向の更新
            UpdateDirection();

            //速度の更新
            Vector3 velocity = movement.velocity;
            velocity = ApplyInputVelocityChange(velocity);
            velocity = ApplyGravityAndJumping(velocity);

            // 地面による移動
            MoveByPlatform();

            // Save lastPosition for velocity calculation.
            Vector3 lastPosition = transform.position;

            Vector3 currentMovementOffset = velocity*Time.deltaTime;

            // ステップを歩いたりスロープの急激な変化を介したとき、地面の喪失を避けるために地面からどれくらい上げる必要があるか調べる
            float pushDownOffset = Mathf.Max(controller.stepOffset,
                new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
            if (_grounded)
                currentMovementOffset -= pushDownOffset*Vector3.up;

            // Reset variables that will be set by collision function
            movingPlatform.hitPlatform = null;
            groundNormal = Vector3.zero;

            // 実際にキャラクターを動かす
            movement.collisionFlags = Move(currentMovementOffset);

            movement.lastHitPoint = movement.hitPoint;
            lastGroundNormal = groundNormal;

            if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform)
            {
                if (movingPlatform.hitPlatform != null)
                {
                    movingPlatform.activePlatform = movingPlatform.hitPlatform;
                    movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
                    movingPlatform.hasNewPlatform = true;
                }
            }

            // Calculate the velocity based on the current and previous position.  
            // This means our velocity will only be the amount the character actually moved as a result of collisions.
            Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
            movement.velocity = (transform.position - lastPosition)/Time.deltaTime;
            Vector3 newHVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);

            // The CharacterController can be moved in unwanted directions when colliding with things.
            // We want to prevent this from influencing the recorded velocity.
            if (oldHVelocity == Vector3.zero)
            {
                movement.velocity = new Vector3(0, movement.velocity.y, 0);
            }
            else
            {
                float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity)/oldHVelocity.sqrMagnitude;
                movement.velocity = oldHVelocity*Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y*Vector3.up;
            }

            if (movement.velocity.y < velocity.y - 0.001)
            {
                if (movement.velocity.y < 0)
                {
                    // Something is forcing the CharacterController down faster than it should.
                    // Ignore this
                    movement.velocity.y = velocity.y;
                }
                else
                {
                    // The upwards movement of the CharacterController has been blocked.
                    // This is treated like a ceiling collision - stop further jumping here.
                    jumping.holdingJumpButton = false;
                }
            }

            GroundedUpdate(pushDownOffset);
        }

        private void GroundedUpdate(float pushDownOffset)
        {
            // We were grounded but just loosed grounding
            if (_grounded && !IsGroundedTest())
            {
                _grounded = false;

                // Apply inertia from platform
                if (movingPlatform.enabled &&
                    (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
                     movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
                    )
                {
                    movement.frameVelocity = movingPlatform.platformVelocity;
                    movement.velocity += movingPlatform.platformVelocity;
                }

                SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
                // We pushed the character down to ensure it would stay on the ground ifthere was any.
                // But there wasn't so now we cancel the downwards offset to make the fall smoother.
                transform.position += pushDownOffset*Vector3.up;
            }
            // We were not grounded but just landed on something
            else if (!_grounded && IsGroundedTest())
            {
                _grounded = true;
                jumping.isJumping = false;
                SubtractNewPlatformVelocity();

                Debug.Log("OnLand in Charactor Motor");
                SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
            }

            // Moving platforms support
            if (MoveWithPlatform())
            {
                // Use the center of the lower half sphere of the capsule as reference point.
                // This works best when the character is standing on moving tilting platforms. 
                movingPlatform.activeGlobalPoint = transform.position +
                                                   Vector3.up*
                                                   (controller.center.y - controller.height*0.5f + controller.radius);
                movingPlatform.activeLocalPoint =
                    movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);

                // Support moving platform rotation as well:
                movingPlatform.activeGlobalRotation = transform.rotation;
                movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation)*
                                                     movingPlatform.activeGlobalRotation;
            }
        }

        //----------------------------------------------------------------------------

        private void FixedUpdate()
        {
            if (movingPlatform.enabled)
            {
                if (movingPlatform.activePlatform != null)
                {
                    if (!movingPlatform.hasNewPlatform)
                    {
                        // unused: Vector3 lastVelocity = movingPlatform.platformVelocity;
                        Vector3 distance = movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(
                            movingPlatform.activeLocalPoint)
                                           - movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint);
                        movingPlatform.platformVelocity = distance/Time.deltaTime;
                    }
                    movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
                    movingPlatform.hasNewPlatform = false;
                }
                else
                {
                    movingPlatform.platformVelocity = Vector3.zero;
                }
            }

            if (useFixedUpdate)
                UpdateFunction();
        }

        //----------------------------------------------------------------------------
        private void Update()
        {
            if (!useFixedUpdate)
                UpdateFunction();
        }

        //----------------------------------------------------------------------------
        //基本的な移動を司る関数（ジャンプは含まれない）
        private Vector3 ApplyInputVelocityChange(Vector3 velocity)
        {
            if (!canControl)
                InputMoveDirection = Vector3.zero;

            // Find desired velocity
            Vector3 desiredVelocity;

            //地面に立っていて,かつその傾斜が大きいとき
            if (_grounded && TooSteep())
            {
                // The direction we're sliding in
                desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
                // Find the input movement direction projected onto the sliding direction
                var projectedMoveDir = Vector3.Project(InputMoveDirection, desiredVelocity);
                // Add the sliding direction, the spped control, and the sideways control vectors
                desiredVelocity = desiredVelocity + projectedMoveDir*sliding.speedControl +
                                  (InputMoveDirection - projectedMoveDir)*sliding.sidewaysControl;
                // Multiply with the sliding speed
                desiredVelocity *= sliding.slidingSpeed;
            }
            else
            {
                desiredVelocity = GetDesiredHorizontalVelocity();
            }

            if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
            {
                desiredVelocity += movement.frameVelocity;
                desiredVelocity.y = 0;
            }

            if (_grounded)
                desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
            else
                velocity.y = 0;

            // Enforce max velocity change
            float maxVelocityChange = GetMaxAcceleration(_grounded)*Time.deltaTime;
            Vector3 velocityChangeVector = (desiredVelocity - velocity);
            if (velocityChangeVector.sqrMagnitude > maxVelocityChange*maxVelocityChange)
            {
                velocityChangeVector = velocityChangeVector.normalized*maxVelocityChange;
            }
            // ifwe're in the air and don't have control, don't apply any velocity change at all.
            // ifwe're on the ground and don't have control we do apply it - it will correspond to friction.
            if (_grounded || canControl)
                velocity += velocityChangeVector;

            if (_grounded)
            {
                // When going uphill, the CharacterController will automatically move up by the needed amount.
                // Not moving it upwards manually prevent risk of lifting off from the ground.
                // When going downhill, DO move down manually, as gravity is not enough on steep hills.
                velocity.y = Mathf.Min(velocity.y, 0);
            }

            return velocity;
        }

        //----------------------------------------------------------------------------
        //ジャンプを司る関数
        private Vector3 ApplyGravityAndJumping(Vector3 velocity)
        {

            if (!InputJump || !canControl)
            {
                jumping.holdingJumpButton = false;
                jumping.lastButtonDownTime = -100;
            }

            if (InputJump && _grounded && jumping.lastButtonDownTime < 0 && canControl)
            {
                SoundManager.I.PlaySound(this.transform, SoundKey.SE_JUMP01);
                jumping.lastButtonDownTime = Time.time;
            }

            if (InputJump)
            {
                Debug.Log("Jump on " + (_grounded ? "ground" : "not ground"));
            }

            if (_grounded)
            {
                velocity.y = Mathf.Min(0, velocity.y) - movement.Gravity*Time.deltaTime;
                // ジャンプキーを押しっぱなしで着地した時にジャンプするのを防ぐために最後の条件がある
                if (jumping.enabled && canControl && (Time.time - jumping.lastButtonDownTime < 0.2))
                {
                    _grounded = false;
                    jumping.isJumping = true;
                    jumping.lastStartTime = Time.time;
                    jumping.lastButtonDownTime = -100;
                    jumping.holdingJumpButton = true;

                    // Calculate the jumping direction
                    if (TooSteep())
                        jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
                    else
                        jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);

                    // 最初の任意の垂直速度を0にし、速度に跳躍力を加える
                    velocity.y = 0;
                    velocity += jumping.jumpDir*CalculateJumpVerticalSpeed(jumping.baseHeight);

                    // プラットフォームから慣性を適用する
                    if (movingPlatform.enabled &&
                        (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
                         movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
                        )
                    {
                        movement.frameVelocity = movingPlatform.platformVelocity;
                        velocity += movingPlatform.platformVelocity;
                    }

                    SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    jumping.holdingJumpButton = false;
                }
            }
            else
            {
                velocity.y = movement.velocity.y - movement.Gravity*Time.deltaTime;

                // ジャンプボタンを長押しした際の、ジャンプの高さの詳細な制御
                if (jumping.isJumping && jumping.holdingJumpButton)
                {
                    // 余分なジャンプが力を持つ時間を計算する
                    // 現在の経過時間がジャンプ可能上限時間以下なら、力を加える
                    if (Time.time <
                        jumping.lastStartTime + jumping.extraHeight/CalculateJumpVerticalSpeed(jumping.baseHeight))
                    {
                        // 重力を除いてy方向に力を加える
                        velocity += jumping.jumpDir*movement.Gravity*Time.deltaTime;
                    }
                }

                // 落下速度の最大値を超えないようMAX選択
                velocity.y = Mathf.Max(velocity.y, -movement.MaxFallSpeed);
            }

            return velocity;
        }

        //----------------------------------------------------------------------------

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0)
            {
                if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001 || lastGroundNormal == Vector3.zero)
                    groundNormal = hit.normal;
                else
                    groundNormal = lastGroundNormal;

                movingPlatform.hitPlatform = hit.collider.transform;
                movement.hitPoint = hit.point;
                movement.frameVelocity = Vector3.zero;
            }
        }

        //----------------------------------------------------------------------------

        private IEnumerator SubtractNewPlatformVelocity()
        {
            // When landing, subtract the velocity of the new ground from the character's velocity
            // since movement in ground is relative to the movement of the ground.
            if (movingPlatform.enabled &&
                (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
                 movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
            {
                // if we landed on a new platform, we have to wait for two FixedUpdates
                // before we know the velocity of the platform under the character
                if (movingPlatform.hasNewPlatform)
                {
                    Transform platform = movingPlatform.activePlatform;
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                    if (_grounded && platform == movingPlatform.activePlatform)
                        yield break;
                }
                movement.velocity -= movingPlatform.platformVelocity;
            }
        }

        //----------------------------------------------------------------------------

        private bool MoveWithPlatform()
        {
            return (movingPlatform.enabled
                    && (_grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
                    && movingPlatform.activePlatform != null
                );
        }

        //----------------------------------------------------------------------------

        private Vector3 GetDesiredHorizontalVelocity()
        {
            // Find desired velocity
            Vector3 desiredLocalDirection = transform.InverseTransformDirection(InputMoveDirection);
            float maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
            if (_grounded)
            {
                // Modify max speed on slopes based on slope speed multiplier curve
                var movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y)*Mathf.Rad2Deg;
                maxSpeed *= movement.SlopeSpeedMultiplier.Evaluate(movementSlopeAngle);
            }
            return transform.TransformDirection(desiredLocalDirection*maxSpeed);
        }

        //----------------------------------------------------------------------------

        private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
        {
            Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
            return Vector3.Cross(sideways, groundNormal).normalized*hVelocity.magnitude;
        }

        //----------------------------------------------------------------------------

        private bool IsGroundedTest()
        {
            return (groundNormal.y > 0.01);
        }

        //----------------------------------------------------------------------------

        private float GetMaxAcceleration(bool grounded)
        {
            // Maximum acceleration on ground and in air
            if (grounded)
                return movement.MaxGroundAcceleration;
            else
                return movement.MaxAirAcceleration;
        }

        //----------------------------------------------------------------------------

        private float CalculateJumpVerticalSpeed(float targetJumpHeight)
        {
            // From the jump height and gravity we deduce the upwards speed 
            // for the character to reach at the apex.
            return Mathf.Sqrt(2*targetJumpHeight*movement.Gravity);
        }

        //----------------------------------------------------------------------------

        public bool IsJumping()
        {
            return jumping.isJumping;
        }

        //----------------------------------------------------------------------------

        private bool IsSliding()
        {
            return (_grounded && sliding.enabled && TooSteep());
        }

        //----------------------------------------------------------------------------

        private bool IsTouchingCeiling()
        {
            return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
        }

        //----------------------------------------------------------------------------

        public bool IsGrounded()
        {
            return _grounded;
        }

        //----------------------------------------------------------------------------

        //傾斜が急かどうか
        private bool TooSteep()
        {
            return (groundNormal.y <= Mathf.Cos(controller.slopeLimit*Mathf.Deg2Rad));
        }

        //----------------------------------------------------------------------------

        private Vector3 GetDirection()
        {
            return InputMoveDirection;
        }

        //----------------------------------------------------------------------------

        private void SetControllable(bool controllable)
        {
            canControl = controllable;
        }

        //----------------------------------------------------------------------------

        // Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
        // The function returns the length of the resulting vector.
        private float MaxSpeedInDirection(Vector3 desiredMovementDirection)
        {
            if (desiredMovementDirection == Vector3.zero)
                return 0;
            else
            {
                float zAsixMaxSpeed = desiredMovementDirection.z > 0
                    ? movement.MaxForwardSpeed
                    : movement.MaxBackwardsSpeed;
                //  float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
                //Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
                //  float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
                Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z).normalized;
                float length = new Vector3(temp.x*movement.MaxSidewaysSpeed, 0, temp.z*zAsixMaxSpeed).magnitude;
                return length;
            }
        }

        //----------------------------------------------------------------------------

        //void SetVelocity(Vector3 velocity)
        //{
        //    grounded = false;
        //    movement.velocity = velocity;
        //    movement.frameVelocity = Vector3.zero;
        //    SendMessage("OnExternalVelocity");
        //}

        public bool Grounded()
        {
            return _grounded;
        }
    }
}