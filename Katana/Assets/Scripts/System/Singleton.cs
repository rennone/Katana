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
            //永続性を有効にするときは親を持たないtransformとする。
            if (persistance)
            {
                this.gameObject.transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            //永続性を有効のフラグを持つSingletonがすでにある場合、後から生成しようとした
            //Ｏｂｊｅｃｔは自身を破棄します。
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
