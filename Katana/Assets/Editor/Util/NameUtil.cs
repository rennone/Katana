using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Katana
{
    // 名前に関する汎用関数群
    public class NameUtil
    {
        // ファイル名に使えない文字
        private static readonly string[] InvalidFileChars = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

        // 変数名に使えない文字
        private static readonly string[] InvalidVariableChars =
            {
                " ", "!", "\"", "#", "$",
                "%", "&", "\'", "(", ")",
                "-", "=", "^",  "~", "\\",
                "|", "[", "{",  "@", "`",
                "]", "}", ":",  "*", ";",
                "+", "/", "?",  ".", ">",
                ",", "<"
            };

        // ファイル名に使えない文字を取り除く
        public static string RemoveFileInvalidChars(string str)
        {
            return ReplaceFileInvalidChars(str, string.Empty);
        }

        // ファイル名に使えない文字をreplacedに入れ替える
        public static string ReplaceFileInvalidChars(string str, string replaced)
        {
            return ReplaceChars(str, InvalidFileChars, replaced);
        }

        // 変数名に使えない文字をrepalcedに入れ替える
        public static string ReplaceVariableInvalidChars(string str, string replaced)
        {
            return ReplaceChars(str, InvalidVariableChars, replaced);
        }

        // 変数名に使えない文字を取り除く
        public static string RemoveVariableInvalidChars(string str)
        {
            return ReplaceVariableInvalidChars(str, string.Empty);
        }

        // strの中の
        // replaceListの文字をrepalcedに入れ替える
        static string ReplaceChars(string str, string[] replaceList, string replaced)
        {
            Array.ForEach(replaceList, c => str = str.Replace(c, replaced));
            return str;
        }
    }

}
