using UnityEngine;
using System.Collections;
using System;

namespace Katana
{
    public class PlayerInput : AMonoBehaviour
    {
        private Player _player;
        private PlayerMotor _motor;
        private float inputVertical_Buffer;

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
                    ? (Input.GetAxisRaw("Horizontal") * Time.deltaTime) * Vector3.right
                    : Vector3.zero;
            }

            // ジャンプ
            if (_player.IsReleased(Player.Action.Jump))
            {
                _motor.InputJump = _player.CanInputJump() && Input.GetButtonDown("Jump");
                // _player._animatorAccess.SetIsJump(_player.CanInputJump() && Input.GetButtonDown("Jump"));
            }

            // 攻撃1
            if (_player.IsReleased(Player.Action.Fire1))
            {
                _player.AnimatorAccess.SetIsAttack(Input.GetButtonDown("Fire1") && _player.CanInputAttack());
            }

            // 攻撃2
            if (_player.IsReleased(Player.Action.Fire2))
            {
                _player.AnimatorAccess.SetIsJumpAttack(Input.GetButtonDown("Fire2") && _player.CanInputJumpAttack());
            }

            //上下キーでのアクション
            {
                float inputVertical = Input.GetAxisRaw("Vertical");

                //連続押し判定の回避
                if (inputVertical != inputVertical_Buffer)
                {
                    //上キーでのアクション
                    if (inputVertical > 0.5f && inputVertical != inputVertical_Buffer)
                    {
                        DoorCheck();
                    }
                }
                inputVertical_Buffer = inputVertical;
            }
        }

        void DoorCheck()
        {
            RaycastHit hit;
            int layerMask = 1 << LayerMask.NameToLayer("Gimmick");
            if (Physics.Raycast(_player.CenterPosition, Vector3.forward, out hit, 5f, layerMask))
            {
                var door = hit.collider.GetComponent<Door>();
                if(door != null)
                {
                    door.Open();
                }
            }
        }


        
    }
}