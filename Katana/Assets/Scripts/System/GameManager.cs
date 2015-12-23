using System;
using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    [HideInInspector]
    public string nowSceneName;

  

    void Start()
    {
        nowSceneName = Application.loadedLevelName;
        
        // デバッグ機能の初期化
#if _DEBUG
        InitDebugViewer();
#endif
    }

	public void GameRestart()
    {
        Application.LoadLevel(nowSceneName);
    }

    //! 以下デバッグ用
#if _DEBUG
    private DebugViewer debugCanvas_;
    void InitDebugViewer()
    {
        Debug.Log("Init Debug Viewer");
        GameObject canvas = GameObject.Instantiate(Resources.Load("Prefabs/DebugMenu/DebugCanvas")) as GameObject;
        debugCanvas_ = canvas.GetComponent<DebugViewer>();
        debugCanvas_.gameObject.SetActive(false);
        StartCoroutine("CheckDebugViewer");

    }
    
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
#endif
}
