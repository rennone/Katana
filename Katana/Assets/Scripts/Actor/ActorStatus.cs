using System;
using UnityEngine;
using System.Collections;
using Katana.Messages;

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
        public int Hp = 10000;

        // 攻撃力
        public int Strong = 10;

        // 防御力
        public int Defense = 10;

        //HP減らす処理
        public virtual Messages.DamageResult.StatusResult DecreaseHP(uint val)
        {
            bool alive = Hp > 0;
            Hp -= (int)val;

            // 死んだとき
            if (alive && Hp <= 0)
            {
                Hp = 0;
                return DamageResult.StatusResult.Dead;
            }

            return DamageResult.StatusResult.Damaged;
        }

        //HP増やす処理
        public virtual Messages.DamageResult.StatusResult IncreaseHP(uint val)
        {
            bool full = Hp == MaxHp;
            Hp += (int)val;

            // 完全回復したとき
            if (!full && Hp >= MaxHp)
            {
                Hp = MaxHp;

                return DamageResult.StatusResult.FullRecovered;
            }

            return DamageResult.StatusResult.Recovered;
        }
    }
}