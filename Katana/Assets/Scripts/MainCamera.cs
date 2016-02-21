using UnityEngine;
using System.Collections;
using Katana;

public class MainCamera : Katana.Singleton<MainCamera> {

    Vector3 relativePos;

	void Start () {
        relativePos = this.transform.position - GameManager.Instance.Player.transform.position;
	}
	
	void Update () {
        this.transform.position = GameManager.Instance.Player.transform.position + relativePos;
	}
}
