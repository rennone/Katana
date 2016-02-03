using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T mInstance = null;
    [SerializeField]
    protected bool persistance = false;

    public static T Instance
    {
        get { return mInstance; }
        private set { mInstance = value; }
    }

    public static T I
    {
        get { return mInstance; }
    }

    protected virtual void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this as T;
            mInstance.Init();
            //�i������L���ɂ���Ƃ��͐e�������Ȃ�transform�Ƃ���B
            if (persistance)
            {
                this.gameObject.transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            //�i������L���̃t���O������Singleton�����łɂ���ꍇ�A�ォ�琶�����悤�Ƃ���
            //�n�����������͎��g��j�����܂��B
            if (persistance)
            {
                DestroyImmediate(this.gameObject);
                //DestroyObject( gameObject );
            }
        }
    }

    protected virtual void Init() { }

    protected virtual void OnApplicationQuit()
    {
        mInstance = null;
    }
}
