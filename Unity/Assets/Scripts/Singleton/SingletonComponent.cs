using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonComponent<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent(typeof(T)) as T;
            }
            return _instance;
        }
    }
    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
