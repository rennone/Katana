using UnityEngine;
using System.Collections;

// GameManagerへの追加など
// すべてのアクターの基本処理を行う( 親クラスのコンストラクタ的な役割 )
public class ActorInitializeFinalize : MonoBehaviour {

	// Use this for initialization
	void Awake () 
    {
        Debug.Log("ActorInitializer");
        GameManager.I.RegisterActor(GetComponent<IActor>());
	}

    void OnDestory()
    {
        GameManager.I.RegisterActor(GetComponent<IActor>());
    }

}
