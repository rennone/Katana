using UnityEngine;
using System.Collections;

// プレイヤーをコピーした敵
public class TestEnemy : Katana.Player 
{
    protected override void OnDead()
    {
        AnimatorAccess.TriggerIsDead();
        GetComponent<CharacterController>().enabled = false;
        _motor.enabled = false;
    }
}
