using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneLoader : MonoBehaviour {

        IEnumerator Load()
        {
            AsyncOperation ao = Application.LoadLevelAsync("Stage02");
            ao.allowSceneActivation = false;
            while (ao.progress < 0.9f)
            {
                //演出などで確実に待ちを入れたい場合は ( ao.progress < 0.9f || 読み込み時間 < 確実に待たせたい時間 ) みたいな感じで判定
                yield return new WaitForEndOfFrame();
            }
            //次のレベルに遷移
            //ao.allowSceneActivation = true;
            //ao.isDoneはfalseのまま
            yield return null;
        }

        // Use this for initialization
        void Start () {
            StartCoroutine("Load");
        }
	
        // Update is called once per frame
        void Update ()
        {
            //Load();
        }
    }
}
