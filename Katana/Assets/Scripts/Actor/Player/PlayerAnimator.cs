using UnityEngine;
using System.Collections;

namespace Katana
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] 
        private Animator _animator = null;

        private PlayerMotor _motor = null;

        public void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
        }

        public bool SetIsAttack(bool enable)
        {
            _animator.SetBool("IsAttack", enable);
            return true;
        }

        public bool IsAttack()
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName("Kick");
        }
        // Update is called once per frame
        public void Update()
        {
            _animator.SetBool("IsJump", _motor.IsJumping()); //ジャンプフラグのセット
            _animator.SetFloat("MoveSpeed", _motor.movement.velocity.magnitude + _motor.InputMoveDirection.magnitude*_motor.movement.MaxForwardSpeed);
           
        }
    }
}