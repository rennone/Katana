using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public int pauseNumber = 0;
    [SerializeField]
    Image image;

	public void ChangeActive(bool active)
    {
        if (active)
        {
            image.sprite = Katana.PauseManager.I.menu_Active;
        }
        else
        {
            image.sprite = Katana.PauseManager.I.menu_Normal;
        }
    }
	
}
