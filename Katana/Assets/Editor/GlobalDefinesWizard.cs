using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

//http://d.hatena.ne.jp/nakamura001/20121115/1353002126

public class GlobalDefinesWizard : ScriptableWizard
{
    [System.Serializable]
    public class GlobalDefine : ISerializable
    {
        public string define;
        public bool enabled;


        public GlobalDefine()
        { }


        protected GlobalDefine(SerializationInfo info, StreamingContext context)
        {
            define = info.GetString("define");
            enabled = info.GetBoolean("enabled");
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("define", define);
            info.AddValue("enabled", enabled);
        }

    }

    private const string SettingFile = "/Editor/GlobalDefinesSetting.bytes";
    //private const string _prefsKey = "kGlobalDefines";
    public List<GlobalDefine> _globalDefines = new List<GlobalDefine>();


    [MenuItem("Tools/Edit Global Defines")]
    static void CreateWizardFromMenu()
    {
        var helper = ScriptableWizard.DisplayWizard<GlobalDefinesWizard>("Global Defines Manager", "SaveAndReBuild", "Cancel");
        helper.minSize = new Vector2(500, 300);
        helper.maxSize = new Vector2(500, 300);


        string settingFile = Application.dataPath + SettingFile;
        

        if (File.Exists(settingFile))
        {

            StreamReader reader = new StreamReader(settingFile);
            string settingData = reader.ReadToEnd();
            reader.Close();

            string[] sepDefines = { ";" };
            string[] sepDefine = { ":" };
            string[] defines = settingData.Split(sepDefines, System.StringSplitOptions.None);

            for (int i = 0; i < defines.Length; ++i)
            {
                string[] def = defines[i].Split(sepDefine, System.StringSplitOptions.None);

                GlobalDefine d = new GlobalDefine();
                d.define = def[0];
                d.enabled = ((string)def[1]).ToUpper() == "TRUE";

                helper._globalDefines.Add(d);
            }
        }

       
    }


    void OnGUI()
    {
        var toRemove = new List<GlobalDefine>();

        foreach (var define in _globalDefines)
        {
            if (DefineEditor(define))
                toRemove.Add(define);
        }

        foreach (var define in toRemove)
            _globalDefines.Remove(define);

        if (GUILayout.Button("Add Define"))
        {
            var d = new GlobalDefine();
            d.define = "NEW_DEFINE";
            d.enabled = false;
            _globalDefines.Add(d);
        }
        GUILayout.Space(40);

        if (GUILayout.Button("Save and Rebuild"))
        {
            SaveAndReBuild();
            Close();
        }
    }


    private void SaveAndReBuild()
    {
        // nothing to save means delete everything
        if (_globalDefines.Count == 0)
        {
            DeleteFiles();
            return;
        }

        // 外部ファイルへ書き出し
        string settingPath = Application.dataPath + SettingFile;
        string writeSettingData = "";
        StreamWriter msWriter = new StreamWriter(settingPath);
        for (int i = 0; i < _globalDefines.Count; ++i)
        {
            writeSettingData += _globalDefines[i].define + ":";

            if (_globalDefines[i].enabled)
            {
                writeSettingData += "TRUE;";
            }
            else
            {
                writeSettingData += "FALSE;";
            }
        }
        writeSettingData = writeSettingData.Remove((writeSettingData.Length - 1));
        msWriter.Write(writeSettingData);
        msWriter.Close();

        // *.rspファイルへ書き出し
        // 有効なマクロを取り出し
        var toDisk = _globalDefines.Where(d => d.enabled).Select(d => d.define).ToArray();
        if (toDisk.Length > 0)
        {
            var builder = new System.Text.StringBuilder("-define:");
            for (var i = 0; i < toDisk.Length; i++)
            {
                if (i < toDisk.Length - 1)
                    builder.AppendFormat("{0};", toDisk[i]);
                else
                    builder.Append(toDisk[i]);
            }

            WriteFiles(builder.ToString());

        }
        else
        {
            // 有効なマクロがなければ削除
            DeleteFiles();
        }

        // 再ビルドを行う
        ReimportSomethingToForceRecompile();
    }


    private void ReimportSomethingToForceRecompile()
    {
        var dataPathDir = new DirectoryInfo(Application.dataPath);
        var dataPathUri = new System.Uri(Application.dataPath);
        // このスクリプトを再インポート
        foreach (var file in dataPathDir.GetFiles("GlobalDefinesWizard.cs", SearchOption.AllDirectories))
        {
            var relativeUri = dataPathUri.MakeRelativeUri(new System.Uri(file.FullName));
            var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
        }

        // Scriptsフォルダ以下のファイルを再ビルド
        dataPathDir = new DirectoryInfo(Application.dataPath + "/Scripts");
        dataPathUri = new System.Uri(Application.dataPath);
        // Scripteフォルダ以下の*csファイルを再インポート
        foreach (var file in dataPathDir.GetFiles("*.cs", SearchOption.AllDirectories))
        {
            var relativeUri = dataPathUri.MakeRelativeUri(new System.Uri(file.FullName));
            var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate );
        }
    }


    private void DeleteFiles()
    {
        var smcsFile = Path.Combine(Application.dataPath, "smcs.rsp");
        var gmcsFile = Path.Combine(Application.dataPath, "gmcs.rsp");

        if (File.Exists(smcsFile))
            File.Delete(smcsFile);

        if (File.Exists(gmcsFile))
            File.Delete(gmcsFile);
    }


    private void WriteFiles(string data)
    {
        var smcsFile = Path.Combine(Application.dataPath, "smcs.rsp");
        var gmcsFile = Path.Combine(Application.dataPath, "gmcs.rsp");

        // -define:debug;poop
        File.WriteAllText(smcsFile, data);
        File.WriteAllText(gmcsFile, data);
    }


    private bool DefineEditor(GlobalDefine define)
    {
        EditorGUILayout.BeginHorizontal();

        define.define = EditorGUILayout.TextField(define.define);
        define.enabled = EditorGUILayout.Toggle(define.enabled);

        bool remove = GUILayout.Button("Remove");

        EditorGUILayout.EndHorizontal();

        return remove;
    }


    // Called when the 'save' button is pressed
    void OnWizardCreate()
    {
        // .Net 2.0 Subset: smcs.rsp
        // .Net 2.0: gmcs.rsp
        // -define:debug;poop
    }


    void OnWizardOtherButton()
    {
        this.Close();
    }

}