using UnityEngine;
using System.Collections;

public class RandomUVScroller : MonoBehaviour {

    [SerializeField]
    float minScrollSpeed = 0.1f;

    [SerializeField]
    float maxScrollSpeed = 0.2f;

    [SerializeField]
    private float repeatDistance = 1f;

    Material thisMat = null;

    void Start()
    {
        thisMat = this.GetComponent<MeshRenderer>().material; GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
        thisMat.SetTextureOffset("_MainTex", Vector2.zero);
        nowScrollSpeed = Random.Range(minScrollSpeed, maxScrollSpeed);
        nowScrollDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    Vector2 nowScrollSize = Vector2.zero;
    float nowScrollSpeed = 0;
    Vector2 nowScrollDirection = Vector2.zero;
    float scrollDistance = 0;
    void Update()
    {
        nowScrollSize += nowScrollDirection * nowScrollSpeed * Time.deltaTime;

        if(nowScrollSize.magnitude >= repeatDistance)
        {
            nowScrollDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            nowScrollSpeed = Random.Range(minScrollSpeed, maxScrollSpeed);
            scrollDistance = 0;
        }

        thisMat.SetTextureOffset("_MainTex", nowScrollSize);
    }
}
