using UnityEngine;
using System.Collections;

namespace Katana
{
    // アニメーション関係をまとめたPlayerのpartial
    public class PlayerAnimator : AnimatorAccess.PlayerAnimator
    {
        public override void OnStateEnterToRun(Animator animator, AnimatorStateInfo stateInfo)
        {
            base.OnStateEnterToRun(animator, stateInfo);
            Debug.Log("Enter Run");
        }
    }
}