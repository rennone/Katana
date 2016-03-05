using UnityEngine;
using System.Collections;

namespace Katana {
    public class SaveManager :Singleton<SaveManager>
    {
        public GameSaveField GameSaveData
        {
            get { return SaveData.GameSaveData; }
        }

        public bool LoadAll()
        {
            if (SaveData.Load())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveAll()
        {
            SaveMainChara();
        }

        public void SaveMainChara()
        {
            S_Character chara = new S_Character();
            Player player = GameManager.Instance.Player;
            chara.position = player.transform.position;
            chara.rotation = player.transform.rotation;
            SaveData.GameSaveData.MainChara = chara;
            SaveData.Save();
        }
    }
}

