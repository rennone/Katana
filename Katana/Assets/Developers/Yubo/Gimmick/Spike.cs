using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {

	void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("MainCharacter"))
        {
            //ここに主人公と当たったらライフ減らす処理を記述
        }
    }
}
