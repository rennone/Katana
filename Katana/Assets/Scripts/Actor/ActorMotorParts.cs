using UnityEngine;
using System.Collections;

namespace Katana
{
// CharacterMotorの内部クラスを別に定義する
    public partial class ActorMotor
    {

        [System.Serializable]
        public class Constraint
        {
            public bool X, Y, Z;
        }

        [System.Serializable]
        public class CharacterMotorMovement
        {
            public float MaxForwardSpeed = 10.0f;
            public float MaxSidewaysSpeed = 10.0f;
            public float MaxBackwardsSpeed = 10.0f;

            // 斜面に基づいて速度を乗算する曲線（負=下向き）
            public AnimationCurve SlopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1),
                new Keyframe(90, 0));

            public float MaxGroundAcceleration = 30.0f;
            public float MaxAirAcceleration = 20.0f;

            public float Gravity = 30.0f;
            public float MaxFallSpeed = 500.0f;

            // 動きの制限
            public Constraint FreezePosition;

            public Vector3 ConstraintPosition = new Vector3();

            // 衝突判定フラグ
            [System.NonSerialized] public CollisionFlags collisionFlags;

            // We will keep track of the character's current velocity,
            [System.NonSerialized] public Vector3 velocity;

            // This keeps track of our current velocity while we're not grounded
            [System.NonSerialized] public Vector3 frameVelocity = Vector3.zero;

            [System.NonSerialized] public Vector3 hitPoint = Vector3.zero;

            [System.NonSerialized] public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
        }

        //ジャンプ時の挙動（選択式）
        public enum MovementTransferOnJump
        {
            None, // ジャンプは床の速度に影響されない
            InitTransfer, // ジャンプは床からの初期速度を取得し、その後徐々に停止する        
            PermaTransfer, // ジャンプは床からの初期速度を取得し、着陸するまでその速度を維持        
            PermaLocked // ジャンプは最後に触れた床の動きを基準にして、その床と一緒に移動する
        }

        // We will contain all the jumping related variables in one helper class for clarity.
        [System.Serializable]
        public class CharacterMotorJumping
        {
            public bool enabled = true;
            public float baseHeight = 12.0f;
            public float extraHeight = 2.0f;

            // How much does the character jump out perpendicular to the surface on walkable surfaces?
            // 0 means a fully vertical jump and 1 means fully perpendicular.
            public float perpAmount = 0.0f; //地面に対して垂直

            // How much does the character jump out perpendicular to the surface on too steep surfaces?
            // 0 means a fully vertical jump and 1 means fully perpendicular.
            public float steepPerpAmount = 0.5f; //よく分からんが0が垂直跳びで1が垂直らしい


            [System.NonSerialized] public bool isJumping = false;

            [System.NonSerialized] public bool holdingJumpButton = false;

            // the time we jumped at(Used to determine for how long to apply extra jump power after jumping.)
            [System.NonSerialized] public float lastStartTime = 0.0f;

            [System.NonSerialized] public float lastButtonDownTime = -100.0f;

            [System.NonSerialized] public Vector3 jumpDir = Vector3.up;
        }

        [System.Serializable]
        public class CharacterMotorMovingPlatform
        {
            public bool enabled = true;

            public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;

            [System.NonSerialized] public Transform hitPlatform;

            [System.NonSerialized] public Transform activePlatform;

            [System.NonSerialized] public Vector3 activeLocalPoint;

            [System.NonSerialized] public Vector3 activeGlobalPoint;

            [System.NonSerialized] public Quaternion activeLocalRotation;

            [System.NonSerialized] public Quaternion activeGlobalRotation;

            [System.NonSerialized] public Matrix4x4 lastMatrix;

            [System.NonSerialized] public Vector3 platformVelocity;

            [System.NonSerialized] public bool hasNewPlatform;
        }

        [System.Serializable]
        public class CharacterMotorSliding
        {
            // Does the character slide on too steep surfaces?
            public bool enabled = true;

            // How fast does the character slide on steep surfaces?
            public float slidingSpeed = 15.0f;

            // How much can the player control the sliding direction?
            // ifthe value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
            public float sidewaysControl = 1.0f;

            // How much can the player influence the sliding speed?
            // ifthe value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
            public float speedControl = 0.4f;
        }
    }
}