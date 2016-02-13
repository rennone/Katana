using UnityEngine;
using System.Collections;
using System;

namespace Katana
{
    public class PlayerInput
    {
        private readonly Player _player;

        public PlayerInput(Player player)
        {
            _player = player;
        }

        // Update is called once per frame
        public void Update()
        {
            _player.Motor.InputMoveDirection = _player.CanMove() ? (Input.GetAxisRaw("Horizontal") * Time.deltaTime) * Vector3.right : Vector3.zero;

            _player.Motor.InputJump = Input.GetButtonDown("Jump");
            _player.Animator.SetIsAttack(Input.GetButtonDown("Fire1"));
        }
    }
}