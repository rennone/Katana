using UnityEngine;
using System.Collections;

public class SaveTest : MonoBehaviour
{
    void Start()
    {
        if (SaveData.Load())
        {
            Debug.Log(SaveData.GameSaveData.SaveVersion);
        }
        GameSaveField saveFile = new GameSaveField();
        Character chara = new Character();
        chara.name = "Piyo";
        chara.position = Vector3.one;
        saveFile.MainChara = chara;
        SaveData.GameSaveData = saveFile;
        SaveData.Save();
        Debug.Log(SaveData.GameSaveData.MainChara.name);
    }
}

