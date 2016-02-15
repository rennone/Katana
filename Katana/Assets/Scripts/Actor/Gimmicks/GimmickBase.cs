using UnityEngine;

namespace Katana
{
    public class GimmickBase : AMonoBehaviour, IDamage
    {
        // ギミックを動かす
        public virtual Messages.MoveResult Move(Katana.Messages.Move move)
        {
            return Messages.MoveResult.DefaultSuccess;
        }

        // 壊す
        public virtual Messages.DamageResult Damage(Katana.Messages.Damage damage)
        {
            return Messages.DamageResult.DefaultSuccess;
        }
    }

}
