using UnityEngine;
using System.Collections;

namespace Katana
{
    public class PlayerAnimator : AnimatorAccess.PlayerAnimator
    {
        private PlayerMotor _motor = null;

        public void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
        }

        // Update is called once per frame
        public void Update()
        {
            SetIsJump(_motor.IsJumping());//ジャンプフラグのセット
            SetMoveSpeed(_motor.movement.velocity.magnitude + _motor.InputMoveDirection.magnitude*_motor.movement.MaxForwardSpeed);
        }

        public void AnimationCallbackWeaponActive()
        {
            GetComponentInChildren<SimpleWeapon>().SetActive(true);
        }

        public void AnimationCallbackWaponInActive()
        {
            GetComponentInChildren<SimpleWeapon>().SetActive(false);
        }

    }
}