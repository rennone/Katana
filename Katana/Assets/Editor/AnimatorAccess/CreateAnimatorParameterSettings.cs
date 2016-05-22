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
using UniRx.Operators;

#endregion

public class CreateAnimatorParameterSettings : AssetPostprocessor
{
    static string GetContollerCodeFileName(string path)
    {
        var directry = Directory.GetParent(path);
        var fileName = Path.GetFileNameWithoutExtension(path);

        return string.Format("{0}/{1}Controller.cs", directry, StripSpace(fileName));
    }

    static string GetCodeFileName(string path)
    {
        var directry = Directory.GetParent(path);
        var fileName = Path.GetFileNameWithoutExtension(path);

        return string.Format("{0}/{1}StateMachine.cs", directry, StripSpace(fileName));
    }

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
            GenerateCode(controller, str);
            GenerateAccessorCode(controller, GetContollerCodeFileName(str));
        }

        // 削除されたアセット
        foreach (var str in deletedAssets)
        {
            if (Path.GetExtension(str) != ".controller") { continue; }
            var path = GetPath(GetCodeFileName(str));
            AssetDatabase.DeleteAsset(path);

            AssetDatabase.DeleteAsset(GetPath(GetContollerCodeFileName(str)));
        }

        // 移動されたアセット
        for (var i = 0; i < movedAssets.Length; i++)
        {
            var fromPath = movedFromAssetPaths[i];
            var toPath = movedAssets[i];
            if (Path.GetExtension(fromPath) != ".controller") { continue; }
            var path = GetPath(GetCodeFileName(fromPath));
            AssetDatabase.MoveAsset(path, GetCodeFileName(toPath));

            path = GetPath(GetContollerCodeFileName(fromPath));
            AssetDatabase.MoveAsset(path, GetContollerCodeFileName(toPath));
        }

        AssetDatabase.Refresh();
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

    static string ControllerClassName(AnimatorController controller) { return StripSpace(controller.name) + "Controller"; }
    static string StateMachineClassName(AnimatorController controller){return StripSpace(controller.name) + "StateMachine";}

    static Dictionary<string, int> HashState(AnimatorController controller)
    {
        Dictionary<string, int> hashState = new Dictionary<string, int>();

        foreach (var layer in controller.layers)
        {
            StateCheck(layer.stateMachine, layer.stateMachine.name + ".", ref hashState);
        }

        return hashState;
    }

    const string StateId = "StateId";
    const string StateEnter = "OnStateEnterTo";
    const string StateExit = "OnStateExitFrom";
    const string StateMove = "OnStateMove";
    const string StateUpdate = "OnStateUpdate";
    const string StateIk = "OnStateIk";

    static void GenerateAccessorCode(AnimatorController animatorController, string outputFileName)
    {
        string intent = "\t\t";
        string prefix = intent + "protected const int {0}Hash = {1};";

        // 各Animator変数へのプロパティ
        Func<string, string, string> makePropertyTemplate = (typeName, ltype) =>
        {
            string getter = intent + "public " + typeName + " Get{0}(){{ return _Animator.Get" + ltype + "({0}Hash); }}";
            string setter = intent + "public " + "void" + " Set{0}(" + typeName + " value){{ _Animator.Set" + ltype + "({0}Hash, value);}}";

            return getter + "\n" + setter;
        };

        // float型
        var floatPropertyTemplate = makePropertyTemplate("float", "Float");
        // int型
        var intPropertyTemplate = makePropertyTemplate("int", "Integer");
        // bool型
        var boolPropertyTemplate = makePropertyTemplate("bool", "Bool");
        // trigger型
        var triggerTemplate = intent + "public void Trigger{0}(){{ _Animator.SetTrigger ({0}Hash); }} public void Reset{0}() {{ _Animator.ResetTrigger ({0}Hash); }}";

        var controllerCodePath = GetPath("Assets/Editor/AnimatorAccessHelper/AnimatorControllerTemplate.txt");
        var controllerTemplate = File.ReadAllText(controllerCodePath);
        Assert.IsNotNull(controllerTemplate);

        StringBuilder fields = new StringBuilder();

        {
            fields.AppendLine("// Perameters");
            // ハッシュ値の定義
            StringBuilder hashParameterBulider = new StringBuilder(intent + "// hash values\n");

            // getter, setterの定義
            StringBuilder getsetBuilder = new StringBuilder(intent + "// parameter setter getter \n");
            // 
            // パラメータを定数化
            foreach (var param in animatorController.parameters)
            {
                string code = string.Empty;
                string name = StripSpace(param.name);

                // ハッシュ値の定義
                hashParameterBulider.AppendLine(string.Format(prefix, name, param.nameHash));

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

            fields.Append(hashParameterBulider.Append("\n"));
            fields.Append(getsetBuilder.Append("\n"));
        }

        // 状態の取得関数を定義
        fields.AppendLine(intent + "// State");

        var getterStateBuilder = new StringBuilder();
        var functionBuilder = new StringBuilder();
        
        foreach (var states in HashState(animatorController))
        {
            // 先頭のBaseLayerの部分を削除
            var stateName = Regex.Replace(states.Key, "^BaseLayer_", "");

            var stateId     = StateMachineClassName(animatorController) + "." + StateId + stateName;
            var stateEnter  = StateEnter + stateName;
            var stateExit   = StateExit + stateName;
            var stateMove   = StateMove + stateName;
            var stateUpdate = StateUpdate + stateName;
            var stateIk     = StateIk + stateName;
            
            // 状態取得関数の行
            string isStateTemplate 
                = intent + "public bool Is{0}State(){{ return {1} == _Animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }}";
            getterStateBuilder.AppendLine(string.Format(isStateTemplate, stateName, stateId));

            // 関数宣言文
            var functionTemplate = intent + "public virtual void {0}(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){{  }}"; ;

            functionBuilder.AppendLine(string.Format(functionTemplate, stateEnter));
            functionBuilder.AppendLine(string.Format(functionTemplate, stateExit));

            // http://unitysiki.blogspot.jp/2014/08/onanimatormoveapply-root-motionunity.html
            // OnStateMoveを定義してしまうとRootMotionが適用されなくなるようなので利用しない
            functionBuilder.AppendLine("//" + string.Format(functionTemplate, stateMove));

            functionBuilder.AppendLine(string.Format(functionTemplate, stateUpdate));
            functionBuilder.AppendLine(string.Format(functionTemplate, stateIk));
        }

        fields.AppendLine(getterStateBuilder.ToString());
        fields.AppendLine(functionBuilder.ToString());

        var controllerClassName = ControllerClassName(animatorController);

        // Controllerコード
        var controllerText = string.Format(controllerTemplate, controllerClassName, StateMachineClassName(animatorController), fields);
        File.WriteAllText(outputFileName, controllerText);
    }


    // コード生成
    static void GenerateCode(AnimatorController animatorController, string outputFileName)
    {
        string intent = "\t\t";

        var codePath = GetPath("Assets/Editor/AnimatorAccessHelper/AnimatorStateMachineTemplate.txt");
        var codeTemplate = File.ReadAllText(codePath);
        Assert.IsNotNull(codeTemplate);

        StringBuilder fields = new StringBuilder();

        // 状態の取得関数を定義

        StringBuilder onStateEnter = new StringBuilder("// case \n");
        StringBuilder onStateExit = new StringBuilder("// case \n");
        StringBuilder onStateMove = new StringBuilder("// case \n");
        StringBuilder onStateUpdate = new StringBuilder("// case \n");
        StringBuilder onStateIk = new StringBuilder("// case \n");

        string stateTemplate = intent + "public const int {0} = {1};";
        var hashStateBulider = new StringBuilder("//State\n");
        
        foreach (var states in HashState(animatorController))
        {
            // 先頭のBaseLayerの部分を削除
            var stateName = Regex.Replace(states.Key, "^BaseLayer_", "");

            var stateId = StateId + stateName;

            // Controller側のコード
            // ハッシュ値の宣言の行
            hashStateBulider.AppendLine(string.Format(stateTemplate, stateId, states.Value));

            // StateMachine側のコード
            // case振り分け文
            var caseTemplate = intent + intent + "case {0} : {1}";

            // 実行文
            var functionExecTemplate = "if( Controller!=null){{ Controller.{0}(animator, stateInfo, layerIndex); }} break;";

            var stateEnter = "OnStateEnterTo" + stateName;//string.Format(functionTemplate, );
            onStateEnter.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateEnter)));

            var stateExit = "OnStateExitFrom" + stateName;
            onStateExit.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateExit)));

            var stateMove ="OnStateMove" + stateName;
            onStateMove.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateMove)));

            var stateUpdate = "OnStateUpdate" + stateName;
            onStateUpdate.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateUpdate)));

            var stateIk = "OnStateIk" + stateName;
            onStateIk.AppendLine(string.Format(caseTemplate, stateId, string.Format(functionExecTemplate, stateIk)));
        }

        fields.AppendLine(hashStateBulider.ToString());

        var controllerClassName = ControllerClassName(animatorController);
        var codeText = string.Format(codeTemplate, StateMachineClassName(animatorController), controllerClassName, fields, onStateEnter, onStateExit, string.Empty, string.Empty,
            onStateMove, onStateUpdate, onStateIk);

        // コード生成( ファイル名は同じ )
        // Statemachineコード
        var fileName = GetCodeFileName(outputFileName);
        File.WriteAllText(fileName, codeText);
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
