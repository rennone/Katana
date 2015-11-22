using UnityEngine;
using System.Collections;

public class MainCamera : Singleton<MainCamera> {

    Vector3 relativePos;

	void Start () {
        relativePos = this.transform.position - Player.I.transform.position;
	}
	
	void Update () {
        this.transform.position = Player.I.transform.position + relativePos;
	}
}
