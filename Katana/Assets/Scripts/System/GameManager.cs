using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    [HideInInspector]
    public string nowSceneName;

    void Start()
    {
        nowSceneName = Application.loadedLevelName;
    }

	public void GameRestart()
    {
        Application.LoadLevel(nowSceneName);
    }
}
