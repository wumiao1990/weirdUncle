using System.Collections;
using System.Collections.Generic;
using Mga;
using UnityEngine;

public class AssetBundleManager: MonoBehaviour
{
    Dictionary<string, Object> m_assetCachePool = new Dictionary<string, Object>();
    
    private static AssetBundleManager m_instance = null;
    public static AssetBundleManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                var gameobject = new GameObject("AssetBundleManager");
                m_instance = gameobject.AddComponent<AssetBundleManager>();
                m_Resources = gameobject.AddComponent<IcResources>();
                m_AssetBundleManager = gameobject.AddComponent<IcAssetBundleManager>();
            }
            return m_instance;
        }
    }
    
    private static IcResources m_Resources = null;
    public static IcResources GetResources() { return m_Resources; }

    private static IcAssetBundleManager m_AssetBundleManager = null;
    public static IcAssetBundleManager GetAssetBundleManager() { return m_AssetBundleManager; }

    public bool m_isLocaAsset = true;
    
    public T InstantiatePrefab<T>(string asset, bool bInstantiate = true, string extension = IcConfig.EXTENSION_PREFAB) where T : class
    {
        Object resObj = Instance._GetAssetObject<T>(asset, extension);
        if (resObj == null)
            return null;

        if (bInstantiate)
        {
            return Object.Instantiate(resObj) as T;
        }

        return resObj as T;
    }
    
    Object _GetAssetObject<T>(string asset,string extension = IcConfig.EXTENSION_PREFAB)
    {
        // 1. try to get it from cache
        Object resObj;
        m_assetCachePool.TryGetValue(asset, out resObj);
        
        if (resObj != null)
            return resObj;

        string dir;
        string name;
        CUtil.SplitPath(asset, out dir, out name);

        resObj = m_Resources.GetPrefabDepend(asset,extension);
            
        if (resObj != null)
        {
            if(!m_assetCachePool.ContainsKey(asset))
                m_assetCachePool.Add(asset, resObj);
            return resObj;
        }

        // 3. try to load from asset bundle
        // ......

        // 4. cant find it
        if (resObj == null)
        {
            if (extension == IcConfig.EXTENSION_OGG || extension == IcConfig.EXTENSION_WAV)
                return resObj;
            UnityEngine.Debug.LogError("Cant Find Resource: " + asset);
        }
        return resObj;
    }
    
    public void UnLoadAllAssets<T>()
    {
        var assets = Resources.FindObjectsOfTypeAll(typeof(T));
        foreach (var asset in assets)
        {
            // 1. remove it frome cache
            _RemoveFromCacheByAsset(asset);

            // 2. unload it
            Resources.UnloadAsset(asset);
        }
    }
    
    public void UnloadResourceAsset<T>(T asset) where T : Object
    {
        try
        {
            // 1. remove it from cache
            _RemoveFromCacheByAssetAll();

            if (asset == null)
                return;

            // 2. unload it
            if (asset is Texture)
            {
                Resources.UnloadAsset(asset as Texture);
            }
            else if (asset is AnimationClip)
            {
                Resources.UnloadAsset(asset as AnimationClip);
            }
            else if (asset is Mesh)
            {
                Resources.UnloadAsset(asset as Mesh);
            }
            else if (asset is Material)
            {
                Resources.UnloadAsset(asset as Material);
            }
            else if (asset is AudioClip)
            {
                Resources.UnloadAsset(asset as AudioClip);
            }
            else
            {
                Resources.UnloadAsset(asset as Object);
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError(e.ToString());
        }
    }
    
    void _RemoveFromCacheByAssetAll()
    {
        m_assetCachePool.Clear();
    }
    void _RemoveFromCacheByAssetKey(string key)
    {
        m_assetCachePool.Remove(key);
    }

    void _RemoveFromCacheByAsset(Object asset)
    {
        string key = "";
        foreach (var entry in m_assetCachePool)
        {
            if (entry.Value != asset)
                continue;
            key = entry.Key;
        }

        _RemoveFromCacheByAssetKey(key);
    }
    
    public static void CleanInstance()
    {
        m_instance = null;
    }
}
