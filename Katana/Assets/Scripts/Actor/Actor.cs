using UnityEngine;
using System.Collections;

namespace Katana
{
    // GameManagerが管理可能なクラス
    // 生成時にGameManagerに登録, 破壊時にGameManagerから削除される.
    public class Actor : AMonoBehaviour
    {
        protected sealed override void Awake()
        {
            // ゲームマネージャーに登録
            GameManager.Instance.RegisterActor(this);
            OnInitialize();
        }

        protected sealed override void Update()
        {
            OnUpdate();
        }

        protected override sealed void Start()
        {
            OnStart();
        }

        protected override sealed void OnDestroy()
        {
            OnFilnalize();  //終了処理
            GameManager.Instance.RemoveActor(this);
        }

    }
}
