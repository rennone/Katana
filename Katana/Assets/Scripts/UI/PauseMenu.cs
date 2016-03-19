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
        Load,
        VolumeBGM
    }

    [SerializeField]
    MenuAction action = MenuAction.None;

    public int pauseNumber = 0;
    [SerializeField]
    Image image;    //各項目の背景イメージ
    [SerializeField]
    Slider slider;  //スライダーがある項目用（音量調整とか）

    void OnEnable()
    {
        switch (action)
        {
            case MenuAction.VolumeBGM:
                slider.value = SoundManager.Instance.BGMVoume;
                break;
        }
    }

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

    //左右キーが押された時のアクション
    public void PushRightLeft(bool isRight)
    {
        switch (action)
        {
            case MenuAction.VolumeBGM:
                VolumeBGM(isRight);
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
        SaveData.SaveAll();
        Katana.PauseManager.Instance.PushPauseButton();    //ポーズメニューを閉じる
    }

    void Load()
    {
        SaveData.LoadAll();
        SoundManager.Instance.PlaySound(Katana.GameManager.Instance.Player.transform, SoundKey.SE_MENU_DECIDE);
        Katana.GameManager.Instance.GameRestart();
    }

    void VolumeBGM(bool isRight)
    {
        var newValue = slider.value;
        newValue += (isRight == true) ? 0.1f : -0.1f;
        newValue = Mathf.Clamp01(newValue);
        slider.value = newValue;
        SoundManager.Instance.BGMVoume = newValue;
    }
}
