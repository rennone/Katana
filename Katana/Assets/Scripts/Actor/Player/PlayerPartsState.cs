﻿using System;
using UnityEngine;
using System.Collections;
using Katana.Messages;

// 変数の宣言.
// 入力の可否など非常にシンプルな関数群はこっちに書く
namespace Katana
{
    public partial class  Player
    {
        protected PlayerMotor _motor;

        protected CapsuleCollider _capsule { get; set; }

        private WeaponBase _kick;

        public PlayerAnimator AnimatorAccess { get; private set; }

        // 衝突判定に使う
        // コライダの頂上の位置
        public Vector3 Top
        {
            get { return transform.TransformPoint(Vector3.up * _capsule.height / 2 + _capsule.center); }
        }

        // コライダの底の位置
        public Vector3 Bottom
        {
            get { return transform.TransformPoint(-Vector3.up * _capsule.height / 2 + _capsule.center); }
        }


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
        //private readonly int ActionNum = Enum.GetNames(typeof (Action)).Length;

        //! 解放された行動
        private uint _releasedAction = (int)(Action.Move | Action.Jump | Action.Squat);

        //! actionを解放する
        public void ReleaseAttack(Action action)
        {
            _releasedAction |= (uint)action;
        }

        //! actionが解放されているかどうか
        public bool IsReleased(Action action)
        {
            return (_releasedAction & (uint) action) != 0;
        }

        // アニメーション通常状態
        public bool IsNormalState()
        {
            return AnimatorAccess.IsIdleState() || AnimatorAccess.IsJumpState() || AnimatorAccess.IsRunState();
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

        public bool CanInputJumpAttack()
        {
            return AnimatorAccess.IsJumpState();
        }

        // Update is called once per frame
        void AnimationUpdate()
        {
            AnimatorAccess.SetIsJump(_motor.IsJumping());//ジャンプフラグのセット
            AnimatorAccess.SetMoveSpeed(_motor.movement.velocity.magnitude + _motor.InputMoveDirection.magnitude * _motor.movement.MaxForwardSpeed);
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


        // private:
        void InitializeComponent()
        {
            _motor = GetComponent<PlayerMotor>();
            _capsule = GetComponent<CapsuleCollider>();
            _kick = GetComponentInChildren<WeaponBase>();

            _motor.CanChangeDirection = IsNormalState;
            
            AnimatorAccess = GetComponent<PlayerAnimator>();
        }

        public void SetPlayerPosition(Vector3 position)
        {
            this.transform.position = position;
        }
    }

}

