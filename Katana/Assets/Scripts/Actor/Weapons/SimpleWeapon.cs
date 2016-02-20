using UnityEngine;
using System.Collections;
using Katana;
using Katana.Messages;

public class SimpleWeapon : Katana.WeaponBase 
{

    protected override void OnCollideGimmick(GimmickBase gimmick, Collider collider)
    {
        Debug.Log("On Cllide Gimmick");
        base.OnCollideGimmick(gimmick, collider);
        Owner.Attack(gimmick, new Damage(0, Vector3.zero, collider, this));
    }

    protected override void OnCollidePlayer(Player player, Collider collider)
    {
        base.OnCollidePlayer(player, collider);
        Debug.Log("On Collide Player");
        Owner.Attack(player, new Damage(0, Vector3.zero, collider, this));
    }
}
