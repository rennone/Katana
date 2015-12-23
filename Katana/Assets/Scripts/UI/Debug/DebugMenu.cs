using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class DebugMenu : MonoBehaviour
{
    struct Menu
    {
        public readonly string Name;
        public readonly Action Function;
        public Menu(string name, Action func)
        {
            Name = name;
            Function = func;
        }
    }

    // スクロールのネストの深さ
    public int NestLevel_ { get; private set; }

    public Transform Content{ get { return ScrollView_.GetChild(0).transform; } }

    public RectTransform ScrollView_ { get; private set; }

    private Action Function_ = null;

    private DebugViewer viewer_;


    private List<Menu> menuList_ = null;

    public DebugMenu Parent_ { get; private set; }

    // ボタンの名前
    string Name_
    {
        get 
        {  
            var text = gameObject.GetComponentInChildren<Text>();
            return text.text;
        }

        set
        {
            var text = gameObject.GetComponentInChildren<Text>();
            text.text = value;
        }
    }


    void Init(string name, Action func, DebugMenu parent)
    {
        Name_     = name;
        Function_ = func;
        Parent_   = parent;
        NestLevel_ = Parent_ != null ? Parent_.NestLevel_ + 1 : 0;
    }

    public void InitAsPage(string name, DebugViewer viewer, DebugMenu parent)
    {
        Init(name, OpenPage, parent);

        viewer_ = viewer;

        // スクロールビューの生成
        ScrollView_ = Instantiate(viewer_.ScrollViewPrefab) as RectTransform;
        ScrollView_.gameObject.SetActive(false);

        // 位置の設定
        ScrollView_.localPosition = new Vector3(NestLevel_ * ScrollView_.rect.width + ScrollView_.localPosition.x, 0);

        // 最初にviewrに登録しておく
        {
            viewer_.RegisterPage(this);
        }

        // 動的にスクロールページを生成する必要があるかもしれないので 
        // メニューのリストを保持しておく
        {
            menuList_ = new List<Menu>();
        }
    }

    public void InitAsMenu(string name, Action func, DebugMenu parent)
    {
        Init(name, func, parent);
        var pageTag = transform.FindChild("PageTag");
        pageTag.gameObject.SetActive(false);
    }

    // name : 機能の名前
    // func : 実行される関数( 引数, 戻り値ともに無しの静的関数)
    public void AddMenu(string name, Action func)
    {
        if (ScrollView_ == null)
            return;

        var node = Instantiate(viewer_.NodePrefab) as DebugMenu;
        node.InitAsMenu(name, func, this);
        node.transform.SetParent(Content, false);

        // メモリとかの都合で動的にスクロールページを生成する必要があるかもしれないので 
        // 引数を保持しておく
        menuList_.Add(new Menu(name, func));
    }

    public DebugMenu AddPage(string name)
    {
        var node = Instantiate(viewer_.NodePrefab) as DebugMenu;
        node.InitAsPage(name, viewer_, this);
        node.transform.SetParent(Content, false);

        return node;
    }

    public void Execute()
    {
        if (Function_ == null)
        {
            Debug.Log( Name_ + " : null function");
        }
        else
        {
            Function_();
        }
    }

    void OpenPage()
    {
        viewer_.OpenPage(this);
    }
}
