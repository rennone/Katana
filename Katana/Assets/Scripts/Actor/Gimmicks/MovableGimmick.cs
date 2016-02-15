using UnityEngine;
using System.Collections;
using Katana.Messages;

namespace Katana
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovableGimmick : GimmickBase
    {
        Rigidbody _rigidbody;

        protected override void OnInitialize()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        public override Messages.MoveResult Move(Messages.Move move)
        {
            _rigidbody.AddForce(move.Direction);
            return base.Move(move);
        }

        public override DamageResult Damage(Damage damage)
        {
            _rigidbody.AddForce(damage.Direction * damage.Strong);
            return base.Damage(damage);
        }
    }
}