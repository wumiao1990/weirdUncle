
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class MonoResPathAttribute : Attribute
{
    //public static Type T = typeof(MonoResPathAttribute);
    public string ResPath;

    public MonoResPathAttribute() : this(null)
    {

    }

    public MonoResPathAttribute(string path)
    {
        ResPath = path;
    }
}