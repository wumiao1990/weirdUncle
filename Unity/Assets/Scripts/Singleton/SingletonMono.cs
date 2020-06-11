using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MonoResPath("SingletonMono/")]
public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    public static readonly string BaseResPath = "SingletonMono/";
    static T instance;
    public static T _ { get { return Instance; } }
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 在当前运行时 Hierarchy 内查找实例
                instance = ExtentionUnity.FindInScene<T>();
                if (instance == null)
                {
                    // 实例不存在则搞一个
                    var type = typeof(T);
                    // 读取路径
                    string path = type.GetAttributeValue((MonoResPathAttribute mrp) => mrp.ResPath);

                    if (string.IsNullOrEmpty(path))
                    {
                        Debug.LogFormat("[SingletonMono] new GameObject AddComponent : {0}", type.Name);

                        // 没有配置路径，那么创建一个
                        var go = new GameObject(type.Name);
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<T>();
                    }
                    else
                    {
                        // 加载失败，就失败了
                        if (path == BaseResPath)
                        {
                            path = path + type.Name + "/" + type.Name;
                        }
                        

                        var go = AssetBundleManager.Instance.InstantiatePrefab<GameObject>(path);
                        if (go != null)
                        {
                            instance = go.GetComponent<T>();
                        }
                        else
                        {
                            Debug.LogErrorFormat("Singleton {0}, {1} 应当被迁移到AssetBundle系统.", type, path);
                            var asset = Resources.Load<T>(path);
                            if (asset == null)
                            {
                                return null;
                            }

                            instance = Instantiate<T>(asset);
                            instance.name = asset.name;
                        }
                    }
                }
                instance.Active();
            }
            return instance;
        }
    }
    public static T GetInstance() { return Instance; }

    public static bool HasActiveInstance()
    {
        var thisInstance = FindObjectOfType<T>();
        if (thisInstance == null)
        {
            return false;
        }

        if (!thisInstance.gameObject.activeInHierarchy || !thisInstance.isActiveAndEnabled)
        {
            return false;
        }

        return true;
    }

    public static bool HasInstance()
    {
        return instance != null;
    }

    void Awake()
    {
        if (instance.IsNull())
        {
            instance = (T)this;
        }
        else if (instance != this)
        {
            if (instance.gameObject == this.gameObject)
            {
                // 销毁脚本实例
                // 重复挂在多个脚本销毁脚本
                Destroy(this);
            }
            else
            {
                if (gameObject.name == typeof(T).Name)
                {
                    // 销毁 GameObject
                    // 重复的 Prefab 实例
                    Debug.LogFormat("Destory duplication gameObject name = {0}, in Scene {1} . the gameObject Can't Contain Other irrelevant script", gameObject.name, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                    Destroy(this.gameObject);
                }
                else
                {
                    // 销毁脚本实例
                    // 其他物体上挂在重复脚本销毁脚本
                    Destroy(this);
                }
            }
        }
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        //XiimoonLog.LogErrorFormat("OnAwake {0} {1}" , typeof(T).Name , Time.frameCount);
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }

    public bool Visible
    {
        set
        {
            this.SetActive(value);
        }
    }

    public virtual void Show()
    {
        Visible = true;
    }

    public virtual void Hide()
    {
        Visible = false;
    }

    public static void S_Hide()
    {
        if (instance.NotNull())
        {
            instance.Hide();
        }
        else
        {
            Debug.LogWarningFormat("Destroy failure ! {0} instance is null .", typeof(T).Name);
        }
    }

    public static void Destroy(float delay = 0)
    {
        if (instance.NotNull())
        {
            Destroy(instance.gameObject, delay);
            instance = null;
        }
        else
        {
            Debug.LogWarningFormat("Destroy failure ! {0} instance is null .", typeof(T).Name);
        }
    }
}
