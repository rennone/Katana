﻿using UnityEngine;
using System.Collections;

public class Player : Singleton<Player> {
	
    public Animator anim;
    public float JumpInitialVelocity;   //ジャンプの速さ
	public float HorizontalInitialVelocity; //左右移動の速さ
    public float JumpHeight;    //ジャンプの高さ
    public float MaxFallSpeed = 10;
    public int MaxHP = 100;

    private int nowHP;    //HP
    private bool isMuteki = false;  //無敵状態かどうかのフラグ
    private Rigidbody myRigidbody;  //自分のRigidbody
    private float jumpStartHeight = -100;
    private bool isGrounded = false;

	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
        IncreaseHP(MaxHP);  //HPをセット
    }
	
	void Update () {
        //左右の移動
        Vector3 newPos = this.transform.position + GetHorizontalMoveScale() + GetVerticalMoveScale();
        SetRotation(newPos);
        myRigidbody.MovePosition(newPos);

        //ジャンプ
        if (Input.GetButtonDown ("Jump") && isGrounded) {
            SetJump();
		}
	}

	void OnCollisionEnter(Collision collision){
		ContactPoint[] contacts = collision.contacts;

		foreach(var contact in contacts)
		{
			Vector3 normal = contact.normal;

			// 上向きな場合
			if(normal.y > 0){
                isGrounded = true;
                anim.SetBool("IsJump", false);
                JumpInitialVelocity = Mathf.Abs(JumpInitialVelocity);
            }
		}

	}

	private void SetJump(){
        isGrounded = false;
        anim.SetBool("IsJump", true);
        jumpStartHeight = this.transform.position.y;
    }

    //左右の移動量
    Vector3 GetHorizontalMoveScale()
    {
        Vector3 moveScale = (HorizontalInitialVelocity * Input.GetAxisRaw("Horizontal")) * Vector3.right * Time.deltaTime;
        anim.SetFloat("MoveSpeed", moveScale.magnitude);
        return moveScale;
    }

    //上下の移動量
    Vector3 GetVerticalMoveScale()
    {
        if (isGrounded)
            return Vector3.zero;

        if ((this.transform.position.y - jumpStartHeight) > JumpHeight)
            JumpInitialVelocity = Mathf.Abs(JumpInitialVelocity) * -1;

        Vector3 moveScale = Vector3.up * JumpInitialVelocity * Time.deltaTime;
        return moveScale;
    }

    void SetRotation(Vector3 newPos)
    {
        if (Mathf.Abs(newPos.x) < 0f)
            return;

        newPos.y = this.transform.position.y;
        Vector3 newForward = Vector3.Lerp(this.transform.forward,(newPos - this.transform.position).normalized,0.5f);
        this.transform.forward = newForward;
    }

    //HP減らす処理
    public void DecreaseHP(int point)
    {
        if (isMuteki)
            return;

        nowHP -= point;
        if(nowHP <= 0)
        {
            nowHP = 0;
            GameOver();
        }

        HPManager.I.ChangeDisplayHP(nowHP);
    }

    //HP増やす処理
    public void IncreaseHP(int point)
    {
        nowHP += point;
        if(nowHP > MaxHP)
        {
            nowHP = MaxHP;
        }
        HPManager.I.ChangeDisplayHP(nowHP);
    }

    public void GameOver()
    {
        GameManager.I.GameRestart();
    }
}