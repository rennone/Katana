using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Katana.Messages;

namespace Katana
{
    public partial class Player : Character
    {
        private PlayerMotor _motor;

        private CapsuleCollider _capsule;

        private SimpleWeapon _kick;
        public PlayerAnimator _animatorAccess;

        // TODO : Vector.up ではなくローカルの上方向ベクトル
        public Vector3 Top
        {
            get { return transform.TransformPoint(Vector3.up*_capsule.height/2 + _capsule.center); }
        }

        public Vector3 Bottom
        {
            get { return transform.TransformPoint(-Vector3.up*_capsule.height/2 + _capsule.center); }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            AnimationUpdate();
        }

        protected override void OnInitialize()
        {
            _motor = GetComponent<PlayerMotor>();
            _capsule = GetComponent<CapsuleCollider>();
            _kick = GetComponentInChildren<SimpleWeapon>();
            var animator = GetComponent<Animator>();

            _motor.CanChangeDirection = IsNormalState;
            _animatorAccess = animator.GetBehaviour<PlayerAnimator>();
            _animatorAccess.SetAnimator(animator);
        }

        public void Damage(int val)
        {
            base.Damage(new Damage(val));
        }

        protected override void OnDead()
        {
            GameManager.Instance.GameRestart();
        }

        protected override void OnDamaged(DamageResult damage)
        {
            //StartCoroutine("AfterDamaged");
        }

        // ダメージを受けた時の処理
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