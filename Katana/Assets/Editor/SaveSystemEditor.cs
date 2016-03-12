using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// セーブデータ周りを操作するスクリプト
public static class SaveSystemEditor
{
    private const string CommandPath = "Tools/SaveCommand/";
    private const string DeleteCommand = CommandPath + "DeleteSaveData";
   
    /// セーブデータを削除
    [MenuItem(DeleteCommand)]
    public static void DeleteSaveData()
    {
        SerializeHelper.DeleteFile(SaveData.SavePath);
    }

    
}