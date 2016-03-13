using UnityEngine;
using System.Collections;

namespace Katana {
    public class SaveManager :Singleton<SaveManager>
    {
        //デリゲートでセーブイベントを作成（何かセーブを行いたいものは、このSaveEventに+=で追加を行ってね）
        public delegate void Save();
        public static event Save SaveEvent;

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
            SaveEvent();    //セーブイベントを発火
            SaveData.Save();
        }
    }
}

