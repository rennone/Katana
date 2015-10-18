using UnityEngine;
using System.Collections;

public class DummyPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // left
        if (Input.GetKey(KeyCode.LeftArrow)) {
            this.transform.position += Vector3.left;
        }
        // right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += Vector3.right;
        }
    }
}
