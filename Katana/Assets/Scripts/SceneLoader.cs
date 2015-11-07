using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{

    // Note : ここでは地形のルートオブジェクトの名前がシーンの名前と同じという前提が必要
    public class SceneLoader : MonoBehaviour
    {
        // ロード済みのステージリスト
        public static List<string> LoadedMapList = new List<string>();

        [SerializeField]
        public List<string> LoadLevelLists = new List<string>();

        private Boolean callLoadCoRoutine = false;

        bool IsForcusedStage()
        {
            // カメラ(主人公)の位置を取得
            var player = GameObject.FindGameObjectWithTag("Player");
            var position = player.transform.position;

            var terrain = GetComponentInChildren<Terrain>();
           
            var terrainPosition = terrain.transform.position;
            //Debug.Log(gameObject.name + " : " + position + "," + terrainPosition);

            return terrainPosition.x < position.x && terrainPosition.x + 500 > position.x;
        }

        // 隣り合うシーンをロードする
        IEnumerator Load()
        {
            // Note : 地形のルートオブジェクトの名前がシーンの名前と同じという前提が必要
            var levelNames = GameObject.FindGameObjectsWithTag("LoadableScene").Select(l => l.gameObject.name);

            // 読み込みリストの内, まだ読み込んでいないものをシーンに追加する
            foreach (var loadLevelList in LoadLevelLists.Where( l => levelNames.Contains(l) == false))
            {
                AsyncOperation ao = Application.LoadLevelAdditiveAsync(loadLevelList);
                yield return ao;
            }
        }

        // いらないシーンを削除する
        IEnumerator UnLoad()
        {
            var levelNames = GameObject.FindGameObjectsWithTag("LoadableScene").Select(l => l.gameObject.name);
            
            foreach (var level in levelNames
                .Where(l =>  LoadLevelLists.Contains(l) == false && l != gameObject.name))
            {
                Debug.Log("Unload " + level);
                var root = GameObject.Find(level);
                Destroy(root);
                LoadedMapList.Remove(level);
                yield return null;
            }
        }

        // Use this for initialization
        void Start () 
        {
            Debug.Log(gameObject.name + "is Loaded");

            // もうすでに読み込まれていたら削除する
            if (LoadedMapList.Contains(gameObject.name))
            {
                Destroy(gameObject);
            }
            else
            {
                LoadedMapList.Add(gameObject.name);
            }
        }
    
        // Update is called once per frame
        void Update ()
        {
            if (callLoadCoRoutine)
            {
                // 別のステージに移ったら, フラグを下ろす
                callLoadCoRoutine = IsForcusedStage();
                return;
            }

            // 主人公がいるのが自分のステージなら, 隣り合うシーンを読み込みを行う
            if (IsForcusedStage())
            {
                Debug.Log(gameObject.name + " Start Load");
                callLoadCoRoutine = true;   //2回読み込まないようにフラグを立てる
                StartCoroutine("Load");
                StartCoroutine("UnLoad");
            }
        }
    }
}
