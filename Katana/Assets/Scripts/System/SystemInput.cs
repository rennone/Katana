using UnityEngine;
using System.Collections;

// ポーズ画面など, ゲームに関係ないインプット回り
namespace Katana
{
    public class SystemInput : MonoBehaviour
    {

        void Start()
        {

        }

        void Update()
        {
            if (PauseManager.Instance != null)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    PauseManager.Instance.PushPauseButton();
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    PauseManager.Instance.PushDecideButton();
                }

                if (PauseManager.Instance.IsPause())
                {
                    float inputScale = Input.GetAxisRaw("Vertical");
                    if (inputScale > 0)
                    {
                        PauseManager.Instance.PushUpDownButton(true);
                    }
                    else if (inputScale < 0)
                    {
                        PauseManager.Instance.PushUpDownButton(false);
                    }
                }
            }
        }
    }
}