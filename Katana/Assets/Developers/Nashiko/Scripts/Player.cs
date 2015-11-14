using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public float JumpInitialVelocity;
	public float HorizontalInitialVelocity;

	private bool isJumpping = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//左右の移動
		this.transform.position += (HorizontalInitialVelocity * Input.GetAxisRaw("Horizontal")) * Vector3.right;

		//ジャンプ
		if (Input.GetButtonDown ("Jump") && !isJumpping) {
			isJumpping = true;
			jump ();
		}
	}

	void OnCollisionEnter(Collision collision){
//		if (collider.gameObject.name () == "Terrain") {
		Debug.Log("test");
   		isJumpping = false;
//		}
	}

	private void jump(){
		var rigitbody = GetComponent<Rigidbody>();
		rigitbody.AddForce (Vector3.up * JumpInitialVelocity);
	}
}