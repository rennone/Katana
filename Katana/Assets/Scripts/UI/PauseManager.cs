using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace Katana
{
    public class PauseManager : Singleton<PauseManager>
    {

        [SerializeField]
        UI_FadeAction background;

        bool _isPause = false;

        void Start()
        {
            this.gameObject.SetActive(false);
        }

        public void PushPauseButton()
        {
            Pausable.PauseState state = _isPause ? Pausable.PauseState.Active : Pausable.PauseState.PauseAll;
            ChangePauseState(state);
        }

        void ChangePauseState(Pausable.PauseState state)
        {
            _isPause = !_isPause;
            this.gameObject.SetActive(_isPause);
            MenuFadeAction(_isPause);

            foreach (var pause in GameObject.FindObjectsOfType<Pausable>())
            {
                pause.State = state;
            }
        }

        void MenuFadeAction(bool isFadeIn)
        {

            background.SetFadeInOut(isFadeIn);
        }
    }
}
