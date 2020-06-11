using System.Collections;
using System.Collections.Generic;

public abstract class Poolable<T>
where T : Poolable<T>, new()
{
    private static readonly ObjectPool<T> s_ObjPool = new ObjectPool<T>(null, t => t.Clear());
    public static T Get()
    {
        return s_ObjPool.Get();
    }

    public static void Release(T item)
    {
        s_ObjPool.Release(item);
    }

    public abstract void Clear();
}