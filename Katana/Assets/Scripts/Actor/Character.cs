using UnityEngine;
using System.Collections;
using Katana.Messages;

namespace Katana
{

    public class Character : Actor, IDamage
    {
        [SerializeField]
        private ActorStatus _status;

        public ActorStatus AStatus { get { return _status; } }
        
        // 攻撃する
        public virtual Messages.DamageResult Attack(IDamage target, Messages.Damage damage)
        {
            damage.Strong += AStatus.Strong;
            damage.Direction = transform.forward;
            return target.Damage(damage);
        }

        public DamageResult Damage(Damage damage)
        {
            var result = damage.Strong < 0 ? AStatus.IncreaseHP((uint) -damage.Strong) : AStatus.DecreaseHP((uint)damage.Strong);

            DamageResult ret = new DamageResult(result);
            switch (result)
            {
                case DamageResult.StatusResult.Dead:
                    OnDead();
                    break;
                case DamageResult.StatusResult.Damaged:
                    OnDamaged(ret);
                    break;
                case DamageResult.StatusResult.Recovered:
                    OnRecover();
                    break;
            }

            return ret;
        }

        public MoveResult Move(Move move)
        {
            throw new System.NotImplementedException();
        }

        //// ダメージ
        //public virtual void Damage(int val)
        //{
        //    AStatus.DecreaseHP(val);
        //    OnDamaged(null);
        //}

        //// 回復
        //public virtual void Recover(int val)
        //{
        //    AStatus.IncreaseHP(val);
        //    OnRecover();
        //}

        protected virtual void OnDamaged(DamageResult damage)
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