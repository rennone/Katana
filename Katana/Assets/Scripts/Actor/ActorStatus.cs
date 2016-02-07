using System;
using UnityEngine;
using System.Collections;

// CharacterのHPなどのステータスを表す親クラス
// 

namespace Katana
{
    // ダメージ情報
    public class DamageInfo
    {
        
    }


    [System.Serializable]
    public class ActorStatus
    {
        // 最大HP
        public int MaxHp = 10000;

        // HP
        public int Hp;

        // 攻撃力
        public int Strong = 10;

        // 防御力
        public int Defense = 10;

        ActorStatus()
        {
            Hp = MaxHp;
            Debug.Log("Constructor");
        }

        //HP減らす処理
        public virtual void DecreaseHP(int val)
        {
            bool alive = Hp > 0;
            Hp -= val;

            // 死んだとき
            if (alive && Hp <= 0)
            {
                Hp = 0;

                if (OnDead != null)
                {
                    OnDead();
                    return;
                }
            }

            OnDamaged(new DamageInfo());
        }

        //HP増やす処理
        public virtual void IncreaseHP(int val)
        {
            bool full = Hp == MaxHp;
            Hp += val;

            // 完全回復したとき
            if (!full && Hp >= MaxHp)
            {
                Hp = MaxHp;

                if (OnFullRecovered != null)
                    OnFullRecovered();
            }
        }

        //protected virtual void OnDead() { }

        //protected virtual void OnFullRecovered() { }





     

        public Action OnDead { private get; set; }
        public Action OnFullRecovered { private get; set; }

        public Action<DamageInfo> OnDamaged { private get; set; } 

    }
}