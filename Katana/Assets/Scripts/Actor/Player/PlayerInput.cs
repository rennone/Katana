using UnityEngine;
using System.Collections;
using System;

namespace Katana
{

    [RequireComponent(typeof (ActorMotor))]
    public class PlayerInput : MonoBehaviour
    {


        private ActorMotor motor_ = null;
        // Use this for initialization
        private void Start()
        {
            motor_ = GetComponent<ActorMotor>();
        }

        // Update is called once per frame
        private void Update()
        {
            motor_.InputMoveDirection = (Input.GetAxisRaw("Horizontal")*Time.deltaTime)*Vector3.right;
            motor_.InputJump = Input.GetButtonDown("Jump");

            if (Input.GetKeyDown(KeyCode.A))
            {
                var player = GetComponent<Player>();
            }
        }
    }
}