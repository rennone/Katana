using UnityEngine;
using System.Collections;
using System;
[RequireComponent(typeof(ActorMotor))]
public class PlayerInput : MonoBehaviour {


    private ActorMotor motor_ = null;
	// Use this for initialization
	void Start () 
    {
        motor_ = GetComponent<ActorMotor>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        motor_.InputMoveDirection = (Input.GetAxisRaw("Horizontal") * Time.deltaTime) * Vector3.right;
	    motor_.InputJump = Input.GetButtonDown("Jump");
    }
}
