using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Katana
{
    public class PauseManager : Singleton<PauseManager>
    {
        [HideInInspector]
        public int nowMenuNum = 0;

        public Sprite menu_Normal;
        public Sprite menu_Active;

        [SerializeField]
        UI_FadeAction background;

        bool _isPause = false;
        bool _isWaiting = false;
        PauseMenu[] _menuList;
        float _nowWaitTimer = 0;

        const float inputWaitTime = 0.2f;

        public bool IsPause()
        {
            return _isPause;
        }

        void Start()
        {
            _menuList = GameObject.FindObjectsOfType<PauseMenu>();
            this.gameObject.SetActive(false);
        }

        void FixedUpdate()
        {
            if (_isWaiting)
            {
                _nowWaitTimer -= Time.unscaledDeltaTime;
                if (_nowWaitTimer < 0)
                    _isWaiting = false;
            }
        }

        public void PushPauseButton()
        {
            Pausable.PauseState state = _isPause ? Pausable.PauseState.Active : Pausable.PauseState.PauseAll;
            ChangePauseState(state);
        }

        public void PushUpDownButton(bool isUp)
        {
            if (_isWaiting)
                return;

            if (!isUp)
            {
                nowMenuNum++;
                if (nowMenuNum >= _menuList.Length)
                    nowMenuNum = 0;
            }
            else
            {
                nowMenuNum--;
                if (nowMenuNum < 0)
                    nowMenuNum = _menuList.Length-1;
            }

            UpdateMenuActive();
            _isWaiting = true;
            _nowWaitTimer = inputWaitTime;
        }

        void UpdateMenuActive()
        {
            for(int i=0;i < _menuList.Length; i++)
            {
                _menuList[i].ChangeActive(_menuList[i].pauseNumber == nowMenuNum);
            }
        }

        void ChangePauseState(Pausable.PauseState state)
        {
            UpdateMenuActive();
            _isPause = !_isPause;
            this.gameObject.SetActive(_isPause);
            MenuFadeAction(_isPause);

            foreach (var pause in GameObject.FindObjectsOfType<Pausable>())
            {
                pause.State = state;
            }
            nowMenuNum = 0;
        }

        void MenuFadeAction(bool isFadeIn)
        {

            background.SetFadeInOut(isFadeIn);
        }
    }
}
