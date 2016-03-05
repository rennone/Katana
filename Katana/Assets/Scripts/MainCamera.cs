using UnityEngine;
using System.Collections;
using Katana;

public class MainCamera : Katana.Singleton<MainCamera> {

    Vector3 relativePos = new Vector3(0f,2f,-2f);

	void Start () {
        //relativePos = this.transform.position - GameManager.Instance.Player.transform.position;
	}
	
	void Update () {
        this.transform.position = GameManager.Instance.Player.transform.position + relativePos;
	}
}
