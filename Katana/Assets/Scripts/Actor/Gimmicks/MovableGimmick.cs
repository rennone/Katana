using UnityEngine;
using System.Collections;
using Katana.Messages;

namespace Katana
{
    // 押すことができるオブジェクト
    [RequireComponent(typeof(Rigidbody))]
    public class MovableGimmick : GimmickBase
    {
        Rigidbody _rigidbody;

        // 衝突したときに, threasold以上の速度ならば
        // 衝突相手にダメージを与える。
        public float threasoldSpeed = 5;

        protected override void OnInitialize()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        // 
        public override Messages.MoveResult Move(Messages.Move move)
        {
            _rigidbody.AddForce(move.Direction);
            return base.Move(move);
        }

        // 攻撃されたとき
        public override DamageResult Damage(Damage damage)
        {
            _rigidbody.AddForce(damage.Direction * damage.Strong);
            return base.Damage(damage);
        }

        protected override void OnTriggerEnterWith(Collider c)
        {
            Debug.Log("Collider Gimmick " + c.name);

            if (_rigidbody.velocity.sqrMagnitude < threasoldSpeed * threasoldSpeed)
                return;

            Debug.Log("Collider Hit" + _rigidbody.velocity);
        }

    }
}