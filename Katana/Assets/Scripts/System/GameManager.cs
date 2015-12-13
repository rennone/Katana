using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    [HideInInspector]
    public string nowSceneName;

    //プレイヤーまたはプレイヤーのスキルで時間停止しないもの用の時間
    float playerDeltaTime;
    public float PlayerDeltaTime
    {
        get { return playerDeltaTime; }
    }

    //プレイヤーのスキルで時間停止するもの用の時間
    float notPlayerDeltaTime;
    public float NotPlayerDeltaTime
    {
        get { return notPlayerDeltaTime; }
    }

    bool isTheWorld = false;
    bool isPause = false;

    void Start()
    {
        nowSceneName = Application.loadedLevelName;
    }

    void Update()
    {
        playerDeltaTime = Time.deltaTime;
        notPlayerDeltaTime = Time.deltaTime;
        if (isTheWorld)
            notPlayerDeltaTime = 0f;
        if (isPause)
        {
            playerDeltaTime = 0;
            notPlayerDeltaTime = 0;
        }
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
