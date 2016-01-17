using UnityEngine;
using System.Collections;
using System.Linq;

namespace UnityEditor
{
    public class EditorUtils : Editor
    {
        private const string HotKey = "%#c";    //ショートカットキー Ctrl + Shift + t
        private const string CommandName = "Tools/Create/Move Player";  // コマンド名
        private const string CommandNameWithHotkey = CommandName + " " + HotKey;

        [MenuItem(CommandNameWithHotkey)]
        public static void Create()
        {
            var player = (GameObject)Object.FindObjectsOfType(typeof(GameObject)).Where(o => o.name == "Player").First();
            var camera = (GameObject)Object.FindObjectsOfType(typeof(GameObject)).Where(o => o.name == "Main Camera").First();
            Debug.Log(player.transform.position);

            var sCamera = SceneView.GetAllSceneCameras().First();

            // プレイヤーの位置を変更
            var pPosition = player.transform.position;
            player.transform.position = new Vector3(sCamera.transform.position.x, sCamera.transform.position.y, 0);

            // カメラの位置を変更
            var cPosition = camera.transform.position;
            camera.transform.position = new Vector3(sCamera.transform.position.x, sCamera.transform.position.y, cPosition.z);
        }
    }
}
