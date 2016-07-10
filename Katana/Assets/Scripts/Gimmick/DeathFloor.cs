using UnityEngine;
using System.Collections;

public class DeathFloor : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            var character = col.GetComponent<Katana.Character>();
            col.GetComponent<Katana.Character>().Damage(new Katana.Messages.Damage(this.gameObject, character.AStatus.MaxHp, Vector3.up));
        }
    }
}
