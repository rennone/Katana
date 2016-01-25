using System;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace UnityEditor
{
    [InitializeOnLoad]
    public class SceneViewEditor : Editor
    {
        private static Vector2 _mousePosition = new Vector2(-1, -1);

        private const string HotKey = "%#c";    //ショートカットキー Ctrl + Shift + t
        private const string CommandName = "Tools/Create/Move Player";  // コマンド名
        private const string CommandNameWithHotkey = CommandName + " " + HotKey;

        static SceneViewEditor()
        {
            // マウスイベントはOnGUIの中でしか取れないので, SceneViewのOnGUIに
            // マウス位置を保存する関数をdeleagteで渡す
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        [MenuItem(CommandNameWithHotkey)]
        public static void Create()
        {
            // スクリーン上の座標は必ず正なので, 負の数だった場合は初期化できていない
            if(_mousePosition.x < 0 || _mousePosition.y < 0)
                return;

            var player = (GameObject)FindObjectsOfType(typeof(GameObject)).First(o => o.name == "Player");
            var camera = (GameObject)FindObjectsOfType(typeof(GameObject)).First(o => o.name == "Main Camera");

            if (player == null)
            {
                Debug.LogError("There is not Object whose name is Player. check name of Object");
                return;
            }

            if (camera == null)
            {
                Debug.LogError("There is not Object whose name is Main Camera. check name of Object");
                return;
            }

            // SceneViewのカメラ
            var sCamera = SceneView.GetAllSceneCameras().First();

            // マウスの位置をカメラ座標に変換
            var pos = new Vector2(_mousePosition.x, _mousePosition.y);
            var position = sCamera.ScreenToWorldPoint(pos);
            position.y = 2*sCamera.transform.position.y - position.y;   //y座標が逆になっているの酒精

            // プレイヤーの位置を変更
            var pPosition = player.transform.position;
            player.transform.position = new Vector3(position.x, position.y, player.transform.position.z);

            // カメラの位置を変更
            var cPosition = camera.transform.position;
            camera.transform.position = new Vector3(position.x, position.y, cPosition.z);
        }

        // マウスの位置を保存する
        static void OnSceneGUI(SceneView sceneView)
        {
            Debug.Log(Event.current.mousePosition);
            _mousePosition = Event.current.mousePosition;
        }
    }
}
