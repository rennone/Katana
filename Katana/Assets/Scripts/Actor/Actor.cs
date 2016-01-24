using UnityEngine;
using System.Collections;

namespace Katana
{
// すべてのキャラクターに共通な機能群を持たせる
    [RequireComponent(typeof (Pausable))] //ポーズ機能
    [RequireComponent(typeof (ActorStatus))] //ステータス
    [RequireComponent(typeof (ActorInitializeFinalize))] //コンストラクタ, デストラクタ
    public class Actor : MonoBehaviour
    {
        private Pausable _pause = null;

        public Pausable APause
        {
            get
            {
                if (_pause == null)
                    _pause = GetComponent<Pausable>();

                return _pause;
            }
        }

        private ActorStatus _status = null;

        public ActorStatus AStatus
        {
            get
            {
                if (_status == null)
                    _status = GetComponent<ActorStatus>();

                return _status;
            }
        }

        // ダメージ
        public virtual void Damage(int val)
        {
            AStatus.DecreaseHP(val);
        }

        // 回復
        public virtual void Recover(int val)
        {
            AStatus.IncreaseHP(val);
        }

        // ポーズ
        public virtual void Pause()
        {
            APause.State = Pausable.PauseState.PauseAll;
        }

        // ポーズから戻る
        public virtual void Resume()
        {
            APause.State = Pausable.PauseState.Active;
        }
    }
}
