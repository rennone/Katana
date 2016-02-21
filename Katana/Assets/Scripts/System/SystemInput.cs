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
            if (PauseManager.I != null)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    PauseManager.I.PushPauseButton();
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    PauseManager.I.PushDecideButton();
                }

                if (PauseManager.I.IsPause())
                {
                    float inputScale = Input.GetAxisRaw("Vertical");
                    if (inputScale > 0)
                    {
                        PauseManager.I.PushUpDownButton(true);
                    }
                    else if (inputScale < 0)
                    {
                        PauseManager.I.PushUpDownButton(false);
                    }
                }
            }
        }
    }
}