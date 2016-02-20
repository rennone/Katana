using System;
using System.IO;
using System.Linq;
using System.Text;
using Katana;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

// 参考
//http://baba-s.hatenablog.com/entry/2014/02/25/000000

/// タグ名,レイア名を定数で管理するクラスを作成するスクリプト
public static class NameCreator
{
    //[PostProcessScene]
    public static void OnPostProcessScene()
    {
        Debug.Log("OnPostProcessScene");
        CreateLayerName();
        CreateTagName();
    }

    private const string CommandPath = "Tools/Create/";
    private const string TagNameCommand = CommandPath + "Tag Name" + " %#t";
    private const string LayerNameCommand = CommandPath + "Layer Name" + " %#l"; 

    /// タグ名を定数で管理するクラスを作成します
    [MenuItem(LayerNameCommand)]
    public static void CreateLayerName()
    {
        if(CanCreate() == false)
            return;

        var layerFilePath = "Assets/Scripts/System/LayerName.cs";
        CreateLayerNameFile(layerFilePath);

        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        EditorUtility.DisplayDialog(layerFilePath, "作成が完了しました", "OK");
    }

    [MenuItem(TagNameCommand)]
    public static void CreateTagName()
    {
        if (CanCreate() == false)
            return;

        var tagFilePath = "Assets/Scripts/System/TagName.cs";
        CreateTagNameFile(tagFilePath);

        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        EditorUtility.DisplayDialog(tagFilePath, "作成が完了しました", "OK");
    }

    
    [MenuItem(TagNameCommand, true)]
    public static bool CanCreateTagName()
    {
        return CanCreate();
    }

    [MenuItem(LayerNameCommand, true)]
    public static bool CanCreateLayerName()
    {
        return CanCreate();
    }

    //! タグ名のクラスファイルの作成
    static void CreateTagNameFile(string outputFilePath)
    {
        var builder = new StringBuilder();
        AddClassHeader(builder, outputFilePath, "タグ名を定数で管理するクラス");
        foreach (var n in InternalEditorUtility.tags.
            Select(c => new { var = NameUtil.RemoveVariableInvalidChars(c), val = c }))
        {
            builder.Append("\t").AppendFormat(@"public const string {0} = ""{1}"";", n.var, n.val).AppendLine();
        }

        builder.AppendLine("}");
        Save(outputFilePath, builder);
    }

    // レイヤー名のクラスファイルの作成
    static void CreateLayerNameFile(string outputFilePath)
    {
        var builder = new StringBuilder();
        AddClassHeader(builder, outputFilePath, "レイヤ番号を定数で管理するクラス");
        foreach (var n in InternalEditorUtility.layers.
            Select(c => new { var = NameUtil.RemoveVariableInvalidChars(c), val = c }))
        {
            //builder.Append("\t").AppendFormat(@"public static int {0} {{ get{{ return LayerMask.NameToLayer(""{1}""); }} }}", n.var, n.val).AppendLine();
            builder.Append("\t").AppendFormat(@"public const int {0}  = {1};",  n.var, LayerMask.NameToLayer(n.val)).AppendLine();
        }
        builder.AppendLine("}");
        Save(outputFilePath, builder);
    }

    // filePath : 書き出しファイル
    // builder  : 書き出す文字列
    static void Save(string filePath, StringBuilder builder)
    {
        var directoryName = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
    }

    // クラスの宣言部分の作成
    static StringBuilder AddClassHeader(StringBuilder builder, string fileName, string comment)
    {
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("//" + comment);
        builder.AppendFormat("public static class {0}", Path.GetFileNameWithoutExtension(fileName)).AppendLine();
        builder.AppendLine("{");

        return builder;
    }

    /// タグ名を定数で管理するクラスを作成できるかどうかを取得します
    public static bool CanCreate()
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }

}