using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateSoundAsset : MonoBehaviour {

    void Start()
    {
        //var soundList = new SoundList();
        //AssetDatabase.CreateAsset(soundList, "Assets/Resources/Data/SoundList.asset");
        CreateAsset<SoundList>();
    }

    public static void CreateAsset<Type>() where Type : ScriptableObject
    {
        Type item = ScriptableObject.CreateInstance<Type>();

        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Data/" + typeof(Type) + ".asset");

        AssetDatabase.CreateAsset(item, path);
        AssetDatabase.SaveAssets();
    }
}
