using UnityEngine;

namespace Katana
{
    public class GimmickBase : ACollider, IDamage
    {
        // ギミックを動かす
        public virtual Messages.MoveResult Move(Katana.Messages.Move move)
        {
            return Messages.MoveResult.DefaultSuccess;
        }

        // ギミックが攻撃される
        public virtual Messages.DamageResult Damage(Katana.Messages.Damage damage)
        {
            return Messages.DamageResult.DefaultSuccess;
        }

    }

}
