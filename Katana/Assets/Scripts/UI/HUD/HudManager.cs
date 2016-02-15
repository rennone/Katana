using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Katana.Hud
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] 
        private HudMessage _message;

        private static HudManager Instance;

        void Awake()
        {
            Instance = this;
        }

        public static bool AddMessage(string message, HudMessage.MessageKind kind)
        {
            if(Instance == null)
                return false;

            Instance._message.AddMessage(message, kind);

            return true;
        }

        public static bool AddMessage(string[] messages, HudMessage.MessageKind kind)
        {
            if (Instance == null)
                return false;

            foreach (var message in messages)
            {
                Instance._message.AddMessage(message, kind);
            }
            return true;
        }
    }
}
