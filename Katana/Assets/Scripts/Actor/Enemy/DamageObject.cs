using UnityEngine;
using System.Collections;

namespace Katana
{
    // Playerに当たるとダメージを与えるオブジェクト
    public class DamageObject : Enemy
    {
        void OnTriggerEnter(Collider col)
        {
            // 通常状態のPlayerと当たったときにダメージを与える
            if (col.gameObject.layer == LayerName.Player)
            {
                col.GetComponent<Player>().Damage(AStatus.Strong);
            }
        }
    }

}
