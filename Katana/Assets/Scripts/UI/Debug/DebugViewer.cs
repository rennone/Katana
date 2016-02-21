using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Katana;
using Katana.Messages;
using UnityEngine.UI;

// デバッグ機能の表示
// 1. ホイールボタン長押しで表示される
// 2. 新しい機能を追加するときは, InitPagesに追加する.
public class DebugViewer : MonoBehaviour
{
    //スクロールビューのプレファブ
    [SerializeField] 
    private RectTransform scrollViewPreafab_;
    public RectTransform ScrollViewPrefab { get { return scrollViewPreafab_;} }

    //スクロールの各ノードのプレファブ
    [SerializeField] 
    private DebugMenu nodePrefab_;
    public DebugMenu NodePrefab { get { return nodePrefab_; } }

    // ページ階層の大本
    private DebugMenu root_;

    // スクロールページを保存する辞書
    Dictionary<int, HashSet<RectTransform>> PageTable_ { get; set; } 
    void Awake()
    {
        PageTable_ = new Dictionary<int, HashSet<RectTransform>>();
        root_ = Instantiate(NodePrefab) as DebugMenu;
        root_.InitAsPage("root", this, null);
        root_.gameObject.SetActive(false);          //rootはボタンを押すわけではないのでfalseにしておく
        root_.transform.SetParent(transform, false);

        InitPages();
    }


    //----------------------------------------------------------------------------------------------------//
    //-----------------------------------------デバッグ機能の編集-----------------------------------------//
    //----------------------------------------------------------------------------------------------------//
    // 機能の追加はここに記述する
    void InitPages()
    {
        // 以下はすべてサンプル
        //「test menu」という名前で 「hello worldと表示する」デバッグ機能をトップ階層に追加
        root_.AddMenu("test menu", () => { Debug.Log("hello world!"); });

        // testという新しいリストを追加
        var testPage = root_.AddPage("test");

        // 「nest menu」という名前で「this is nestと表示する」デバッグ機能をtestPageの下に追加
        testPage.AddMenu("nest menu", () => {Debug.Log("this is nest");});

        // 2段階にネストさせることも可能だが, 余り深くなりすぎてもわかりづらいだけなので
        // 多くても3つ目でやめておいたほうがいいと思われる.
        var nestPage = root_.AddPage("nest");
        var nestt2Page = nestPage.AddPage("nest A");
        nestt2Page.AddMenu("A", null);

        // 上記のように直接書いてもいいが, 余り多くなりすぎると関数分けしたほうが良い
        AddPlayerPage();    //プレイヤー用のページを追加する

        AddEnemyPage();     //エネミー用ページの作成

        AddSystemPage();
    }

    //! プレイヤー用のページを追加する
    void AddPlayerPage()
    {
        // プレイヤー用のデバッグページを作成
        var page = root_.AddPage("Player");

        // HP回復
        page.AddMenu("recover",() => { GameManager.Instance.Player.Damage(new Damage(null, -10)); });

        // ダメージ
        page.AddMenu("damaged", () => { GameManager.Instance.Player.Damage(new Damage(null, 10)); });

        for (int i = 0; i < 30; i++)
        {
            page.AddMenu("playerFunc " + i.ToString(), null);
        }
    }

    //! エネミー用のページを作成
    void AddEnemyPage()
    {
        var page = root_.AddPage("Enemy");
        page.AddMenu("create enemy1", null);
        page.AddMenu("create enemy2", null);
    }

    void AddSystemPage()
    {
        var page = root_.AddPage("System");
        page.AddMenu("Stop Time", null);
    }



    //----------------------------------------------以下DebugMenuからの登録用------------------------------------------//

    //! pageを登録する
    public void RegisterPage(DebugMenu menu)
    {
        var level = menu.NestLevel_;
        if (PageTable_.ContainsKey(level) == false)
            PageTable_.Add(level, new HashSet<RectTransform>());

        PageTable_[level].Add(menu.ScrollView_);
        menu.ScrollView_.transform.SetParent(transform, false);
    }


    //! 表示非表示を切り替える
    public void InvertActive()
    {
        var active = !gameObject.activeSelf;
        gameObject.SetActive(active);

        // 再び開いた時に2番目以下のページがが表示されないようにする
        if (active)
        {
            OpenPage(root_);
        }

        Debug.Log("InvertActive");
    }

    //! pageを開く
    public void OpenPage(DebugMenu page)
    {
        SetVisiblePageOverEqual(page.NestLevel_);

        // 自分を表示する
        page.ScrollView_.gameObject.SetActive(true);
    }

    //! level以下のページは非表示にする
    void SetVisiblePageOverEqual(int level)
    {
        // 自分と同じネスト以下のページはすべて非表示にする
        foreach (var pageList in PageTable_.Where((e) => e.Key >= level))
        {
            foreach (var p in pageList.Value)
            {
                p.gameObject.gameObject.SetActive(false);
            }
        }
    }
}
