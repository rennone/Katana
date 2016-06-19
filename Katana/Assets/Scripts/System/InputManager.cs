using UnityEngine;
using System.Collections;

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

    bool isKeeping_Horizontal = false;
    bool isKeeping_Vertical = false;

    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    protected override bool IsPersistent()
    {
        return true;
    }

    void Update () {

        if (!CanInput) return;

        //左右の入力判定-----------------------------------------------------------------------------
        var inputHorizontal = Input.GetAxisRaw("Horizontal");
        if(inputHorizontal != 0)
        {
            if(!isKeeping_Horizontal)
            {
                isKeeping_Horizontal = true;
                if (onAxisDownEvent != null)
                    onAxisDownEvent(Axis.Horizontal, inputHorizontal);
            }
            if (onAxisEvent != null)
                onAxisEvent(Axis.Horizontal, inputHorizontal);
        }
        else
        {
            if (isKeeping_Horizontal)
            {
                isKeeping_Horizontal = false;
                if (onAxisUpEvent != null)
                    onAxisUpEvent(Axis.Horizontal, inputHorizontal);
            }
        }

        //上下の入力判定-----------------------------------------------------------------------------
        var inputVertical = Input.GetAxisRaw("Vertical");
        print(inputVertical);
        if (inputVertical >= 0.01f || inputVertical <= -0.01f)
        {
            if (!isKeeping_Vertical)
            {
                isKeeping_Vertical = true;
                if (onAxisDownEvent != null)
                    onAxisDownEvent(Axis.Vertical, inputVertical);
            }
            if (onAxisEvent != null)
                onAxisEvent(Axis.Vertical, inputVertical);
        }
        else
        {
            if (isKeeping_Vertical)
            {
                isKeeping_Vertical = false;
                if (onAxisUpEvent != null)
                    onAxisUpEvent(Axis.Vertical, inputVertical);
            }
        }

        //Aボタン----------------------------------------------------------------------------------
        string inputKey = "Fire1";
        if (Input.GetButtonDown(inputKey))
        {
            if (onButtonDownEvent != null)
                onButtonDownEvent(Button.A);
        }
        if (Input.GetButton(inputKey))
        {
            if (onButtonEvent != null)
                onButtonEvent(Button.A);
        }
        if (Input.GetButtonUp(inputKey))
        {
            if (onButtonUpEvent != null)
                onButtonUpEvent(Button.A);
        }
        //Bボタン----------------------------------------------------------------------------------
        inputKey = "Jump";
        if (Input.GetButtonDown(inputKey))
        {
            if (onButtonDownEvent != null)
                onButtonDownEvent(Button.B);
        }
        if (Input.GetButton(inputKey))
        {
            if (onButtonEvent != null)
                onButtonEvent(Button.B);
        }
        if (Input.GetButtonUp(inputKey))
        {
            if (onButtonUpEvent != null)
                onButtonUpEvent(Button.B);
        }
        //Startボタン----------------------------------------------------------------------------------
        inputKey = "Start";
        if (Input.GetButtonDown(inputKey))
        {
            if (onButtonDownEvent != null)
                onButtonDownEvent(Button.Start);
        }
        if (Input.GetButton(inputKey))
        {
            if (onButtonEvent != null)
                onButtonEvent(Button.Start);
        }
        if (Input.GetButtonUp(inputKey))
        {
            if (onButtonUpEvent != null)
                onButtonUpEvent(Button.Start);
        }
    }
}
