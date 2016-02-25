using UnityEngine;
using System.Collections;
using Katana.Hud;

namespace Katana
{
    //! プレイヤーの行動を解放するアイテム
    //! 解放時のルーチンさえ共通化できればこのクラス一つで完結する
    public class ActionOpener : MonoBehaviour
    {
        // 解放する行動
        [SerializeField]
        private Player.Action _openedAction;

        // 解放したときに表示するメッセージ
        [SerializeField] 
        private string[] _messages;



        void OnTriggerEnter(Collider collider)
        {
            if (collider.tag != TagName.Player)
                return;

            // TODO : 以下の処理は本来ここでは行わず, エフェクトの後に行うべき
            // 例えば, プレイヤーの動きを止めて、このアイテムがプレイヤーに吸い込まれるなど

            var player = collider.GetComponent<Player>();

            // 行動を解放
            player.ReleaseAttack(_openedAction); //武器開放

            // メッセージを表示
            Hud.HudManager.AddMessage(_messages, HudMessage.MessageKind.Hint);

            // 自分を破壊する
            Destroy(this.gameObject);
        }


        // TODO : アイテムのエフェクトはここではなく別のスクリプトに書くべき
        void Update()
        {
            // 画像を回転させる
            var rotSpeed = 100;
            transform.Rotate(Vector3.forward, Time.deltaTime * rotSpeed);
        }
    }
}