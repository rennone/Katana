using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Katana.Hud
{
    public class HudPlayerStatus : HudComponent
    {
        // 表示に使うステータス
        ActorStatus _targetStatus;

        // 実際表示するステータス
        // アニメーションでゆっくりと減っていったりするのに使う
        ActorStatus _displayStatus;

        [SerializeField]
        Text _hp;


        protected override void OnStart()
        {
            base.OnStart();
            _targetStatus = GameManager.Instance.Player.AStatus;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            _hp.text = _targetStatus.Hp.ToString();
        }
    }

}
