using UnityEngine;
using System.Collections;

public class PlayerStatus : ActorStatus
{
    private PlayerController controller_ = null;

    override protected void Awake()
    {
        base.Awake();
        Debug.Log("Awake PlayerStatus");
        controller_ = GetComponent<PlayerController>();
    }

    override protected void Update()
    {
        base.Update();
        HPManager.I.ChangeDisplayHP(Hp);
    }

    protected override void OnDead()
    {
        controller_.OnDead();
    }
}
