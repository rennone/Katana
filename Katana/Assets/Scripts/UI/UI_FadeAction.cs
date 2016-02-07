using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UI_FadeAction : MonoBehaviour {

    [SerializeField]
    Color activeColor;
    [SerializeField]
    Color fadeColor;
    [SerializeField]
    float fadeTime = 0.5f;

    Material thisMat;
    
    void Start()
    {
        thisMat = this.GetComponent<Image>().material;
    }

    public void SetFadeInOut(bool isFadeIn)
    {
        if (isFadeIn)
        {
            thisMat.DOColor(activeColor,fadeTime);
        }
        else
        {
            thisMat.DOColor(fadeColor, fadeTime);
        }
    }
}
