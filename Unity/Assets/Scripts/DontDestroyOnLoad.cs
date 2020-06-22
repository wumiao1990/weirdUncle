using UnityEngine;
using System.Collections.Generic;
public class DontDestroyOnLoad : MonoBehaviour
{
    static HashSet<GameObject> dontDestroyPool = new HashSet<GameObject>();
    // Use this for initialization
    void Awake()
    {
        // if (!dontDestroyPool.Contains(gameObject))
        {
            GameObject.DontDestroyOnLoad(gameObject);

            // dontDestroyPool.Add(gameObject);
        }
    }
}
