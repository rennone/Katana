using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Katana
{

    [RequireComponent(typeof (ActorHolder))]
    public class GameManager : Singleton<GameManager>
    {

        [HideInInspector] public string nowSceneName;

        private ActorHolder holder_;

        private ActorHolder Holder
        {
            get
            {
                if (holder_ == null)
                    holder_ = GetComponent<ActorHolder>();
                return holder_;
            }
        }

        public Player Player
        {
            get { return holder_.Player; }
        }

        private void Start()
        {
            nowSceneName = Application.loadedLevelName;

            // デバッグ機能の初期化
#if _DEBUG
        InitDebugViewer();
#endif
            SoundManager.Instance.PlayBgm(SoundKey.BGM_COMBAT);
        }

        public void GameRestart()
        {
            Application.LoadLevel(Application.loadedLevelName);
        }

        // Actorを登録する
        public void RegisterActor(Actor actor)
        {
            Holder.RegisterActor(actor);
        }

        public void RemoveActor(Actor actor)
        {
            GetComponent<ActorHolder>().RemoveActor(actor);
        }

        public void Pause()
        {

        }

        void OnDestroy()
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
}