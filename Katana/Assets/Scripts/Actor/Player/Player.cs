﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Katana.Messages;

namespace Katana
{
    public partial class Player : Character
    {
       
        protected override void OnUpdate()
        {
            base.OnUpdate();
            AnimationUpdate();
        }

        protected override void OnInitialize()
        {
            InitializeComponent();
            LoadSaveData();
        }

        public void Damage(int val)
        {
            base.Damage(new Damage(null, val));
        }

        protected override void OnDead()
        {
            GameManager.Instance.GameRestart();
        }

        protected override void OnDamaged(DamageResult damage)
        {
            //StartCoroutine("AfterDamaged");
            Debug.Log("Remain HP = " + AStatus.Hp);
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

        //セーブデータのロードと反映
        void LoadSaveData()
        {
            //セーブデータをロード
            if (SaveManager.Instance.GameSaveData.SaveVersion != "Null")
            {
                SaveDataCharacter chara = SaveManager.Instance.GameSaveData.MainChara;
                this.transform.position = chara.position;
                this.transform.rotation = chara.rotation;
            }
        }
    }
}