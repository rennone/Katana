using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPManager : Singleton<HPManager> {

    [SerializeField]
    Text hpText;

	public void ChangeDisplayHP(int point)
    {
        hpText.text = "HP : " + point.ToString();
    }
}
