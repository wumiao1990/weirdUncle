using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class SingletonN<T> where T : class, new()
{
    public SingletonN() { }

    class SingletonCreator
    {
        static SingletonCreator() { }
        // Private object instantiated with private constructor
        internal static readonly T instance = new T();
    }

    public static T _
    {
        get { return SingletonCreator.instance; }
    }
}