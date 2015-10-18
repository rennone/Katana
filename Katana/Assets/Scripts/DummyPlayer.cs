using UnityEngine;
using System.Collections;

public class DummyPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position += Vector3.right * Input.GetAxis("Horizontal");
    }
}
