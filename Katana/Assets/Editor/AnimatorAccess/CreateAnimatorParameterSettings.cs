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

        return string.Format("{0}/{1}Access.cs", directry, StripSpace(fileName));
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
        string prefix = intent + "protected const int {0}Hash = {1};";

        Func<string, string, string> makePropertyTemplate = (typeName, ltype) =>
        {
            string getter = intent + "public " + typeName + " Get{0}(){{ return _animator.Get" + ltype +"({0}Hash); }}";
            string setter = intent + "public " + "void" + " Set{0}(" + typeName + " value){{ _animator.Set" + ltype + "({0}Hash, value);}}";
            
            return getter + "\n" + setter;
        };

        var floatPropertyTemplate = makePropertyTemplate("float", "Float");
        var intPropertyTemplate   = makePropertyTemplate("int", "Integer");
        var boolPropertyTemplate  = makePropertyTemplate("bool", "Bool");
        
        var triggerTemplate = intent + "public void Trigger{0}(){{ _animator.SetTrigger ({0}Hash); }} public void Reset{0}() {{ _animator.ResetTrigger ({0}Hash); }}";

        

        var codePath = GetPath("Assets/Editor/AnimatorAccessHelper/AnimatorParameterImporter.txt");
        var codeTemplate = File.ReadAllText(codePath);
        Assert.IsNotNull(codeTemplate);

        StringBuilder fields = new StringBuilder();

        fields.AppendLine("// Perameters");

        // ハッシュ値の定義
        StringBuilder hashBulider = new StringBuilder(intent + "// hash values\n");


        StringBuilder getsetBuilder = new StringBuilder(intent + "// parameter setter getter \n");
        // 
        // パラメータを定数化
        foreach (var param in animatorController.parameters)
        {
            string code = string.Empty;
            string name = StripSpace(param.name);

            // ハッシュ値の定義
            hashBulider.AppendLine(string.Format(prefix, name, param.nameHash));

            if (param.type == AnimatorControllerParameterType.Bool)
                code = string.Format(boolPropertyTemplate, name);
            else if (param.type == AnimatorControllerParameterType.Float)
                code = string.Format(floatPropertyTemplate, name);
            else if (param.type == AnimatorControllerParameterType.Int)
                code = string.Format(intPropertyTemplate, name);
            else if (param.type == AnimatorControllerParameterType.Trigger)
                code = string.Format(triggerTemplate, name);

            // getter setter
            getsetBuilder.AppendLine(code);
        }
        fields.Append(hashBulider.ToString() + "\n");
        fields.Append(getsetBuilder.ToString() + "\n");

        Dictionary<string, int> hashState = new Dictionary<string, int>();
       
        foreach (var layer in animatorController.layers)
        {
            StateCheck(layer.stateMachine, layer.stateMachine.name + ".", ref hashState);
        }

        // 状態の取得関数を定義
        fields.AppendLine(intent + "// State");

        string stateTemplate = intent + "public const int {0} = {1};";
        string isStateTemplate = intent +
                                 "public bool Is{0}State(){{ return {1} == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }}";

        StringBuilder onStateEnter = new StringBuilder("// case \n");
        StringBuilder onStateExit = new StringBuilder("// case \n");
        StringBuilder onStateMove = new StringBuilder("// case \n");
        StringBuilder onStateUpdate = new StringBuilder("// case \n");
        StringBuilder onStateIK = new StringBuilder("// case \n");

        hashBulider = new StringBuilder();
        var getterBuilder = new StringBuilder();
        var functionBuilder = new StringBuilder();
        foreach (var states in hashState)
        {
            // 先頭のBaseLayerの部分を削除
            var stateName = Regex.Replace(states.Key, "^BaseLayer_", "");

            var stateId = "StateId" + stateName;
            // ハッシュ値の宣言の行
            hashBulider.AppendLine(string.Format(stateTemplate, stateId, states.Value));

            // 状態取得関数の行
            getterBuilder.AppendLine(string.Format(isStateTemplate, stateName, stateId));

            var functionTemplate = intent +  "public virtual void {0}(Animator animator, AnimatorStateInfo stateInfo){{}}";
            var caseTemplate = intent + intent + "case {0} : {1}; break;";
            var functionExecTemplate = "{0}(animator, stateInfo)";

            var stateEnter = "OnStateEnterTo" + stateName;//string.Format(functionTemplate, );
            onStateEnter.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateEnter)));

            var stateExit = "OnStateExitFrom" + stateName;
            onStateExit.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateExit)));

            var stateMove ="OnStateMove" + stateName;
            onStateMove.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateMove)));

            var stateUpdate = "OnStateUpdate" + stateName;
            onStateUpdate.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateUpdate)));

            var stateIk = "OnStateIk" + stateName;
            onStateIK.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateIk)));

            functionBuilder.AppendLine(string.Format(functionTemplate, stateEnter));
            functionBuilder.AppendLine(string.Format(functionTemplate, stateExit));


            // http://unitysiki.blogspot.jp/2014/08/onanimatormoveapply-root-motionunity.html
            // OnStateMoveを定義してしまうとRootMotionが適用されなくなるようなので利用しない
            functionBuilder.AppendLine("//" + string.Format(functionTemplate, stateMove));

            functionBuilder.AppendLine(string.Format(functionTemplate, stateUpdate));
            functionBuilder.AppendLine(string.Format(functionTemplate, stateIk));
        }

        fields.AppendLine(hashBulider.ToString());
        fields.AppendLine(getterBuilder.ToString());
        fields.AppendLine(functionBuilder.ToString());
        return string.Format(codeTemplate, StripSpace(animatorController.name), fields.ToString(), onStateEnter, onStateExit, string.Empty, string.Empty,
            onStateMove, onStateUpdate, onStateIK);
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
