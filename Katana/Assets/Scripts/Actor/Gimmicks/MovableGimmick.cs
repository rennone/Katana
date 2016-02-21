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
        public float ThreasoldSpeed = 5;

        public int Strong = 100;

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

        // 衝突したとき
        protected override void OnTriggerEnterWith(Collider c)
        {
            Debug.Log("Collider Gimmick " + c.name);

            // 速度が十分あるか
            if (_rigidbody.velocity.sqrMagnitude < ThreasoldSpeed*ThreasoldSpeed)
            {
                Debug.Log("Less Speed " + _rigidbody.velocity.magnitude);
                return;
            }
            // 移動方向の後ろからぶつかった場合(衝突された場合)、攻撃しない
            var direction = c.transform.position - transform.position;
            if (Vector3.Dot(direction, _rigidbody.velocity) < 0)
            {
                Debug.Log("Direction " + Vector3.Angle(direction, _rigidbody.velocity));
                return;
            }

            var target = c.GetComponent<IDamage>();
            if (target == null)
                return;

            target.Damage(new Damage(gameObject, Strong, c));
        }

    }
}