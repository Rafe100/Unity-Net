using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> :BaseSingleton where T:BaseSingleton {

    private static T _instance;
    private static readonly object _lock = new object();

    private static bool applicatonIsQuitting = false;

    public static void Initialize(bool isDontDestray = true)
    {
        InitInstance(isDontDestray);
    }

    public static T Instance
    {
        get
        {
            if(applicatonIsQuitting)
            {
                return null;
            }
            return _instance;
        }
    }
  

    private static void InitInstance(bool isNeedDontDestroy)
    {
        applicatonIsQuitting = false;
        if(_instance == null)
        {
            lock(_lock)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if(_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "SaoNIma" + typeof(T).ToString();

                }
            }
        }
    }

}
