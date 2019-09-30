using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInstance<T>  : MonoBehaviour where T : MonoBehaviour {
    static  T instance;

    public static T Instance {
        get {
            if (instance == null) {
                var obj = new GameObject(typeof(T).FullName);
                instance = obj.AddComponent<T>();

            }
            return instance;
        }
    }

    private void Awake() {
        instance = this as T;
    }

}
