using UnityEngine;
using System.Collections;
using Katana.Messages;

namespace Katana{
	public class TestEnemy : Katana.Enemy 
	{
		[SerializeField]
		Rigidbody thisRigid;
		[SerializeField]
		Collider thisCollider;
		[SerializeField]
		GameItem gameItem;

		protected override void OnStart ()
		{
			base.OnStart ();

		}

		protected override void OnDead(Damage damage)
    	{
			this.thisCollider.enabled = false;
			thisRigid.useGravity = false;
			thisRigid.AddForce(damage.Direction * 1000f);
			StartCoroutine(DeadHide());
			if(gameItem != null){
				var item = Instantiate(gameItem,this.transform.position,Quaternion.identity) as GameItem;
			}
    	}

		IEnumerator DeadHide(){
			yield return new WaitForSeconds(3f);
			Destroy(this.gameObject);
		}
	}
}
