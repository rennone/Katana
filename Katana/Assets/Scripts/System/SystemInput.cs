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
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (PauseManager.I != null)
                    PauseManager.I.PushPauseButton();
            }
        }
    }
}