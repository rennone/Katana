using System;
using UnityEngine;
namespace AnimatorAccess
{
	[System.Serializable]
	public class PlayerAnimator : MonoBehaviour
	{
		public Animator _animator;

// Perameters
		protected readonly static int MoveSpeedHash = 526065662;
		public float GetMoveSpeed(){ return _animator.GetFloat(MoveSpeedHash); }
		public void SetMoveSpeed(float value){ _animator.SetFloat(MoveSpeedHash, value);}

		protected readonly static int IsJumpHash = 19011116;
		public bool GetIsJump(){ return _animator.GetBool(IsJumpHash); }
		public void SetIsJump(bool value){ _animator.SetBool(IsJumpHash, value);}

		protected readonly static int IsAttackHash = 2145273271;
		public bool GetIsAttack(){ return _animator.GetBool(IsAttackHash); }
		public void SetIsAttack(bool value){ _animator.SetBool(IsAttackHash, value);}

// State
		public static readonly int StateIdIdle = 1432961145;
		public bool IsIdle(){ return StateIdIdle == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdRUN00_F = 195871495;
		public bool IsRUN00_F(){ return StateIdRUN00_F == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdJump = 788460410;
		public bool IsJump(){ return StateIdJump == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdKick = -1751635351;
		public bool IsKick(){ return StateIdKick == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }

	}
}