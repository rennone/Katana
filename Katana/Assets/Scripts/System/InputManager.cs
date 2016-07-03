using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : Katana.Singleton<InputManager> {

    public enum Button
    {
        None,
        A,
        B,
        R1,
        L1,
        Start,
    }

    public enum Axis
    {
        None,
        Horizontal,
        Vertical,
    }

    public delegate void OnButtonDown(Button button);
    public static event OnButtonDown onButtonDownEvent = null;

    public delegate void OnButton(Button button);
    public static event OnButton onButtonEvent = null;

    public delegate void OnButtonUp(Button button);
    public static event OnButtonUp onButtonUpEvent = null;

    public delegate void OnAxisDown(Axis axis,float value);
    public static event OnAxisDown onAxisDownEvent = null;

    public delegate void OnAxis(Axis axis, float value);
    public static event OnAxis onAxisEvent = null;

    public delegate void OnAxisUp(Axis axis, float value);
    public static event OnAxisUp onAxisUpEvent = null;

    [HideInInspector]
    public bool CanInput = true;

    bool isKeepingHorizontal = true;
    bool isKeepingVertical = true;
    bool inputWaiting = true;
    Dictionary<string, Button> buttonDictionary = new Dictionary<string, Button>();

    void Awake()
    {
        buttonDictionary.Add("Fire1", Button.A);
        buttonDictionary.Add("Jump", Button.B);
        buttonDictionary.Add("Start", Button.Start);
    }

    IEnumerator Start()
    {
        //シーン遷移時の入力の待機
        yield return new WaitForSeconds(0.5f);
        inputWaiting = false;
    }

    //protected override bool IsPersistent()
    //{
    //    return true;
    //}

    void Update () {
        if (!CanInput || inputWaiting) return;

        //左右の入力判定-----------------------------------------------------------------------------
        var inputHorizontal = Input.GetAxisRaw("Horizontal");
        if(inputHorizontal != 0)
        {
            if(!isKeepingHorizontal)
            {
                isKeepingHorizontal = true;
                if (onAxisDownEvent != null)
                    onAxisDownEvent(Axis.Horizontal, inputHorizontal);
            }
            if (onAxisEvent != null)
                onAxisEvent(Axis.Horizontal, inputHorizontal);
        }
        else
        {
            if (isKeepingHorizontal)
            {
                isKeepingHorizontal = false;
                if (onAxisUpEvent != null)
                    onAxisUpEvent(Axis.Horizontal, inputHorizontal);
            }
        }

        //上下の入力判定-----------------------------------------------------------------------------
        var inputVertical = Input.GetAxisRaw("Vertical");
        if (inputVertical != 0)
        {
            if (!isKeepingVertical)
            {
                isKeepingVertical = true;
                if (onAxisDownEvent != null)
                    onAxisDownEvent(Axis.Vertical, inputVertical);
            }
            if (onAxisEvent != null)
                onAxisEvent(Axis.Vertical, inputVertical);
        }
        else
        {
            if (isKeepingVertical)
            {
                isKeepingVertical = false;
                if (onAxisUpEvent != null)
                    onAxisUpEvent(Axis.Vertical, inputVertical);
            }
        }


        //ボタン群---------------------------------------------------------------------------------
        foreach(var button in buttonDictionary)
        {
            if (Input.GetButtonDown(button.Key))
            {
                if (onButtonDownEvent != null)
                    onButtonDownEvent(button.Value);
            }
            if (Input.GetButton(button.Key))
            {
                if (onButtonEvent != null)
                    onButtonEvent(button.Value);
            }
            if (Input.GetButtonUp(button.Key))
            {
                if (onButtonUpEvent != null)
                    onButtonUpEvent(button.Value);
            }
        }
    }
}
