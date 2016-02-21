using System;
using UnityEngine;
using System.Collections;
using System.Reflection;

// 攻撃(アクション)の際に利用するメッセージ群
namespace Katana
{
    namespace Messages
    {
        public class ResultBase<U> where U : ResultBase<U>
        {
            public static T Construct<T, A>(A arg)
            {
                Type type = typeof(T);
                ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(A) });

                if (ctor == null)
                    throw new NotSupportedException("コンストラクタが定義されていません。");

                return (T)ctor.Invoke(new object[] { arg });
            }

            public static readonly U DefaultSuccess = Construct<U, bool>(true);
            public static readonly U DefaultFailed  = Construct<U, bool>(false);

            public readonly bool success;
            public ResultBase(bool result)
            {
                success = result;
            } 
        }
       
    }

    // 敵(ギミック)を攻撃(破壊)用
    namespace Messages
    {
        // ダメージ用のクラス
        public class Damage
        {
            public int Strong;
            public Vector3 Direction;
            public Collider Collider;
            public WeaponBase Weapon;
            public readonly GameObject Attacker; //攻撃者

            public Damage(GameObject attacker,int strong, Vector3 direction, Collider collider = null, WeaponBase weapon = null)
            {
                Attacker = attacker;
                Strong = strong;
                Direction = direction;
                Collider = collider;
                Weapon = weapon;
            }

            public Damage(GameObject attacker, int strong = 0, Collider collider = null, WeaponBase weapon = null)
                :this(attacker, strong, Vector3.zero, collider, weapon)
            {
            }
        }

        // ダメージの結果クラス
        public class DamageResult : ResultBase<DamageResult>
        {
            public enum StatusResult
            {
                Damaged,        //ダメージを受けた
                Dead,           //死んだ
                Blocked,        //防いだ
                Recovered,      //回復した (ダメージが負)
                FullRecovered   //全回復した(ダメージが負)
            }

            public readonly StatusResult Result;

            public DamageResult(bool success_, StatusResult result)
                :base(success_)
            {
                Result = result;
            }

            public DamageResult(StatusResult result = StatusResult.Damaged)
                :this(result != StatusResult.Blocked, result)
            {
            }

            public DamageResult(bool success)
                : this(success, success ? StatusResult.Damaged : StatusResult.Blocked)
            {
                
            }
        }
    }

    // ギミックを動かす用
    namespace Messages
    {
        public class Move
        {
            public readonly Vector3 Direction;

            public Move(Vector3 direction)
            {
                Direction = direction;
            }
        }

        public class MoveResult : ResultBase<MoveResult>
        {
            public MoveResult(bool sucecss)
                :base(sucecss)
            {
                
            }
        }
    }
}