using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {

    [SerializeField]
    private int damage = 30;

	//void OnTriggerEnter(Collider col)
 //   {
 //       if (col.gameObject.layer == LayerMask.NameToLayer("MainCharacter"))
 //       {
 //           col.GetComponent<Player>().DecreaseHP(damage);
 //       }
 //   }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("MainCharacter"))
        {
            col.gameObject.GetComponent<Player>().DecreaseHP(damage);
        }
    }
}
