using UnityEngine;
using System.Collections;

namespace Katana
{
    // アニメーション関係をまとめたPlayerのpartial
    public class PlayerAnimator : AnimatorAccess.PlayerAnimator
    {
        public override void OnStateEnterToRun(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnterToRun(animator, stateInfo, layerIndex);
            Debug.Log("Enter Run");
        }

        public override void OnStateEnterToScrewKick(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnterToScrewKick(animator, stateInfo, layerIndex);
            Debug.Log("Screw Kick");
        }
    }
}