using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {

    [SerializeField]
    private int damage = 30;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerName.Player)
        {
            col.GetComponent<PlayerController>().Damage(damage);
        }
    }

    //void OnCollisionEnter(Collision col)
    //{
    //    Debug.Log("Collide to Player");
    //    if (col.gameObject.layer == LayerMask.NameToLayer("MainCharacter"))
    //    {
    //        col.gameObject.GetComponent<Player>().DecreaseHP(damage);
    //    }
    //}
}
