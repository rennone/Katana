using UnityEngine;
using System.Collections;

namespace Katana
{
    public class AMonoBehaviour : MonoBehaviour
    {
        protected virtual void Update() { }
        protected virtual void Awake() { }
    }
// すべてのキャラクターに共通な機能群を持たせる
    [RequireComponent(typeof (Pausable))] //ポーズ機能
    public class Actor : AMonoBehaviour
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

        protected virtual void AwakeSelf() { }
        protected virtual void UpdateSelf() { }
        protected virtual void StartSelf() { }
        protected virtual void OnDestroySelf() { }

       
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

        protected sealed override void Awake()
        {
            // ゲームマネージャーに登録
            GameManager.Instance.RegisterActor(this);
            AwakeSelf();
        }

        protected sealed override void Update()
        {
            UpdateSelf();
        }

        void Start()
        {
            StartSelf();
        }

        void OnDestroy()
        {
            OnDestroySelf();
            GameManager.Instance.RemoveActor(this);
        }

    }
}
