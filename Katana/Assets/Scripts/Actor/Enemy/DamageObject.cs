using UnityEngine;
using System.Collections;

namespace Katana
{
    // Playerに当たるとダメージを与えるオブジェクト
    public class DamageObject : Enemy
    {
        [SerializeField]
        int strong_ = 10;   //攻撃力
        public int Strong { get { return strong_; } }


        void OnTriggerEnter(Collider col)
        {
            Debug.Log("Hit Player");
            // 通常状態のPlayerと当たったときにダメージを与える
            if (col.gameObject.layer == LayerName.Player)
            {
                col.GetComponent<Player>().Damage(Strong);
            }
        }
    }

}
