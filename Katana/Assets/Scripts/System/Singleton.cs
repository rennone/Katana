using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T I
    {
        get
        {
            if(instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
            }
            return instance;
        }
    }

    void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    protected virtual void Awake()
    {
        CheckInstance();
    }

    protected virtual bool CheckInstance()
    {
        if (this == I) { return true; }
        Destroy(this);
        return false;
    }
}