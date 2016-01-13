using UnityEngine;
using System.Collections;

// Controller (Baseクラスなのでこれを継承して他のクラスを定義してもよい)
// 他のオブジェクトとのインターフェースとしての役割をする.
// 衝突時のダメージ関数などはここに書いておいて
// 入力(AI)やステータスは他のスクリプトに書く

[RequireComponent(typeof(ActorStatus))]
public class EnemyController : MonoBehaviour 
{
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collide");
        // プレイヤーとぶつかった処理
        if (collider.gameObject.layer == LayerName.Player)
        {
            var player = collider.gameObject.transform.GetComponent<PlayerController>();
            var status = GetComponent<ActorStatus>();   //ステータスを取得
            player.Damage(status.Strong);               //プレイヤーに攻撃
        }
    }

    public virtual void Damage(int val)
    {
        var status = GetComponent<ActorStatus>();
        status.DecreaseHP(val);
    }

    public virtual void Recover(int val)
    {
        var status = GetComponent<ActorStatus>();
        status.IncreaseHP(val);
    }


}
