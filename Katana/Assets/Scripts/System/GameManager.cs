using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    [HideInInspector]
    public string nowSceneName;

    bool isTheWorld = false;
    bool isPause = false;

    void Start()
    {
        nowSceneName = Application.loadedLevelName;
    }

    public float GetDeltaTime(string tag = "Default")
    {
        float deltaTime = Time.deltaTime;
        switch (tag)
        {
            case "Player":
                if (isPause)
                    deltaTime = 0;
                break;
            default:
                if (isTheWorld || isPause)
                    deltaTime = 0;
                break;
        }
        return deltaTime;
    }

    public void TheWorld(bool isTimeStop)
    {
        isTheWorld = isTimeStop;
    }

    public void Pause(bool isTimeStop)
    {
        isPause = isTimeStop;
    }

	public void GameRestart()
    {
        Application.LoadLevel(nowSceneName);
    }
}
