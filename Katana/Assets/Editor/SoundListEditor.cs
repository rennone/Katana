using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class SoundListEditor
{
    private static readonly string SOUND_DEF_PATH = @"Assets/Scripts/AutoGenerate/SoundKey.cs"; //サウンドキー
    private static readonly string SoundListPass = @"Assets/Resources/Data/SoundList.asset";    //サウンドリストのパス
    private static readonly string SoundFolder = @"Assets/Resources/Sounds";    //サウンドデータの格納フォルダ

    [MenuItem("Assets/Custom/Add SoundList")]
    public static void AddSoundList()
    {
        SetSoundList();
    }

    private static void SetSoundList()
    {
        // ファイルが選択されている時.
        if (Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0)
        {
            //サウンドリストを確保
            SoundList soundList = AssetDatabase.LoadAssetAtPath(SoundListPass, typeof(SoundList)) as SoundList;
            // 選択されているファイルの数だけ繰り返す
            foreach (var file in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(file); //パスの文字列を取得
                var fileName = System.IO.Path.GetFileNameWithoutExtension(path);    //ファイル名を取得
                var fileExtension = System.IO.Path.GetExtension(path);  //ファイルの拡張子を取得

                //拡張子が違う場合はリスト追加しない
                if (fileExtension != ".wav")
                {
                    Debug.Log("<color=blue>" + "Not SoundFile : " + fileName + "</color>");
                    continue;
                }

                bool isUpdated = false;
                //SoundListを全検索
                for(int i=0;i < soundList.SoundDatas.Count;i++)
                {
                    //すでにリストにあったものを更新
                    if (soundList.SoundDatas[i].SoundDataKey == fileName)
                    {
                        soundList.SoundDatas[i].Sound = AssetDatabase.LoadAssetAtPath(path,typeof(AudioClip)) as AudioClip;
                        isUpdated = true;
                        Debug.Log("<color=yellow>" + "SoundList Update : " + fileName + "</color>");
                        break;
                    }
                }

                //リストになかったので追加
                if (!isUpdated)
                {
                    SoundData dataSet = new SoundData();
                    dataSet.SoundDataKey = fileName;
                    dataSet.Type = SoundType.BGM;
                    dataSet.Sound = AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)) as AudioClip;
                    dataSet.SyncCount = 1;
                    dataSet.Volume = 1;
                    dataSet.Distance = 1000;
                    dataSet.RollOff = AudioRolloffMode.Linear;

                    dataSet.DopplerLevel = 0;
                    dataSet.pitch = 1;
                    soundList.SoundDatas.Add(dataSet);
                    Debug.Log("<color=green>" + "SoundList Add : " + fileName + "</color>");
                }
                
            }
            CreateSoundKeyDef(soundList.SoundDatas.Select(c => c.SoundDataKey.ToString()).ToArray());   //サウンドキーを更新

            AssetDatabase.SaveAssets();
        }
    }

    //定義ファイルをリフレッシュする
    static public void CreateSoundKeyDef(string[] soundNames)
    {
        string sounds = string.Join(",\n", soundNames.Select(c1 => c1.ToUpper()).ToArray());
        string template = "";
        template += "public enum SoundKey{\n";
        template += sounds;
        template += "\n}";

        System.IO.StreamWriter sw = new System.IO.StreamWriter(
            SOUND_DEF_PATH,
            false,
            System.Text.Encoding.GetEncoding("utf-8"));
        sw.Write(template);
        sw.Close();
        AssetDatabase.Refresh();
    }
}