using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AnimatorAccess;

namespace Katana
{
    public class EnemyTest : Platforms.Patroller
    {
        PlayerMotor motor_;
        private Player _owner;

        private SearchEye _eye;

        protected virtual void Awake()
        {
            motor_ = GetComponent<PlayerMotor>();
            _owner = GetComponent<Player>();

            _eye = GetComponentInChildren<SearchEye>();
            _eye.OnFind = FindPlayer;
            _eye.OnLoseSight = LostPlayer;
        }

        protected override void Update()
        {
            UpdateMove();

            var enable = false;
            if (_eye.Find)
            {
                var distance = (GameManager.Instance.Player.transform.position - transform.position);
                if (distance.sqrMagnitude < 2)
                {
                    enable = true;
                }
            }
            _owner.AnimatorAccess.SetIsFire2(enable);
        }

        void UpdateMove()
        {
            if (_owner.CanInpuMove() == false)
            {
                motor_.InputMoveDirection = Vector3.zero;
                return;
                
            }

            if (_eye.Find)
            {
                var distance = (GameManager.Instance.Player.transform.position - transform.position);
                distance.y = 0;
                motor_.InputMoveDirection = distance.x > 0 ? Vector3.right :  Vector3.left;
            }
            else
            {
                var distance = Destination - transform.position;
                distance.Normalize();
                distance.y = 0;
                distance.z = 0;

                // 移動量
                var offset = motor_.movement.MaxForwardSpeed * Time.deltaTime; //TODO : Timeを書き換え

                // 次に進む
                if (distance.sqrMagnitude <= offset * offset)
                {
                    Next();
                    return;
                }
                motor_.InputMoveDirection = distance;
            }
        }

        void FindPlayer()
        {
            Debug.Log("Find Player");
            motor_.movement.MaxSpeed = 50;
        }

        void LostPlayer()
        {
            Debug.Log("Lost Player");
            motor_.movement.MaxSpeed = 5;
        }
    }
}

