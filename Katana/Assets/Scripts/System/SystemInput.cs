using UnityEngine;
using System.Collections;

// ポーズ画面など, ゲームに関係ないインプット回り
namespace Katana
{
    public class SystemInput : MonoBehaviour
    {
        bool _isPause = false;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Pausable.PauseState state = _isPause ? Pausable.PauseState.Active : Pausable.PauseState.PauseAll;
                foreach (var pause in GameObject.FindObjectsOfType<Pausable>())
                {
                    pause.State = state;
                }

                _isPause = !_isPause;
            }
        }
    }
}