using System;
using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    [HideInInspector]
    public string nowSceneName;

    private DebugViewer debugCanvas_;

    void Start()
    {
        nowSceneName = Application.loadedLevelName;

        GameObject canvas = GameObject.Instantiate(Resources.Load("Prefab/DebugMenu/DebugCanvas")) as GameObject;
        debugCanvas_ = canvas.GetComponent<DebugViewer>();
        debugCanvas_.gameObject.SetActive(false);

        StartCoroutine("CheckDebugViewer");
    }

	public void GameRestart()
    {
        Application.LoadLevel(nowSceneName);
    }

    //! デバッグメニューの更新処理
    
    IEnumerator CheckDebugViewer()
    {
        const float triggerTime = 1.0f;

        float wheelPushedTime = 0.0f;
        while (true)    
        {

            if (Input.GetMouseButton(2))
            {
                wheelPushedTime += Time.deltaTime;
            }

            if (Input.GetMouseButtonUp(2))
            {
                wheelPushedTime = 0.0f;
            }
            
            if (wheelPushedTime > triggerTime)
            {
                debugCanvas_.InvertActive();
                wheelPushedTime = 0.0f;
            }
            
            yield return null;
        }
    }
}
