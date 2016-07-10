using UnityEngine;
using System.Collections;

namespace Katana
{
    // アニメーション関係をまとめたPlayerのpartial
    public class PlayerAnimator : AnimatorAccess.PlayerAnimatorController
    {
        public override void OnStateEnterToRun(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnterToRun(animator, stateInfo, layerIndex);
            Debug.Log("Enter Run");
        }

        // スクリューキック中に重力を0にする
        public override void OnStateEnterToScrewKick(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Enter Screw Kick");
            PlayerMotor motor = GetComponent<PlayerMotor>();
            motor.movement.EnableGravity = false;
            base.OnStateEnterToScrewKick(animator, stateInfo, layerIndex);
        }

        // スクリューキックが終わると重力をデフォルトにもどす
        public override void OnStateExitFromScrewKick(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Exit Screw Kick");
            PlayerMotor motor = GetComponent<PlayerMotor>();
            motor.movement.EnableGravity = true;
            base.OnStateExitFromScrewKick(animator, stateInfo, layerIndex);
        }
    }
}