using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneMerge : MonoBehaviour {

    public const string StageHeader = "Area00-";
    public static Scene ActiveStage { get; private set; }

	void Awake()
    {
        var nowScene = SceneManager.GetActiveScene();
        if (nowScene.name.Contains(StageHeader))
        {
            ActiveStage = nowScene;
            if (!SceneManager.GetSceneByName("InGameCommon").isLoaded)
            {
                SceneManager.LoadScene("InGameCommon", LoadSceneMode.Additive);
            }

        }
    }
}
