using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Katana
{
    public class PauseManager : Singleton<PauseManager>
    {
        public enum PauseMenuState{
            DefaultMenu,
            SaveMenu,
            OptionMenu,
            TitleMenu
        }

        [HideInInspector]
        public int nowMenuNum = 0;

        public Sprite menu_Normal;
        public Sprite menu_Active;

        [SerializeField]
        UI_FadeAction background;

        [HideInInspector]
        public PauseMenuState nowMenuState = PauseMenuState.DefaultMenu;

        bool _isPause = false;
        bool _isWaiting = false;
        PauseMenu[] _menuList;
        PauseMenuOutline[] outlines;
        float _nowWaitTimer = 0;

        const float inputWaitTime = 0.2f;

        public bool IsPause()
        {
            return _isPause;
        }

        void Start()
        {
            outlines = GameObject.FindObjectsOfType<PauseMenuOutline>();
            ChangePauseMenuState(PauseMenuState.DefaultMenu);
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

        //メニューリストの更新
        public void SetMenuList(Transform menuOutline)
        {
            _menuList = menuOutline.GetComponentsInChildren<PauseMenu>();
            nowMenuNum = 0;
            UpdateMenuActive();
        }

        //ポーズボタンが押された時のアクション
        public void PushPauseButton()
        {
            Pausable.PauseState state = _isPause ? Pausable.PauseState.Active : Pausable.PauseState.PauseAll;
            ChangePauseState(state);
        }

        //上下ボタンが押された時のアクション
        public void PushUpDownButton(bool isUp)
        {
            if (_isWaiting || _menuList.Length <= 0)
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

        //決定ボタンが押された時のアクション
        public void PushDecideButton()
        {
            if (_isWaiting || _menuList.Length <= 0)
                return;

            _menuList[nowMenuNum].Decision();

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

        //ポーズ中かどうかを切り替え
        void ChangePauseState(Pausable.PauseState state)
        {
            _isPause = !_isPause;
            this.gameObject.SetActive(_isPause);
            //開く時にメニューを初期化
            if (_isPause)
                ChangePauseMenuState(PauseMenuState.DefaultMenu);
            UpdateMenuActive();
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

        //ポーズの中のメニューを切り替え
        public void ChangePauseMenuState(PauseMenuState state)
        {
            nowMenuState = state;
            foreach(var outline in outlines)
            {
                outline.ChangeMenu(state);
            }
        }
    }
}
