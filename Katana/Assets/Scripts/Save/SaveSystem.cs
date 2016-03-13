/*
セーブの仕組み
セーブデータは「C:\Users\"ユーザー名"\AppData\Local/"会社名"/"プロダクト名"」の中に作成される
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class GameSaveField
{
    //セーブデータのバージョン（このCSファイルの最終更新日時）
    public string SaveVersion = "Null";

    //メインキャラクターに関する情報
    public SaveDataCharacter MainChara = new SaveDataCharacter(); 
}

[System.Serializable]
public class SaveDataCharacter
{
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
}

public static class SaveData
{
    private static readonly string ThisFilePath = "Assets/Scripts/Save/SaveSystem.cs";

    public static readonly string SavePath = "Save";

    //デリゲートでセーブイベントを作成（何かセーブを行いたいものは、このSaveEventに+=で追加を行ってね）
    public delegate void SaveDelegate();
    public static event SaveDelegate SaveEvent;

    private static GameSaveField m_SaveData = new GameSaveField();
    public static GameSaveField GameSaveData
    {
        get { return m_SaveData; }
        set { m_SaveData = value; }
    }

    static public void Save()
    {
#if UNITY_EDITOR
        //このスクリプトの最終更新日時を記録
        DateTime dtUpdate = System.IO.File.GetLastWriteTime(ThisFilePath);
        m_SaveData.SaveVersion = dtUpdate.ToString();
#endif
        SerializeHelper.Serialize<GameSaveField>(SavePath, m_SaveData);
    }

    static public bool Load()
    {
        GameSaveField save = SerializeHelper.Deserialize<GameSaveField>(SavePath);
        if (save != null)
        {
#if UNITY_EDITOR
            //このファイルの最終更新日時をセーブ時のものと比較
            //違いがあれば、それはおそらく情報の追加や削除が行われてるということなので、
            //セーブデータを一旦削除
            DateTime dtUpdate = System.IO.File.GetLastWriteTime(ThisFilePath);
            if (dtUpdate.ToString() != save.SaveVersion)
            {
                Debug.Log("セーブバージョンに相違があるので、セーブデータを削除しました。");
                SerializeHelper.DeleteFile(SavePath);
                return false;
            }
#endif
            m_SaveData = save;
            return true;
        }
        else
        {
            return false;
        }
    }
    
    //全セーブデータの呼び出し
    public static bool LoadAll()
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

    //全セーブ
    public static void SaveAll()
    {
        SaveEvent();    //セーブイベントを発火
        SaveData.Save();
    }
}

//シリアライズとデシリアライズ関係の処理をまとめたクラス
public static class SerializeHelper
{
    private static string GetFilePath(string fileName)
    {

        string directoryPath = Application.persistentDataPath;

        //ディレクトリが無ければ作成
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = directoryPath + "/" + fileName;

        return filePath;
    }

    //セーブデータを削除
    public static void DeleteFile(string fileName)
    {
        string directoryPath = Application.persistentDataPath;

        string filePath = directoryPath + "/" + fileName;

        File.Delete(filePath);
    }

    public static void Serialize<T>(string prefKey, T serializableObject)
    {
        string json = JsonUtility.ToJson(serializableObject);   //シリアライズ化
        byte[] jsonArray = Encoding.UTF8.GetBytes(json);
#if EncryptSaveData
        AesCryptography aesCryptography = new AesCryptography();
        jsonArray = aesCryptography.Encrypt(jsonArray); //暗号化
#endif
        File.WriteAllBytes(GetFilePath(prefKey), jsonArray);
    }

    public static T Deserialize<T>(string prefKey)
    {
        string filePath = GetFilePath(prefKey);
        if (!File.Exists(filePath))
        {
            //セーブデータ無し
            return default(T);
        }

        byte[] serializedArray = File.ReadAllBytes(filePath);
#if EncryptSaveData
        AesCryptography aesCryptography = new AesCryptography();
        serializedArray = aesCryptography.Decrypt(serializedArray);  //複合化
#endif
        string data = Encoding.UTF8.GetString(serializedArray);

        T deserializedObject = JsonUtility.FromJson<T>(data);   //デシリアライズ化
        return deserializedObject;
    }
}

//暗号化関係の処理(AES方式を採用)
public class AesCryptography
{
    // 256bit(32byte)のInitVector（初期ベクタ）とKey（暗号キー）
    private const string AesInitVector = @"pdk23laozi9jngiqnvl3ial1ocmj98ai";
    private const string AesKey = @"nsi4ia91kajos8eunzi35ia1okmlauzi";

    private const int BlockSize = 256;
    private const int KeySize = 256;


    /// <summary>
    /// 暗号化スクリプト
    /// </summary>
    /// <returns>byte[] 暗号化したbyte列</returns>
    public byte[] Encrypt(byte[] binData)
    {
        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Padding = PaddingMode.Zeros;
        myRijndael.Mode = CipherMode.CBC;
        myRijndael.KeySize = KeySize;
        myRijndael.BlockSize = BlockSize;

        byte[] key = new byte[0];
        byte[] InitVector = new byte[0];

        key = System.Text.Encoding.UTF8.GetBytes(AesKey);
        InitVector = System.Text.Encoding.UTF8.GetBytes(AesInitVector);

        ICryptoTransform encryptor = myRijndael.CreateEncryptor(key, InitVector);

        MemoryStream msEncrypt = new MemoryStream();
        CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

        byte[] src = binData;

        // 暗号化する
        csEncrypt.Write(src, 0, src.Length);
        csEncrypt.FlushFinalBlock();

        byte[] dest = msEncrypt.ToArray();

        return dest;
    }



    /// <summary>
    /// 複合化スクリプト
    /// </summary>
    /// <returns>byte[] 複合化したbyte列</returns>
    public byte[] Decrypt(byte[] binData)
    {

        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Padding = PaddingMode.Zeros;
        myRijndael.Mode = CipherMode.CBC;
        myRijndael.KeySize = KeySize;
        myRijndael.BlockSize = BlockSize;

        byte[] key = new byte[0];
        byte[] InitVector = new byte[0];

        key = System.Text.Encoding.UTF8.GetBytes(AesKey);
        InitVector = System.Text.Encoding.UTF8.GetBytes(AesInitVector);

        ICryptoTransform decryptor = myRijndael.CreateDecryptor(key, InitVector);
        byte[] src = binData;
        byte[] dest = new byte[src.Length];

        MemoryStream msDecrypt = new MemoryStream(src);
        CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

        // 複号化する
        csDecrypt.Read(dest, 0, dest.Length);

        return dest;
    }
}