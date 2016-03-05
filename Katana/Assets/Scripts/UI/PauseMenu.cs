using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    //このenumに何か追加する時は必ず一番下に追加してね
    public enum MenuAction
    {
        None,
        ChangeSaveMenu,
        ChangeOptionMenu,
        ChangeTitleMenu,
        Save,
        Load
    }

    [SerializeField]
    MenuAction action = MenuAction.None;

    public int pauseNumber = 0;
    [SerializeField]
    Image image;

	public void ChangeActive(bool active)
    {
        if (active)
        {
            image.sprite = Katana.PauseManager.Instance.menu_Active;
        }
        else
        {
            image.sprite = Katana.PauseManager.Instance.menu_Normal;
        }
    }

    //決定が押された時のアクション
    public void Decision()
    {
        switch (action)
        {
            case MenuAction.None:
                ChangeNone();
                break;
            case MenuAction.ChangeSaveMenu:
                ChangeSaveMenu();
                break;
            case MenuAction.ChangeOptionMenu:
                ChangeOptionMenu();
                break;
            case MenuAction.ChangeTitleMenu:
                ChangeTitleMenu();
                break;
            case MenuAction.Save:
                Save();
                break;
            case MenuAction.Load:
                Load();
                break;
        }
    }

//ここから下アクション内容***************************************************
    void ChangeNone()
    {
        Katana.PauseManager.Instance.PushPauseButton();    //ポーズメニューを閉じる
    }

    void ChangeSaveMenu()
    {
        Katana.PauseManager.Instance.ChangePauseMenuState(Katana.PauseManager.PauseMenuState.SaveMenu);
        SoundManager.Instance.PlaySound(Katana.GameManager.Instance.Player.transform, SoundKey.SE_MENU_DECIDE);
    }

    void ChangeOptionMenu()
    {
        Katana.PauseManager.Instance.ChangePauseMenuState(Katana.PauseManager.PauseMenuState.OptionMenu);
        SoundManager.Instance.PlaySound(Katana.GameManager.Instance.Player.transform, SoundKey.SE_MENU_DECIDE);
    }

    void ChangeTitleMenu()
    {
        Katana.PauseManager.Instance.ChangePauseMenuState(Katana.PauseManager.PauseMenuState.TitleMenu);
        SoundManager.Instance.PlaySound(Katana.GameManager.Instance.Player.transform, SoundKey.SE_MENU_DECIDE);
    }
	
    void Save()
    {
        Katana.SaveManager.Instance.SaveAll();
        Katana.PauseManager.Instance.PushPauseButton();    //ポーズメニューを閉じる
    }

    void Load()
    {
        Katana.SaveManager.Instance.LoadAll();
        SoundManager.Instance.PlaySound(Katana.GameManager.Instance.Player.transform, SoundKey.SE_MENU_DECIDE);
        Katana.GameManager.Instance.GameRestart();
    }
}
