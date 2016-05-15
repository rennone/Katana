using UnityEngine;
using System.Collections;

public class RepeatUVScroller : MonoBehaviour {

    [SerializeField]
    Vector2 scrollDirection = Vector2.zero;

    [SerializeField]
    float scrollSpeed = 0.1f;

    [SerializeField]
    private float repeatDistance = 1f;

    Material thisMat = null;

    void Start()
    {
        thisMat = this.GetComponent<MeshRenderer>().material; GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
        thisMat.SetTextureOffset("_MainTex", Vector2.zero);
    }

    Vector2 nowScrollSize = Vector2.zero;
    float scrollDistance = 0;
    void Update()
    {
        nowScrollSize += scrollDirection * scrollSpeed * Time.deltaTime;

        if(nowScrollSize.magnitude >= repeatDistance)
        {
            scrollDirection *= -1;
            scrollDistance = 0;
        }

        thisMat.SetTextureOffset("_MainTex", nowScrollSize);
    }
}
