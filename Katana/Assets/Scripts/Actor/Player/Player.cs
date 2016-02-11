using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Katana
{
    public class Player : Character
    {
        private PlayerInput _input;

        protected override void AwakeSelf()
        {
            AStatus.OnDead = OnDead;

            // TODO : プレハブに書いておくので必要ないようにする.
            var characterController = GetComponent<CharacterController>();
            characterController.center = new Vector3(0, 0.7f, 0);
            characterController.radius = 0.2f;
            characterController.height = 1.25f;

            var motor = GetComponent<ActorMotor>();
            motor.movement.FreezePosition.Z = true;
            motor.jumping.baseHeight  = 4.0f;
            motor.jumping.extraHeight = 3.0f;

            var capsuleColider = GetComponent<CapsuleCollider>();
            capsuleColider.isTrigger = true;
            capsuleColider.center = new Vector3(0, 0.75f, 0);
            capsuleColider.radius = 0.2f;
            capsuleColider.height = 1.6f;

            _input = new PlayerInput(GetComponent<PlayerMotor>(), GetComponent<PlayerAnimator>());
        }

        protected override void UpdateSelf()
        {
            base.UpdateSelf();
            _input.Update();
        }

        protected override void OnDead()
        {
            GameManager.Instance.GameRestart();
        }

        protected override void OnDamaged(DamageInfo damage)
        {
            StartCoroutine("AfterDamaged");
        }

        private void SetLayerRecursively(GameObject actor, int layer)
        {
            actor.layer = layer;
            foreach (Transform child in actor.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        private IEnumerator AfterDamaged()
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
                    renderer.materials[0].SetFloat("_Transparent", Mathf.Abs(Mathf.Sin(elapsedTime*4*Mathf.PI)));
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
}