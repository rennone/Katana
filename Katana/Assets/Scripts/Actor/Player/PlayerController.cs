using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(ActorMotor))]
[RequireComponent(typeof(ActorStatus))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : ActorController
{
    void Awake()
    {
        GetComponent<ActorStatus>().OnDead = OnDead;

        var characterController = GetComponent<CharacterController>();
        characterController.center = new Vector3(0, 0.7f, 0);
        characterController.radius = 0.2f;
        characterController.height = 1.25f;

        var motor = GetComponent<ActorMotor>();
        motor.movement.MaxForwardSpeed = 500;
        motor.movement.MaxSidewaysSpeed = 500;
        motor.movement.MaxBackwardsSpeed = 500;
        motor.movement.MaxGroundAcceleration = 1000;
        motor.movement.MaxAirAcceleration = 500;
        motor.movement.Gravity = 50;
        motor.movement.MaxFallSpeed = 500;
        motor.movement.FreezePosition.Z = true;
        motor.jumping.baseHeight = 4.5f;
        motor.jumping.extraHeight = 3.0f;

        var capsuleColider = GetComponent<CapsuleCollider>();
        capsuleColider.isTrigger = true;
        capsuleColider.center = new Vector3(0, 0.75f, 0);
        capsuleColider.radius = 2.1f;
        capsuleColider.height = 1.6f;
    }

    void Update()
    {

    }


    public void OnDead()
    {
        GameManager.I.GameRestart();
    }

    override public void Damage(int val)
    {
        base.Damage(val);
        StartCoroutine("AfterDamaged");
    }

    void SetLayerRecursively(GameObject actor, int layer)
    {
        actor.layer = layer;
        foreach (Transform child in actor.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    IEnumerator AfterDamaged()
    {
       // gameObject.layer = LayerName.PlayerDamaged;
        Dictionary<string, Shader> backups = new Dictionary<string, Shader>();
        var renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            backups[renderer.gameObject.name] = renderer.materials[0].shader;
            renderer.materials[0].shader = Shader.Find("Custom/surface_two_pass");
        }

        SetLayerRecursively(gameObject, LayerName.PlayerDamaged); //ダメージレイヤに追加
        float afterglowTime = 2.0f;
        float elapsedTime = 0.0f;
        while (elapsedTime < afterglowTime)
        {
            //renderer.material.color = new Color(1, 1, 1, Mathf.Sin(elapsedTime * 3));
            foreach (var renderer in renderers)
            {
                renderer.materials[0].SetFloat("_Transparent", Mathf.Abs(Mathf.Sin(elapsedTime * 4 * Mathf.PI)));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.materials[0].shader = backups[renderer.gameObject.name];
        }
        SetLayerRecursively(gameObject, LayerName.Player);
    }
}