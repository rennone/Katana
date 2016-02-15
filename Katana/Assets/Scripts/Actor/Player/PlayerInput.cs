using UnityEngine;
using System.Collections;
using System;

namespace Katana
{
    public partial class Player
    {

        // Update is called once per frame
        public void InputUpdate()
        {
            Motor.InputMoveDirection = CanMove() ? (Input.GetAxisRaw("Horizontal") * Time.deltaTime) * Vector3.right : Vector3.zero;

            Motor.InputJump = Input.GetButtonDown("Jump");

            Animator.SetIsAttack(Input.GetButtonDown("Fire1") && CanAttack());
        }
    }
}