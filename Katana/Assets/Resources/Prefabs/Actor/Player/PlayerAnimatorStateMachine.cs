using System;
using UnityEngine;
// generated by Editor/AnimatorAccess ☀
namespace AnimatorAccess
{
	[System.Serializable]
	public class PlayerAnimatorStateMachine : StateMachineBehaviour
	{		
		public PlayerAnimatorController Controller{ private get; set;}

		//State
		public const int StateIdAttack_Fire1 = -375459552;
		public const int StateIdIdle = 1432961145;
		public const int StateIdRun = -827840423;
		public const int StateIdJump = 788460410;
		public const int StateIdKick = -1751635351;
		public const int StateIdScrewKick = -106577475;
		public const int StateIdDown = -1783470761;



		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//新しいステートに移り変わった時に実行
			switch(stateInfo.fullPathHash)
			{
				// case 
				case StateIdAttack_Fire1 : if( Controller!=null){ Controller.OnStateEnterToAttack_Fire1(animator, stateInfo, layerIndex); } break;
				case StateIdIdle : if( Controller!=null){ Controller.OnStateEnterToIdle(animator, stateInfo, layerIndex); } break;
				case StateIdRun : if( Controller!=null){ Controller.OnStateEnterToRun(animator, stateInfo, layerIndex); } break;
				case StateIdJump : if( Controller!=null){ Controller.OnStateEnterToJump(animator, stateInfo, layerIndex); } break;
				case StateIdKick : if( Controller!=null){ Controller.OnStateEnterToKick(animator, stateInfo, layerIndex); } break;
				case StateIdScrewKick : if( Controller!=null){ Controller.OnStateEnterToScrewKick(animator, stateInfo, layerIndex); } break;
				case StateIdDown : if( Controller!=null){ Controller.OnStateEnterToDown(animator, stateInfo, layerIndex); } break;

			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//ステートが次のステートに移り変わる直前に実行
			switch(stateInfo.fullPathHash)
			{
				// case 
				case StateIdAttack_Fire1 : if( Controller!=null){ Controller.OnStateExitFromAttack_Fire1(animator, stateInfo, layerIndex); } break;
				case StateIdIdle : if( Controller!=null){ Controller.OnStateExitFromIdle(animator, stateInfo, layerIndex); } break;
				case StateIdRun : if( Controller!=null){ Controller.OnStateExitFromRun(animator, stateInfo, layerIndex); } break;
				case StateIdJump : if( Controller!=null){ Controller.OnStateExitFromJump(animator, stateInfo, layerIndex); } break;
				case StateIdKick : if( Controller!=null){ Controller.OnStateExitFromKick(animator, stateInfo, layerIndex); } break;
				case StateIdScrewKick : if( Controller!=null){ Controller.OnStateExitFromScrewKick(animator, stateInfo, layerIndex); } break;
				case StateIdDown : if( Controller!=null){ Controller.OnStateExitFromDown(animator, stateInfo, layerIndex); } break;

			}
		}

		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			//スクリプトが貼り付けられたステートマシンに遷移してきた時に実行
			
		}

		public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
		{
			//スクリプトが貼り付けられたステートマシンから出て行く時に実行			
			
		}
		/*
		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//MonoBehaviour.OnAnimatorMoveの直後に実行される
			switch(stateInfo.fullPathHash)
			{
				// case 
				case StateIdAttack_Fire1 : if( Controller!=null){ Controller.OnStateMoveAttack_Fire1(animator, stateInfo, layerIndex); } break;
				case StateIdIdle : if( Controller!=null){ Controller.OnStateMoveIdle(animator, stateInfo, layerIndex); } break;
				case StateIdRun : if( Controller!=null){ Controller.OnStateMoveRun(animator, stateInfo, layerIndex); } break;
				case StateIdJump : if( Controller!=null){ Controller.OnStateMoveJump(animator, stateInfo, layerIndex); } break;
				case StateIdKick : if( Controller!=null){ Controller.OnStateMoveKick(animator, stateInfo, layerIndex); } break;
				case StateIdScrewKick : if( Controller!=null){ Controller.OnStateMoveScrewKick(animator, stateInfo, layerIndex); } break;
				case StateIdDown : if( Controller!=null){ Controller.OnStateMoveDown(animator, stateInfo, layerIndex); } break;

			}
		}
		*/
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//最初と最後のフレームを除く、各フレーム単位で実行
			switch(stateInfo.fullPathHash)
			{
				// case 
				case StateIdAttack_Fire1 : if( Controller!=null){ Controller.OnStateUpdateAttack_Fire1(animator, stateInfo, layerIndex); } break;
				case StateIdIdle : if( Controller!=null){ Controller.OnStateUpdateIdle(animator, stateInfo, layerIndex); } break;
				case StateIdRun : if( Controller!=null){ Controller.OnStateUpdateRun(animator, stateInfo, layerIndex); } break;
				case StateIdJump : if( Controller!=null){ Controller.OnStateUpdateJump(animator, stateInfo, layerIndex); } break;
				case StateIdKick : if( Controller!=null){ Controller.OnStateUpdateKick(animator, stateInfo, layerIndex); } break;
				case StateIdScrewKick : if( Controller!=null){ Controller.OnStateUpdateScrewKick(animator, stateInfo, layerIndex); } break;
				case StateIdDown : if( Controller!=null){ Controller.OnStateUpdateDown(animator, stateInfo, layerIndex); } break;

			}
		}

		public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//MonoBehaviour.OnAnimatorIKの直後に実行される
			switch(stateInfo.fullPathHash)
			{
				// case 
				case StateIdAttack_Fire1 : if( Controller!=null){ Controller.OnStateIkAttack_Fire1(animator, stateInfo, layerIndex); } break;
				case StateIdIdle : if( Controller!=null){ Controller.OnStateIkIdle(animator, stateInfo, layerIndex); } break;
				case StateIdRun : if( Controller!=null){ Controller.OnStateIkRun(animator, stateInfo, layerIndex); } break;
				case StateIdJump : if( Controller!=null){ Controller.OnStateIkJump(animator, stateInfo, layerIndex); } break;
				case StateIdKick : if( Controller!=null){ Controller.OnStateIkKick(animator, stateInfo, layerIndex); } break;
				case StateIdScrewKick : if( Controller!=null){ Controller.OnStateIkScrewKick(animator, stateInfo, layerIndex); } break;
				case StateIdDown : if( Controller!=null){ Controller.OnStateIkDown(animator, stateInfo, layerIndex); } break;

			}
		}


	}
}