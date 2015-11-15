using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public float JumpInitialVelocity;
	public float HorizontalInitialVelocity;
    public float JumpHeight;
    public float Gravity = 9.8f;

    private Rigidbody myRigidbody;
    private float nowJumpVelocity = 0;
    private float jumpStartHeight = 0;
    private float nowGravity = 0;
    private bool isGrounded = false;

	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
    }
	
	void Update () {
        //左右の移動
        Vector3 newPos = this.transform.position + GetHorizontalMoveScale() + GetVerticalMoveScale();
        myRigidbody.MovePosition(newPos);

        //ジャンプ
        if (Input.GetButtonDown ("Jump") && isGrounded) {
            SetJump();
		}
	}

	void OnCollisionEnter(Collision collision){

		Debug.Log ("OnCollisionEnter");
		ContactPoint[] contacts = collision.contacts;

		foreach(var contact in contacts)
		{
			Vector3 normal = contact.normal;

			// 上向きな場合
			if(normal.y > 0){
				Debug.Log(normal.ToString());
                nowGravity = 0;
                nowJumpVelocity = 0;
                isGrounded = true;
            }
		}

	}

	private void SetJump(){
        isGrounded = false;
        nowJumpVelocity = JumpInitialVelocity;
        jumpStartHeight = this.transform.position.y;
    }

    //左右の移動量
    Vector3 GetHorizontalMoveScale()
    {
        Vector3 moveScale = (HorizontalInitialVelocity * Input.GetAxisRaw("Horizontal")) * Vector3.right * Time.deltaTime;
        return moveScale;
    }

    //上下の移動量
    Vector3 GetVerticalMoveScale()
    {
        if (isGrounded)
            return Vector3.zero;

        Vector3 moveScale = Vector3.up * nowJumpVelocity * Time.deltaTime;
        nowGravity += Time.deltaTime * Gravity;
        nowJumpVelocity -= nowGravity;
        return moveScale;
    }
}