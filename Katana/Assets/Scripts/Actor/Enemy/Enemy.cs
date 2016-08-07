using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Katana.Messages;

namespace Katana
{
// Controller (Baseクラスなのでこれを継承して他のクラスを定義してもよい)
// 他のオブジェクトとのインターフェースとしての役割をする.
// 敵の共通処理を書く
    public class Enemy : Character
    {
		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
		}

		public void Damage(int val)
		{
			base.Damage(new Damage(null, val));
		}

		protected override void OnDead(Damage damage)
		{
			base.OnDead(damage);
		}

		protected override void OnDamaged(DamageResult damage)
		{
			base.OnDamaged(damage);
			//Debug.Log("Remain HP = " + AStatus.Hp);
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnFilnalize()
		{
			base.OnFilnalize();
		}
			
    }
}
