using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ActorHolder))]
public class GameManager : Singleton<GameManager> {

    [HideInInspector]
    public string nowSceneName;

    private ActorHolder holder_;

    ActorHolder Holder
    {
        get
        {
            if(holder_ == null)
                holder_ = GetComponent<ActorHolder>();
            return holder_;
        }
    }

    public PlayerController Player { get { return holder_.Player; } }

    new void Awake()
    {
        base.Awake();
    }

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

    // Actorを登録する
    public void RegisterActor(ActorController actor)
    {
        Holder.RegisterActor(actor);
    }

    public void RemoveActor(ActorController actor)
    {
        GetComponent<ActorHolder>().RemoveActor(actor);
    }

    public void Pause()
    {

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
