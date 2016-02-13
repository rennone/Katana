using System;
using UnityEngine;
namespace AnimatorAccess
{
	[System.Serializable]
	public class FightingUnityChan_free : MonoBehaviour
	{
		public Animator _animator;

// Perameters
		protected readonly static int JabHash = 1561758614;
		public void Jab(){ _animator.SetTrigger (JabHash); } public void ResetJab() { _animator.ResetTrigger (JabHash); }

		protected readonly static int HikickHash = -109429625;
		public void Hikick(){ _animator.SetTrigger (HikickHash); } public void ResetHikick() { _animator.ResetTrigger (HikickHash); }

		protected readonly static int SpinkickHash = 1686858944;
		public void Spinkick(){ _animator.SetTrigger (SpinkickHash); } public void ResetSpinkick() { _animator.ResetTrigger (SpinkickHash); }

		protected readonly static int Rising_PHash = -1246624435;
		public void Rising_P(){ _animator.SetTrigger (Rising_PHash); } public void ResetRising_P() { _animator.ResetTrigger (Rising_PHash); }

		protected readonly static int HeadspringHash = -1051224368;
		public void Headspring(){ _animator.SetTrigger (HeadspringHash); } public void ResetHeadspring() { _animator.ResetTrigger (HeadspringHash); }

		protected readonly static int LandHash = 137525990;
		public void Land(){ _animator.SetTrigger (LandHash); } public void ResetLand() { _animator.ResetTrigger (LandHash); }

		protected readonly static int RunHash = 1748754976;
		public bool GetRun(){ return _animator.GetBool(RunHash); }
		public void SetRun(bool value){ _animator.SetBool(RunHash, value);}

		protected readonly static int ScrewKHash = -351223662;
		public void ScrewK(){ _animator.SetTrigger (ScrewKHash); } public void ResetScrewK() { _animator.ResetTrigger (ScrewKHash); }

		protected readonly static int DamageDownHash = -2142859338;
		public void DamageDown(){ _animator.SetTrigger (DamageDownHash); } public void ResetDamageDown() { _animator.ResetTrigger (DamageDownHash); }

		protected readonly static int SAMKHash = 763179511;
		public void SAMK(){ _animator.SetTrigger (SAMKHash); } public void ResetSAMK() { _animator.ResetTrigger (SAMKHash); }

// State
		public static readonly int StateIdIdle = 1432961145;
		public bool IsIdle(){ return StateIdIdle == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdRun = -827840423;
		public bool IsRun(){ return StateIdRun == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdHikick = -95841596;
		public bool IsHikick(){ return StateIdHikick == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdRISING_P = -836276697;
		public bool IsRISING_P(){ return StateIdRISING_P == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdJab = -75152913;
		public bool IsJab(){ return StateIdJab == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdHeadspring = -160291357;
		public bool IsHeadspring(){ return StateIdHeadspring == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdLand = 558702772;
		public bool IsLand(){ return StateIdLand == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdSpinkick = -1900937766;
		public bool IsSpinkick(){ return StateIdSpinkick == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdDamageDown = -1218233211;
		public bool IsDamageDown(){ return StateIdDamageDown == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdScrewKick = -106577475;
		public bool IsScrewKick(){ return StateIdScrewKick == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }
		public static readonly int StateIdSAMK = 67267493;
		public bool IsSAMK(){ return StateIdSAMK == _animator.GetCurrentAnimatorStateInfo(0).fullPathHash; }

	}
}