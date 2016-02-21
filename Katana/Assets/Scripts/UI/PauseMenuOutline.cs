using UnityEngine;
using System.Collections;

public class PauseMenuOutline : MonoBehaviour {

    [SerializeField]
    Katana.PauseManager.PauseMenuState menuState;
    
    public void ChangeMenu(Katana.PauseManager.PauseMenuState state)
    {
        if(menuState == state)
        {
            this.gameObject.SetActive(true);
            Katana.PauseManager.I.SetMenuList(transform);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
