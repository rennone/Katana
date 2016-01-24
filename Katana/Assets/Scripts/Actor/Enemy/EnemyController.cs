using UnityEngine;
using System.Collections;
using Katana;

namespace Katana
{
// Controller (Baseクラスなのでこれを継承して他のクラスを定義してもよい)
// 他のオブジェクトとのインターフェースとしての役割をする.
// 敵の共通処理を書く

    [RequireComponent(typeof (ActorStatus))]
    public class Enemy : Actor
    {
        //private void OnTriggerEnter(Collider collider)
        //{
        //    Debug.Log("Collide");
        //    // プレイヤーとぶつかった処理
        //    if (collider.gameObject.layer == LayerName.Player)
        //    {
        //        var player = collider.gameObject.transform.GetComponent<Player>();
        //        var status = GetComponent<ActorStatus>(); //ステータスを取得
        //        player.Damage(status.Strong); //プレイヤーに攻撃
        //    }
        //}
    }
}
