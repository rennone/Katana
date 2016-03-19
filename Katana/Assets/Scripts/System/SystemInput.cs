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
                    float inputVertical = Input.GetAxisRaw("Vertical");
                    float inputHorizontal = Input.GetAxisRaw("Horizontal");
                    if (inputVertical > 0)
                    {
                        PauseManager.Instance.PushUpDownButton(true);
                    }
                    else if (inputVertical < 0)
                    {
                        PauseManager.Instance.PushUpDownButton(false);
                    }

                    if(inputHorizontal > 0)
                    {
                        PauseManager.Instance.PushRightLeftButton(true);
                    }else if(inputHorizontal < 0)
                    {
                        PauseManager.Instance.PushRightLeftButton(false);
                    }

                }
            }
        }
    }
}