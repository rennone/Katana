using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// 参考
//http://baba-s.hatenablog.com/entry/2014/02/25/000000

// メニューバー -> Tool -> Create -> Tag Name で実行可能
// (Ctrl + Shif + t)
/// <summary>
/// タグ名を定数で管理するクラスを作成するスクリプト
/// </summary>
public static class LayerNameCreator
{
    // 変数名に使えない文字を管理する配列
    private static readonly string[] INVALUD_CHARS =
    {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        ",", "<"
    };

    private const string HotKey = "%#t";    //ショートカットキー Ctrl + Shift + t
    private const string CommandName = "Tools/Create/Tag Name";  // コマンド名
    private const string CommandNameWithHotkey = CommandName + " " + HotKey;
    private const string OutputFilePath = "Assets/Scripts/System/TagName.cs";      // ファイルパス

    private static readonly string OutputFileName = Path.GetFileName(OutputFilePath);                   // ファイル名(拡張子あり)
    private static readonly string OutputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(OutputFilePath);   // ファイル名(拡張子なし)

    /// <summary>
    /// タグ名を定数で管理するクラスを作成します
    /// </summary>
    [MenuItem(CommandNameWithHotkey)]
    public static void Create()
    {
        CreateScript();
        EditorUtility.DisplayDialog(OutputFileName, "作成が完了しました", "OK");
    }

    /// <summary>
    /// スクリプトを作成します
    /// </summary>
    public static void CreateScript()
    {
        var builder = new StringBuilder();

        builder.AppendLine("/// <summary>");
        builder.AppendLine("/// タグ名を定数で管理するクラス");
        builder.AppendLine("/// </summary>");
        builder.AppendFormat("public static class {0}", OutputFileNameWithoutExtension).AppendLine();
        builder.AppendLine("{");

        foreach (var n in InternalEditorUtility.tags.
            Select(c => new { var = RemoveInvalidChars(c), val = c }))
        {
            builder.Append("\t").AppendFormat(@"public const string {0} = ""{1}"";", n.var, n.val).AppendLine();
        }

        builder.AppendLine("}");

        var directoryName = Path.GetDirectoryName(OutputFilePath);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
       
        
        File.WriteAllText(OutputFilePath, builder.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
    }

    /// <summary>
    /// タグ名を定数で管理するクラスを作成できるかどうかを取得します
    /// </summary>
    [MenuItem(CommandNameWithHotkey, true)]
    public static bool CanCreate()
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }

    /// <summary>
    /// 無効な文字を削除します
    /// </summary>
    public static string RemoveInvalidChars(string str)
    {
        Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
        return str;
    }
}