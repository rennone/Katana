using UnityEngine;
using System.Collections;

namespace Katana
{
    public interface IDamage
    {
        Messages.DamageResult Damage(Messages.Damage damage);

        Messages.MoveResult Move(Messages.Move move);
    }
}

