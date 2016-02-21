using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEditor
{
    [InitializeOnLoad]
    public class SceneViewEditor : Editor
    {
        private static Vector2 _sceneViewMouse = new Vector2(-1, -1);

        private const string HotKey = "%#c";    //ショートカットキー Ctrl + Shift + t
        private const string CommandName = "Tools/Move/Player(shortcut only)";  // コマンド名
        private const string CommandNameWithHotkey = CommandName + " " + HotKey;

        static SceneViewEditor()
        {
            // マウスイベントはOnGUIの中でしか取れないので, SceneViewのOnGUIに
            // マウス位置を保存する関数をdeleagteで渡す
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        [MenuItem(CommandNameWithHotkey)]
        public static bool EnableMovePlayer()
        {
            var sCamera = SceneView.GetAllSceneCameras().First();
            
            if (sCamera == null)
                return false;

            return _sceneViewMouse.x >= 0 && _sceneViewMouse.y >= 0 && _sceneViewMouse.x < sCamera.pixelWidth && _sceneViewMouse.y < sCamera.pixelHeight;
        }

        [MenuItem(CommandNameWithHotkey)]
        public static void MovePlayerPosition()
        {
            // スクリーン上の座標は必ず正なので, 負の数だった場合は初期化できていない
            if(_sceneViewMouse.x < 0 || _sceneViewMouse.y < 0)
                return;

            var player = (GameObject)FindObjectsOfType(typeof(GameObject)).First(o => Regex.IsMatch(o.name ,"^Player"));
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
            var pos = new Vector2(_sceneViewMouse.x, _sceneViewMouse.y);
            var position = sCamera.ScreenToWorldPoint(pos);
            position.y = 2*sCamera.transform.position.y - position.y;   //y座標が逆になっているの酒精

            // プレイヤーの位置を変更
            player.transform.position = new Vector3(position.x, position.y, player.transform.position.z);

            // カメラの位置を変更
            camera.transform.position = new Vector3(position.x, position.y, camera.transform.position.z);
        }

        // マウスの位置を保存する
        static void OnSceneGUI(SceneView sceneView)
        {
            _sceneViewMouse = Event.current.mousePosition;
        }
    }
}
