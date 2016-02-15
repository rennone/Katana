using UnityEngine;
using System.Collections;

namespace Katana
{

    public class Character : Actor
    {
        [SerializeField]
        private ActorStatus _status;

        public ActorStatus AStatus { get { return _status; } }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AStatus.OnDead = OnDead;
            AStatus.OnDamaged = OnDamaged;
        }

        // 攻撃する
        public virtual Messages.DamageResult Attack(IDamage target, Messages.Damage damage)
        {
            damage.Strong += AStatus.Strong;
            damage.Direction = transform.forward;
            return target.Damage(damage);
        }

        // ダメージ
        public virtual void Damage(int val)
        {
            AStatus.DecreaseHP(val);
            OnDamaged(null);
        }

        // 回復
        public virtual void Recover(int val)
        {
            AStatus.IncreaseHP(val);
            OnRecover();
        }

        protected virtual void OnDamaged(DamageInfo damage)
        {

        }

        protected virtual void OnRecover()
        {

        }

        protected virtual void OnDead()
        {

        }
    }
}