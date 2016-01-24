using System;
using UnityEngine;
using System.Collections;

// CharacterのHPなどのステータスを表す親クラス
// 

public class ActorStatus : MonoBehaviour
{

    
    virtual protected void Awake()
    {
        Debug.Log("ActorStatus Awake");
        hp_ = MaxHp;
    }

	// Use this for initialization
    virtual protected void Start()
	{
        Debug.Log("Start Actor Status");
	}
	
	// Update is called once per frame
    virtual protected void Update() 
    {
        // 毒でHPを減らす処理や
        // 時間経過によるステータス異常の回復などを行う
	}


    //HP減らす処理
    virtual public void DecreaseHP(int val)
    {
        bool alive = hp_ > 0;
        hp_ -= val;

        // 死んだとき
        if ( alive && hp_ <= 0)
        {
            hp_ = 0;

            if(OnDead != null)
                OnDead();
        }
    }

    //HP増やす処理
    virtual public void IncreaseHP(int val)
    {
        bool full = hp_ == MaxHp;
        hp_ += val;

        // 完全回復したとき
        if (!full &&  hp_ >= MaxHp)
        {
            hp_ = MaxHp;

            if(OnFullRecovered != null)
                OnFullRecovered();
        }
    }

    //protected virtual void OnDead() { }

    //protected virtual void OnFullRecovered() { }





    // 最大HP
    [SerializeField]
    private int maxHp_ = 10000;
    public int MaxHp { get { return maxHp_; } }

    // HP
    private int hp_;
    public int Hp { get { return hp_; } }

    // 攻撃力
    [SerializeField]
    private int strong_ = 10;
    public int Strong { get { return strong_; } }

    // 防御力
    [SerializeField]
    private int defense_ = 10;
    public int Defense { get { return defense_; } }

    public Action OnDead{ private get; set; }
    public Action OnFullRecovered { private get; set; }

}
