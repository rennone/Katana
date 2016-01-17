using UnityEngine;
using System.Collections;

// GameManagerへの追加など
// すべてのアクターの基本処理を行う( 親クラスのコンストラクタ, デストラクタ的な役割 )
public class ActorInitializeFinalize : MonoBehaviour {

	// Use this for initialization
	void Awake () 
    {
        // ゲームマネージャーに登録
        GameManager.I.RegisterActor(GetComponent<IActor>());
	}

    void OnDestory()
    {
        // ゲームマネージャーから削除
        GameManager.I.RegisterActor(GetComponent<IActor>());
    }

}
