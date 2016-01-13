using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(ActorMotor))]
[RequireComponent(typeof(PlayerStatus))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : IActor
{
    private bool isMuteki = false;  //無敵状態かどうかのフラグ
    private PlayerStatus status_ = null;

    void Awake()
    {
        status_ = GetComponent<PlayerStatus>();
    }

    void Update()
    {
       // Debug.Log(transform.position);
    }


    public void OnDead()
    {
        GameManager.I.GameRestart();
    }

    override public void Damage(int val)
    {
        status_.DecreaseHP(val);
        StartCoroutine("AfterDamaged");
        
    }

    override public void Recover(int val)
    {
        status_.IncreaseHP(val);
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