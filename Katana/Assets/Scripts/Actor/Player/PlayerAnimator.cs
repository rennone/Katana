using UnityEngine;
using System.Collections;

namespace Katana
{
    [RequireComponent(typeof (ActorMotor))]
    public class PlayerAnimator : MonoBehaviour
    {

        [SerializeField] private Animator animator_ = null;

        private ActorMotor motor_ = null;

        private void Awake()
        {
            motor_ = GetComponent<ActorMotor>();
        }

        // Update is called once per frame
        private void Update()
        {
            animator_.SetBool("IsJump", motor_.IsJumping()); //ジャンプフラグのセット
            animator_.SetFloat("MoveSpeed",
                motor_.movement.velocity.magnitude + motor_.InputMoveDirection.magnitude*motor_.movement.MaxForwardSpeed);
        }
    }
}