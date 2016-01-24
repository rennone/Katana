using UnityEngine;
using System.Collections;

namespace Katana
{
    // 猪突猛進型のAI
    // ひたすら真っすぐ進むのみ
    [RequireComponent(typeof(ActorMotor))]
    public class RusherEnemyAI : MonoBehaviour
    {
        private ActorMotor motor_ = null;

        private Animator animator_;

        // Use this for initialization
        void Awake()
        {
            motor_ = GetComponent<ActorMotor>();
            animator_ = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            // あまり理に簡単なAIなので入力処理とアニメーション処理を同じファイルに書いてある
            // Input処理
            motor_.InputMoveDirection = transform.forward;  //ひたすら前進

            //Animator処理
            animator_.SetInteger(Animator.StringToHash("State"), 1);
        }
    }
}

