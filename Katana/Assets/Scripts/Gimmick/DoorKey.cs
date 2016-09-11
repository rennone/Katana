using UnityEngine;
using System.Collections;
using Katana.Hud;

namespace Katana{
	public class DoorKey : GameItem{

        bool isCollisionEnd = false;

        // 解放したときに表示するメッセージ
        [SerializeField]
        private string[] _messages;

        void OnTriggerEnter(Collider collider)
        {
            if (isCollisionEnd) return;

            if (collider.tag != TagName.Player)
                return;

            // TODO : 以下の処理は本来ここでは行わず, エフェクトの後に行うべき
            // 例えば, プレイヤーの動きを止めて、このアイテムがプレイヤーに吸い込まれるなど

            var player = collider.GetComponent<Player>();

            //持ち物に鍵を追加
            player.CatchItem(ItemKind.DoorKey);

            // メッセージを表示
            Hud.HudManager.AddMessage(_messages, HudMessage.MessageKind.Hint);

            // 自分を破壊する
            Destroy(this.gameObject);

            isCollisionEnd = true;
        }

    }
}
