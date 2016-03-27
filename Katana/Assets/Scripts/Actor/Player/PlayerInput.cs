using UnityEngine;
using System.Collections;
using System;

namespace Katana
{
    public class PlayerInput : AMonoBehaviour
    {
        private Player _player;
        private PlayerMotor _motor;

        protected override void OnInitialize()
        {
            _player = GetComponent<Player>();
            _motor = GetComponent<PlayerMotor>();
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            // 移動
            if (_player.IsReleased(Player.Action.Move))
            {
                _motor.InputMoveDirection = _player.CanInpuMove()
                    ? (Input.GetAxisRaw("Horizontal")*Time.deltaTime)*Vector3.right
                    : Vector3.zero;
            }

            // ジャンプ
            if (_player.IsReleased(Player.Action.Jump))
            {
                _motor.InputJump = _player.CanInputJump() && Input.GetButtonDown("Jump");
               // _player._animatorAccess.SetIsJump(_player.CanInputJump() && Input.GetButtonDown("Jump"));
            }

            // 攻撃1
            if (_player.IsReleased(Player.Action.Attack))
            {
                _player.AnimatorAccess.SetIsAttack(Input.GetButtonDown("Fire1") && _player.CanInputAttack());
            }

            // 攻撃2
            if (_player.IsReleased(Player.Action.JumpAttack))
            {
                _player.AnimatorAccess.SetIsJumpAttack(Input.GetButtonDown("Fire2") && _player.CanInputJumpAttack());
            }

            // しゃがむ
            if (_player.IsReleased(Player.Action.Squat))
            {
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    _player.AnimatorAccess.SetIsSquad(Input.GetButtonDown("Vertical"));
                }
            }
        }

    }
}