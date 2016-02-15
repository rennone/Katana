using UnityEngine;
using System.Collections;
namespace Katana
{
    public partial class  Player
    {
        bool hasWeapon = false;
        // 移動可能か
        public bool CanMove()
        {
            return Animator.IsKickState() == false;
        }

        public bool CanAttack()
        {
            return hasWeapon;
        }

        public bool CanJump()
        {
            return true;
        }

        public void ReleaseAttack()
        {
            hasWeapon = true;
        }
    }

}

