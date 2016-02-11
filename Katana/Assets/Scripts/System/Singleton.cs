using UnityEngine;
using UnityEngine.Assertions;

// http://caitsithware.com/wordpress/archives/118

// Note : 継承クラスではAwakeを宣言しないこと.代わりにInitを使う
//      : Initの中で他のSingletonにアクセスしたとき, そのSingletonのInitは完了しているかはわからない!
//      : 
namespace Katana
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _mInstance;

        public static T Instance
        {
            get
            {
                if (_mInstance != null)
                    return _mInstance;

                // シーンから探してくる
                _mInstance = (T)FindObjectOfType(typeof(T));

                if (_mInstance == null)
                {
                    // シーンになかったら型名と同じプレハブがないか探してくる。
                    System.Type type = typeof(T);
                    var prefabPath = "Prefabs/Singletons/" + type.Name.ToString();
                    var prefab = Resources.Load(prefabPath);

                    if (prefab == null)
                    {
                        Debug.LogError("prefab " + prefabPath + " not exist");
                    }
                    else
                    {
                        var gameObject = Instantiate(prefab) as GameObject;
                        _mInstance = gameObject.GetComponent<T>();
                        if (_mInstance == null)
                        {
                            Debug.LogError("Problem during the creation of " + type.ToString(), gameObject);
                        }
                    }

                }
                return _mInstance;
            }
        }

        protected static string PrefabName()
        {
            return "Test";
        }

        // シーンをまたいでも存在し続けるかどうか
        protected virtual bool IsPersistent()
        {
            return false;
        }

        // 初期化関数
        protected virtual void OnInitialize()
        {
        }

        // 終了関数
        protected virtual void OnFinalize()
        {
        }



        static void Initialize(T instance)
        {
            if (_mInstance == null)
            {
                // 代入と初期化
                _mInstance = instance;
                _mInstance.OnInitialize();
                //永続性を有効にするときは親を持たないtransformとする。
                if (instance.IsPersistent())
                {
                    instance.gameObject.transform.parent = null;
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            else if (_mInstance != instance)
            {
                //永続性を有効のフラグを持つSingletonがすでにある場合、後から生成しようとした
                //Ｏｂｊｅｃｔは自身を破棄します。
                if (instance.IsPersistent())
                {
                    DestroyImmediate(instance.gameObject);
                }
            }
        }

        static void Finalize(T instance)
        {
            if (_mInstance == instance)
            {
                _mInstance.OnFinalize();

                _mInstance = null;
            }
        }

        // Instanceに登録
        // 
        private void Awake()
        {
           Initialize(this as T);
        }

        private void OnDestroyed()
        {
            Finalize(this as T);
        }


        // ゲームが終了したときにSingletonが削除されていなかったら削除する
        private void OnApplicationQuit()
        {
            if (_mInstance != null)
            {
                DestroyImmediate(_mInstance.gameObject);
            }
        }
    }
}