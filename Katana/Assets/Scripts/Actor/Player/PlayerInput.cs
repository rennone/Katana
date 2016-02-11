using UnityEngine;
using System.Collections;
using System;

namespace Katana
{
    public class PlayerInput
    {
        private ActorMotor _motor;
        private PlayerAnimator _animator;

        public PlayerInput(ActorMotor motor, PlayerAnimator animator)
        {
            _motor = motor;
            _animator = animator;
        }

        // Update is called once per frame
        public void Update()
        {
            _motor.InputMoveDirection = (Input.GetAxisRaw("Horizontal")*Time.deltaTime)*Vector3.right;
            _motor.InputJump = Input.GetButtonDown("Jump");
        }
    }
}