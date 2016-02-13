using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Katana;

public class CopyAnimation : MonoBehaviour 
{
    const string duplicatePostfix = "_edit";

    [MenuItem("Assets/Copy Animation Clip")]
    static void CopyCurvesToDuplicate()
    {
        List<AnimationClip> clipList = new List<AnimationClip>();
        clipList.Clear();
        foreach (UnityEngine.Object obj in Selection.objects)
        {
            // AnimationClipだけを取り出す
            AnimationClip clip = obj as AnimationClip;
            if (clip != null)
            {
                clipList.Add(clip);
            }
        }

        if (clipList.Count == 0)
        {
            return;
        }

        foreach (AnimationClip clip in clipList)
        {
            AnimationClip copyClip = CreateFile(clip);
            Duplicate(clip, copyClip);
            Debug.Log("Copying curves into " + copyClip.name + " is done");
        }
    }

    static AnimationClip CreateFile(AnimationClip importedClip)
    {
        string fileName = NameUtil.RemoveFileInvalidChars(importedClip.name) + duplicatePostfix + ".anim";
        string importedPath = AssetDatabase.GetAssetPath(importedClip);
        string copyPath = Path.GetDirectoryName(importedPath);
        copyPath += "/" +  fileName;

        AnimationClip src = AssetDatabase.LoadAssetAtPath(importedPath, typeof(AnimationClip)) as AnimationClip;
        AnimationClip newClip = new AnimationClip();
        newClip.name = src.name + duplicatePostfix;
        AssetDatabase.CreateAsset(newClip, copyPath);
        AssetDatabase.Refresh();
        return newClip;
    }

    static AnimationClip Duplicate(AnimationClip src, AnimationClip dest)
    {
        AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(src, true);
        foreach(var data in curveDatas)
        {
            EditorCurveBinding curve = new EditorCurveBinding();
            curve.path = data.path;
            curve.type = data.type;
            curve.propertyName = data.propertyName;           
            AnimationUtility.SetEditorCurve(
                dest,
                curve,
                data.curve
                );
        }
        return dest;
    }
}
