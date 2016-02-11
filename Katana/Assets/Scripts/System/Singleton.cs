using UnityEngine;
using UnityEngine.Assertions;

// http://caitsithware.com/wordpress/archives/118

// Note : �p���N���X�ł�Awake��錾���Ȃ�����.�����Init���g��
//      : Init�̒��ő���Singleton�ɃA�N�Z�X�����Ƃ�, ����Singleton��Init�͊������Ă��邩�͂킩��Ȃ�!
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

                // �V�[������T���Ă���
                _mInstance = (T)FindObjectOfType(typeof(T));

                if (_mInstance == null)
                {
                    // �V�[���ɂȂ�������^���Ɠ����v���n�u���Ȃ����T���Ă���B
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

        // �V�[�����܂����ł����݂������邩�ǂ���
        protected virtual bool IsPersistent()
        {
            return false;
        }

        // �������֐�
        protected virtual void OnInitialize()
        {
        }

        // �I���֐�
        protected virtual void OnFinalize()
        {
        }



        static void Initialize(T instance)
        {
            if (_mInstance == null)
            {
                // ����Ə�����
                _mInstance = instance;
                _mInstance.OnInitialize();
                //�i������L���ɂ���Ƃ��͐e�������Ȃ�transform�Ƃ���B
                if (instance.IsPersistent())
                {
                    instance.gameObject.transform.parent = null;
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            else if (_mInstance != instance)
            {
                //�i������L���̃t���O������Singleton�����łɂ���ꍇ�A�ォ�琶�����悤�Ƃ���
                //�n�����������͎��g��j�����܂��B
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

        // Instance�ɓo�^
        // 
        private void Awake()
        {
           Initialize(this as T);
        }

        private void OnDestroyed()
        {
            Finalize(this as T);
        }


        // �Q�[�����I�������Ƃ���Singleton���폜����Ă��Ȃ�������폜����
        private void OnApplicationQuit()
        {
            if (_mInstance != null)
            {
                DestroyImmediate(_mInstance.gameObject);
            }
        }
    }
}