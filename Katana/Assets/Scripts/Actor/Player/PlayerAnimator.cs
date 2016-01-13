using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ActorMotor))]
public class PlayerAnimator : MonoBehaviour {

    [SerializeField]
    Animator animator_ = null;

    ActorMotor motor_ = null;

    void Awake()
    {
        motor_ = GetComponent<ActorMotor>();
    }

	// Update is called once per frame
	void Update () 
    {
        animator_.SetBool("IsJump", motor_.IsJumping()); //ジャンプフラグのセット
        animator_.SetFloat("MoveSpeed", motor_.movement.velocity.magnitude + motor_.InputMoveDirection.magnitude * motor_.movement.MaxForwardSpeed);
	}
}
