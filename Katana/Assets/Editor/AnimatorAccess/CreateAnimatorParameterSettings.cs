/**
 * CreateAnimatorParameterSettings
 * 
 * Copyright (c) 2015 Tatsuhiko Yamamura
 * This software is released under the MIT License.
 * http://opensource.org/licenses/mit-license.php
 */
#region using
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using UnityEngine.Assertions;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;


#endregion

public class CreateAnimatorParameterSettings : AssetPostprocessor
{
    // http://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPostprocessAllAssets.html
    // アセットがインポートしたときに呼ばれる.
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // 読み込まれたアセット
        foreach (var str in importedAssets)
        {
            // .controllerがアセットされたか
            if (Path.GetExtension(str) != ".controller") { continue; }

            // アニメータからコードを生成する
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(str);
            var code = GenerateCode(controller);

            // ファイル名は同じ
            var fileName = GetCodeFileName(str);
            File.WriteAllText(fileName, code);
        }

        // 削除されたアセット
        foreach (var str in deletedAssets)
        {
            if (Path.GetExtension(str) != ".controller") { continue; }
            var path = GetPath(GetCodeFileName(str));
            AssetDatabase.DeleteAsset(path);
        }

        // 移動されたアセット
        for (var i = 0; i < movedAssets.Length; i++)
        {
            var fromPath = movedFromAssetPaths[i];
            var toPath = movedAssets[i];
            if (Path.GetExtension(fromPath) != ".controller") { continue; }
            var path = GetPath(GetCodeFileName(fromPath));
            AssetDatabase.MoveAsset(path, GetCodeFileName(toPath));
        }

        AssetDatabase.Refresh();
    }

    static string GetCodeFileName(string path)
    {

        var directry = Directory.GetParent(path);
        var fileName = Path.GetFileNameWithoutExtension(path);

        return string.Format("{0}/{1}.cs", directry, StripSpace(fileName));
    }

    static string GetPath(string path)
    {
        if (File.Exists(path))
        {
            return path;
        }

        var fileName = Path.GetFileName(path);
        var files = Directory.GetFiles("Assets", fileName, SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            return null;
        }
        return files[0];
    }

    static string StripSpace(string name)
    {

        return Regex.Replace( Regex.Replace(name, " ", ""), "\\W", "_");
    }

    static string GenerateCode(AnimatorController animatorController)
    {
        string intent = "\t\t";
        string prefix = intent + "protected readonly static int {0}Hash = {1};";

        Func<string, string> makePropertyTemplate = (typeName) =>
        {
            string ltype = typeName.Substring(0, 1).ToUpper() + typeName.Substring(1);
            string getter = intent + "public " + typeName + " Get{0}(){{ return _animator.Get" + ltype + "({0}Hash); }}";
            string setter = intent + "public " + "void" + " Set{0}(" + typeName + " value){{ _animator.Set" + ltype + "({0}Hash, value);}}";
            return prefix + "\n" + getter + "\n" + setter;
            //  return prefix + "\n" + intent + "public " + typeName + " {0}{{ get{{ return _animator.Get" + Ltype + "({0}Hash); }} set{{ _animator.Set" + Ltype + "({0}Hash, value); }}}}";
        };

        string floatPropertyTemplate = makePropertyTemplate("float");
        string intPropertyTemplate   = makePropertyTemplate("int");
        string boolPropertyTemplate  = makePropertyTemplate("bool");
        
        string triggerTemplate = prefix + "\n" + intent + "public void {0}(){{ _animator.SetTrigger ({0}Hash); }} public void Reset{0}() {{ _animator.ResetTrigger ({0}Hash); }}";

        

        var codePath = GetPath("Assets/Editor/AnimatorAccessHelper/AnimatorParameterImporter.txt");
        var codeTemplate = File.ReadAllText(codePath);
        Assert.IsNotNull(codeTemplate);

        StringBuilder fields = new StringBuilder();

        fields.AppendLine("// Perameters");
        // パラメータを定数化
        foreach (var param in animatorController.parameters)
        {
            string code = string.Empty;
            string name = StripSpace(param.name);
            if (param.type == AnimatorControllerParameterType.Bool)
                code = string.Format(boolPropertyTemplate, name, param.nameHash);
            if (param.type == AnimatorControllerParameterType.Float)
                code = string.Format(floatPropertyTemplate, name, param.nameHash);
            if (param.type == AnimatorControllerParameterType.Int)
                code = string.Format(intPropertyTemplate, name, param.nameHash);
            if (param.type == AnimatorControllerParameterType.Trigger)
                code = string.Format(triggerTemplate, name, param.nameHash);
            fields.AppendLine(code + "\n");
        }

        Dictionary<string, int> hashState = new Dictionary<string, int>();
       
        foreach (var layer in animatorController.layers)
        {
            StateCheck(layer.stateMachine, layer.stateMachine.name + ".", ref hashState);
        }

        fields.AppendLine("// State");

        string stateTemplate = intent + "public static readonly int {0} = {1};";
        string isStateTemplate = intent +
                                 "public bool Is{0}(){{ return {1} == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }}";
        foreach (var states in hashState)
        {
            // 先頭のBaseLayerの部分を削除
            var removed = Regex.Replace(states.Key, "^BaseLayer_", "");

            var stateId = "StateId" + removed;
            // ハッシュ値の宣言の行
            fields.AppendLine(string.Format(stateTemplate, stateId, states.Value));

            // 状態取得関数の行
            fields.AppendLine(string.Format(isStateTemplate, removed, stateId));
        }

        return string.Format(codeTemplate, StripSpace(animatorController.name), fields.ToString());
    }

    static void StateCheck(AnimatorStateMachine statemachiene, string stateNameSpace, ref Dictionary<string, int> hashState)
    {
        foreach (var child in statemachiene.stateMachines)
        {
            StateCheck(child.stateMachine, stateNameSpace + child.stateMachine.name + ".", ref hashState);
        }
        foreach (var state in statemachiene.states)
        {
            var name = stateNameSpace + state.state.name;
            var hash = Animator.StringToHash(name);
            var stripedName = StripSpace(name);
            if (hashState.ContainsKey(stripedName) == false)
            {
                hashState.Add(stripedName, hash);
            }
        }
    }

}
