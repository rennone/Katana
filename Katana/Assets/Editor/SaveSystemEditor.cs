using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// セーブデータ周りを操作するスクリプト
public static class SaveSystemEditor
{
    private const string CommandPath = "Tools/SaveCommand/";
    private const string DeleteCommand = CommandPath + "DeleteSaveData";
    private const string EncryptCommand = CommandPath + "Encrypt On-Off";

    private const string EncryptDeffineSymbol = "EncryptSaveData";

    /// セーブデータを削除
    [MenuItem(DeleteCommand)]
    public static void DeleteSaveData()
    {
        SerializeHelper.DeleteFile(SaveData.SavePath);
    }

    /// セーブデータの暗号化処理のOn/Off切り替え
    [MenuItem(EncryptCommand)]
    public static void EncryptChange()
    {
        var defineSymbols = PlayerSettings
            .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
            .Split(';').ToList();

        if(defineSymbols.Any(c => c == EncryptDeffineSymbol))
        {
            //暗号化しない設定に
            defineSymbols.Remove(EncryptDeffineSymbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", defineSymbols.ToArray())
            );
            Debug.Log("<color=green>" + "EncryptOn"+"</color>");
        }
        else
        {
            //暗号化する設定に
            defineSymbols.Add(EncryptDeffineSymbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", defineSymbols.ToArray())
            );
            Debug.Log("<color=green>" + "EncryptOff" + "</color>");
        }
    }
}