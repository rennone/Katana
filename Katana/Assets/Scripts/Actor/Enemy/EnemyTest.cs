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

        private SeachEye _eye;

        protected virtual void Awake()
        {
            motor_ = GetComponent<PlayerMotor>();
            _owner = GetComponent<Player>();

            _eye = GetComponentInChildren<SeachEye>();
            _eye.OnFind = () => { Debug.Log("FInd"); };
            _eye.OnLoseSight = () => { Debug.Log("Loss Find"); };
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
            _owner._animatorAccess.SetIsFire2(enable);
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
                motor_.InputMoveDirection = new Vector3(distance.x > 0 ? 1 : -1, 0, 0);
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
        private int _range = 20;

    }
}

