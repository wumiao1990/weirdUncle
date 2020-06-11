using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtentionUnity
{
	public static bool IsNull(this System.Object obj)
	{
		return obj == null;
	}
	public static bool NotNull(this System.Object obj)
	{
		return obj != null;
	}
	public static bool IsNull(this UnityEngine.Object obj)
	{
		return obj == null;
	}
	public static bool NotNull(this UnityEngine.Object obj)
	{
		return obj != null;
	}
	public static bool IsNull(this UnityEngine.MonoBehaviour obj)
	{
		return obj == null;
	}
	public static bool NotNull(this UnityEngine.MonoBehaviour obj)
	{
		return obj != null;
	}
	public static bool IsEmpty<T>(this T[] str)
	{
		return str.IsNull() || str.Length == 0;
	}
    
	public static void ClearChild(this GameObject go)
	{
		if (go.IsNull()) return;
		go.transform.ClearChild();
	}

	public static void ClearChild(this Component com)
	{
		if (com.IsNull() || com.transform == null) return;
		if (com.transform.childCount <= 0) return;
		foreach (Transform tran in com.transform)
		{
			UnityEngine.Object.Destroy(tran.gameObject);
		}
	}
	public static void _ActiveChild(this Component com, int index)
	{
		if (index < 0) return;
		if (com == null) return;
		com.transform._ActiveChild(index);
	}
	
	public static void Active(this Component com)
	{
		com.SetActive(true);
	}
	
	public static void SetActive(this Component com, bool isActive)
	{
		if (com.IsNull() || com.gameObject.activeSelf == isActive) return;
		com.gameObject.SetActive(isActive);
	}
	
	/// <summary>
	/// 得到子节点
	/// </summary>
	public static Transform GetChildBone(Transform self, string name)
	{
		Queue<Transform> searchQueue = new Queue<Transform>();
		searchQueue.Enqueue(self);
		while (searchQueue.Count > 0)
		{
			var trans = searchQueue.Dequeue();
			if (trans.gameObject.name.Equals(name))
			{
				return trans;
			}
			foreach (Transform childTrans in trans)
			{
				searchQueue.Enqueue(childTrans);
			}
		}
		return null;
	}

	/// <summary>
	/// 得到子节点
	/// </summary>
	public static Transform CreateGameObject(string name)
	{
		var gameObject = new GameObject(name);
		return gameObject.transform;
	}

	public static void Destroy(UnityEngine.Object obj)
	{
		GameObject.Destroy(obj);
	}
	
	public static T FindInScene<T>() where T : Component
	{
		var objs = Resources.FindObjectsOfTypeAll<T>();
		foreach (var temp in objs)
		{
			if (!temp.gameObject.scene.IsValid())
			{
				continue;
			}
			return temp;
		}
		return null;
	}
	
	public static TValue GetAttributeValue<TAttribute, TValue>(
		this Type type,
		Func<TAttribute, TValue> valueSelector)
		where TAttribute : Attribute
	{
		var att = type.GetCustomAttributes(
			typeof(TAttribute), true
		).FirstOrDefault() as TAttribute;
		if (att != null)
		{
			return valueSelector(att);
		}
		return default(TValue);
	}
	
	public static void ClearChildForEditor(this Component com)
	{
		if (com.IsNull() || com.transform == null) return;
		if (com.transform.childCount <= 0) return;

		while (com.transform.childCount > 0)
		{
			Transform t = com.transform.GetChild(0);
			t.parent = null;

			UnityEngine.Object.DestroyImmediate(t.gameObject);
		}
		// foreach (Transform tran in com.transform)
		// {
		//     UnityEngine.Object.DestroyImmediate(tran.gameObject);
		// }
	}
	
	public static bool IsEmpty(this string str)
	{
		return str.IsNull() || str == "" || str == "nil" || str == "null";
	}
}
