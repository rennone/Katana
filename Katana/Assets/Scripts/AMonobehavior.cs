using UnityEngine;
using System.Collections;

namespace Katana
{
    // 継承してsealed をつけるための親クラス
    public class AMonoBehaviour : MonoBehaviour
    {
        // Monobehaviorが用意している関数
        protected virtual void Update() { OnUpdate(); }

        protected virtual void Awake() { OnInitialize(); }

        protected virtual void OnDestroy() { OnFilnalize(); }

        protected virtual void Start() { OnStart(); }

        protected virtual void OnGUI() { OnDraw();}


        // 上記関数の代わりになるもの
        // Awakeの代わり
        protected virtual void OnInitialize() { }

        // Updateの代わり
        protected virtual void OnUpdate() { }

        // Startの代わり
        protected virtual void OnStart() { }

        // OnDestroyの代わり
        protected virtual void OnFilnalize() { }

        // OnGUIの代わり
        protected virtual void OnDraw() { }
    }
}
