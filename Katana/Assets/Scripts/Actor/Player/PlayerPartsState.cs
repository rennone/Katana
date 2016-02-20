using System;
using UnityEngine;
using System.Collections;
using Katana.Messages;

namespace Katana
{
    public partial class  Player
    {
        // ゲームを進めていくと, 解放される行動.
        // ゲームパッドの入力と(スティックを除いて)一対一対応する
        // 最大32個
        [System.Serializable]
        public enum Action
        {
            Move  = 1,          // 移動
            Jump  = 1 << 1,     // ジャンプ
            Squat = 1 << 2,     // しゃがむ
            Fire1 = 1 << 3,     // 攻撃1 TODO : 名前要修正
            Fire2 = 1 << 4,     // 攻撃2
        }

        // Actionの項目数
        private readonly int ActionNum = Enum.GetNames(typeof (Action)).Length;

        //! 解放された行動
        private uint _releasedAction = (int)(Action.Move | Action.Jump | Action.Squat);

        bool hasWeapon = false;
        
        public bool CanAttack()
        {
            return hasWeapon;
        }

        //! actionを解放する
        public void ReleaseAttack(Action action)
        {
            _releasedAction |= (uint)action;
            hasWeapon = true;
            
        }

        //! actionが解放されているかどうか
        public bool IsReleased(Action action)
        {
            return (_releasedAction & (uint) action) != 0;
        }

        // アニメーション通常状態
        public bool IsNormalState()
        {
            return _animatorAccess.IsIdleState() || _animatorAccess.IsJumpState() || _animatorAccess.IsRunState();
        }

        // 移動の入力が受付中かどうか
        public bool CanInpuMove()
        {
            return IsNormalState();
        }

        public bool CanInputJump()
        {
            return IsNormalState();
        }

        public bool CanInputAttack()
        {
            return IsNormalState();
        }

        // Update is called once per frame
        void AnimationUpdate()
        {
            _animatorAccess.SetIsJump(_motor.IsJumping());//ジャンプフラグのセット
            _animatorAccess.SetMoveSpeed(_motor.movement.velocity.magnitude + _motor.InputMoveDirection.magnitude * _motor.movement.MaxForwardSpeed);
        }

        public void AnimationCallbackWeaponActive()
        {
            // TODO : 武器を指定できるようにする
            _kick.SetActive(true);
        }

        public void AnimationCallbackWaponInActive()
        {
            // TODO : 武器を指定できるようにする
            _kick.SetActive(false);
        }
    }

}

