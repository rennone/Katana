using UnityEngine;
using System.Collections;

namespace Katana
{

    public class Character : Actor
    {
        [SerializeField]
        private ActorStatus _status;

        public ActorStatus AStatus { get { return _status; } }

        protected override void AwakeSelf()
        {
            base.AwakeSelf();
            AStatus.OnDead = OnDead;
            AStatus.OnDamaged = OnDamaged;
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