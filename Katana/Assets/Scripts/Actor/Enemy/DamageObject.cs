using UnityEngine;
using System.Collections;

public class DamageObject : MonoBehaviour
{
    [SerializeField]
    int strong_ = 10;   //攻撃力
    public int Strong { get { return strong_; } }



    void OnTriggerEnter(Collider col)
    {
        // 通常状態のPlayerと当たったときにダメージを与える
        if (col.gameObject.layer == LayerMask.NameToLayer("MainCharacter"))
        {
            col.GetComponent<PlayerController>().Damage(Strong);
        }
    }
}
