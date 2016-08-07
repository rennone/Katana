using UnityEngine;
using System.Collections;

public class OneTimeCollisionIgnorer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        Physics.IgnoreCollision(collider.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), true);
    }
    void OnTriggerExit(Collider collider)
    {
        Physics.IgnoreCollision(collider.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
    }
}
