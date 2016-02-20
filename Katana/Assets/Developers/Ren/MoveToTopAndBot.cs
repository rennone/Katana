using UnityEngine;
using System.Collections;

public class MoveToTopAndBot : MonoBehaviour {

    CapsuleCollider controller;

    public GameObject top, bot;

    Vector3 Top
    {
        get
        {
            return transform.TransformPoint( Vector3.up * controller.height / 2 +  controller.center );
        }
    }

    Vector3 Bottom
    {
        get
        {
            return transform.TransformPoint(-Vector3.up * controller.height / 2 + controller.center);
        }
    }

    Vector3 Center
    {
        get
        {
            return transform.TransformPoint(  controller.center + transform.localPosition);
        }
    }

	// Use this for initialization
	void Start () 
    {
        controller = GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        top.transform.position = Top;
        bot.transform.position = Bottom;
        Debug.Log(Center + " : " + transform.position + " : " + controller.center + " : " + controller.height);
	}
}
