using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_SizeController : MonoBehaviour {

    //とりあえず1920*1080（16:9）を基準としてる
    const float BASE_WIDTH = 1920;
    const float BASE_HEIGHT = 1080;

    RectTransform thisRect;
    Vector3 firstScale;
    Vector3 firstPos;

	void Start () {
        thisRect = GetComponent<RectTransform>();
        firstScale = thisRect.localScale;
        firstPos = thisRect.localPosition;

        FitWindow();
	}

    void FitWindow()
    {
        float scaleRatio = Screen.width/BASE_WIDTH;
        thisRect.localScale = firstScale * scaleRatio;
        thisRect.localPosition = firstPos * scaleRatio;
    }
}
