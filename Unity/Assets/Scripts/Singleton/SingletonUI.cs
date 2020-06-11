using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonUI<T> : SingletonMono<T> where T : SingletonUI<T>
{
    public static new string BaseResPath = "UI/";
    static Transform uiRoot;
    public static Transform UIRoot
    {
        get
        {
            if (uiRoot.IsNull())
            {
                uiRoot = GameObject.Find("GameUIRoot").transform;
            }
            return uiRoot;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        Instance.transform.SetParent(UIRoot);
        Instance.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        Instance.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        Instance.transform.localScale = Vector3.one;
    }
}
