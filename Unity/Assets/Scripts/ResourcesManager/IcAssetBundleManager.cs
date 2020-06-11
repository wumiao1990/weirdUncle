using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Mga
{
    public class LoadedAssetBundle
    {
        public AssetBundle m_AssetBundle;
        public int m_ReferencedCount;

        public LoadedAssetBundle(AssetBundle assetBundle)
        {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = 1;
        }

        public LoadedAssetBundle(AssetBundle assetBundle, int referencedCount)
        {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = referencedCount;
        }
    }

    public class LoadingWWW
    {
        public WWW m_www;
        public int m_ReferencedCount;

        public LoadingWWW(WWW www)
        {
            m_www = www;
            m_ReferencedCount = 1;
        }
    }
    public class IcAssetBundleManager : MonoBehaviour
    {

        string m_BaseDownloadingWWWURL = string.Empty;
        string m_BaseDownloadingFSURL = string.Empty;
        const string kAssetBundlesPath = "/AssetBundles/";
        Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        Dictionary<string, LoadingWWW> m_DownloadingWWWs = new Dictionary<string, LoadingWWW>();
        Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string>();
        List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

        public AssetBundleManifest m_AssetBundleManifest = null;
        public AssetBundleManifest AssetBundleManifestObject
        {
            set { m_AssetBundleManifest = value; }
        }

        List<string> m_keysToRemove = new List<string>();
        void Update()
        {
            // Collect all the finished WWWs.
            m_keysToRemove.Clear();
            if (m_DownloadingWWWs != null && m_DownloadingWWWs.Count > 0)
            {
                foreach (var keyValue in m_DownloadingWWWs)
                {
                    WWW download = keyValue.Value.m_www;

                    if (download.isDone)
                    {
                        if (download.error != null)
                        {
                            if (!m_DownloadingErrors.ContainsKey(keyValue.Key))
                                m_DownloadingErrors.Add(keyValue.Key, download.error);
                            else
                                UnityEngine.Debug.LogError(string.Format("m_DownloadingErrors key <{0}> duplicate, error=<{1}>", keyValue.Key, download.error));
                            m_keysToRemove.Add(keyValue.Key);
                            continue;
                        }
                        else
                        {
                            AssetBundle bundle = download.assetBundle;
                            if (bundle != null)
                            {
                                if (!m_LoadedAssetBundles.ContainsKey(keyValue.Key))
                                    m_LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle, keyValue.Value.m_ReferencedCount));
                                else
                                    UnityEngine.Debug.LogError(string.Format("m_LoadedAssetBundles key <{0}> duplicate.>", keyValue.Key));

                                m_keysToRemove.Add(keyValue.Key);
                            }
                            else
                            {
                                string maybeError = "www download failed, download.assetBundle == null, but download.error == null.";
                                if (!m_DownloadingErrors.ContainsKey(keyValue.Key))
                                    m_DownloadingErrors.Add(keyValue.Key, maybeError);

                                UnityEngine.Debug.LogError(string.Format("WWW download assetbundle is null, key <{0}>, error=<{1}>", keyValue.Key, download.error));
                                m_keysToRemove.Add(keyValue.Key);
                            }
                        }
                    }
                }
            }

            //Remove the finished WWWs.
            for (int i = 0; i < m_keysToRemove.Count; i++)
            {
                string key = m_keysToRemove[i];
                WWW download = m_DownloadingWWWs[key].m_www;
                m_DownloadingWWWs.Remove(key);
                download.Dispose();
            }

            // Update all in progress operations
            for (int i = 0; i < m_InProgressOperations.Count; )
            {
                if (!m_InProgressOperations[i].Update())
                {
                    m_InProgressOperations.RemoveAt(i);
                }
                else
                    i++;
            }
        }

        public IEnumerator Init()
        {
            m_AssetBundleManifest = null;

            string manifestPath = IcConfig.GetPlatform();
            int version = 1;//AssetBundleManager.GetResources().GetRunningResourceVer();

            // Set base downloading url.
            m_BaseDownloadingWWWURL = IcResources.GetPersistentDataPath_WWW();//GetPersistentDataPath_WWW();
            m_BaseDownloadingFSURL = IcResources.GetPersistentDataPath_FileStream();

            //// Set base downloading url.
            //string relativePath = GetRelativePath();
            //m_BaseDownloadingURL = relativePath + kAssetBundlesPath + platformFolderForAssetBundles + "/";

            // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
            var request = Initialize(manifestPath, version);
            if (request != null)
                yield return StartCoroutine(request);

            UnloadAssetBundle(manifestPath, false);//add by xlm
        }

        // 加载 AssetBundleManifest.
        public AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName, int version)
        {
            LoadAssetBundle(manifestAssetBundleName, version, ThreadPriority.High, true);
            var operation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
            m_InProgressOperations.Add(operation);
            return operation;
        }

        protected void LoadAssetBundle(string assetBundleName, int version, ThreadPriority tp, bool isLoadingAssetBundleManifest = false)
        {
            //if (!isLoadingAssetBundleManifest)
            //    assetBundleName = RemapVariantName (assetBundleName);

            // Check if the assetBundle has already been processed.
            bool bLoadDependency = LoadAssetBundleInternal(assetBundleName, version, tp, isLoadingAssetBundleManifest);

            // Load dependencies.
            if (bLoadDependency && !isLoadingAssetBundleManifest)//修改by xlm
                LoadDependencies(assetBundleName, tp);
        }

        // Where we actuall call WWW to download the assetBundle.\
        //return value: true 需要loaddependency, false 不需要(只有在已经处于错误列表里的时候不需要)
        protected bool LoadAssetBundleInternal(string assetBundleName, int version, ThreadPriority tp, bool isLoadingAssetBundleManifest)
        {
            string error;
            if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
            {
                return false;
            }

            // Already loaded.
            LoadedAssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                bundle.m_ReferencedCount++;
                return true;
            }

            //// @TODO: Do we need to consider the referenced count of WWWs?
            //// In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
            //// But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
            //if (m_DownloadingWWWs.ContainsKey(assetBundleName) )
            //    return true;

            LoadingWWW www = null;
            m_DownloadingWWWs.TryGetValue(assetBundleName, out www);
            if (www != null)
            {
                if (www.m_www != null && www.m_www.threadPriority < tp)
                    www.m_www.threadPriority = tp;

                www.m_ReferencedCount++;
                return true;
            }

            //if (!IcConfig.UNCOMPRESS_ASSETBUNDLE)//version 5.0.4 CreateFromFile 会导致失败 "Error while reading AssetBundle header!", 但www不会出该错
            if (false)
            {
                //WWW download = null;
                string url;
                if (version == 0)
                    url = m_BaseDownloadingWWWURL + assetBundleName;
                else
                    url = m_BaseDownloadingWWWURL + assetBundleName;

                //#if UNITY_EDITOR
                //IcLog.Log(string.Format("Start LoadAssetBundleInternal:{0},  ver={1}", assetBundleName, version));
                //#endif

                //// For manifest assetbundle, always download it as we don't have hash for it.
                //if (isLoadingAssetBundleManifest || m_AssetBundleManifest == null)
                //    download = new WWW(url);
                //else
                //{
                //    Hash128 h = m_AssetBundleManifest.GetAssetBundleHash(assetBundleName);
                //    download = WWW.LoadFromCacheOrDownload(url, h, 0);//new WWW(url); //

                //    //download = new WWW(url);
                //}

                //不使用Cache
                WWW download = new WWW(url);
                download.threadPriority = tp;

                m_DownloadingWWWs.Add(assetBundleName, new LoadingWWW(download));
            }
            #region 另外一种加载方式
            else//暂时去掉警告
            {
                string url;
                string assetbundlePath = IcConfig.ASSETS + "/" + assetBundleName;
                if (AssetBundleManager.GetResources().m_mapResourceVer.ContainsKey(assetBundleName))                    
                {
                    url = m_BaseDownloadingFSURL + assetBundleName;
                    if(!File.Exists(url))                    
                        url = IcResources.StreamingLoadFromFilePathURL() + "/" + assetBundleName;
                    
                }
                else
                    url = IcResources.StreamingLoadFromFilePathURL() + "/" + assetBundleName;

                //IcLog.Log(string.Format("Start LoadAssetBundleInternal:{0},  ver={1}", assetBundleName, version));              
              
                //最快的方式, 但是不压缩的话, 资源接近1G, 占用太大
                AssetBundle ab = AssetBundleManager.GetResources().LoadAssetbundleDirectlyReturnAB(url, assetBundleName, typeof(Object));

                //网上有评测说比 WWW.LoadFromCacheOrDownload还要慢, 且是最占用内存的方式, 不采用
                //AssetBundle ab = IcResources.LoadAssetbundle(url);
                if (ab != null)
                {
                    bundle = new LoadedAssetBundle(ab);
                    m_LoadedAssetBundles.Add(assetBundleName, bundle);
                }
                else
                {
                    Debug.LogError("url is not find:" + url);
                }
            }
            #endregion
            return true;
        }

        // Where we get all the dependencies and load them all.
        protected void LoadDependencies(string assetBundleName, ThreadPriority tp)
        {
            if (m_AssetBundleManifest == null)
            {
                UnityEngine.Debug.LogError("Please initialize AssetBundleManifest first.");
                return;
            }

            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = null;

            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
            {
                dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName.ToLower());//path, System.IO.Path.GetFileName(m_assetPath)
                if (dependencies.Length == 0)
                    return;

                m_Dependencies.Add(assetBundleName, dependencies);
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                //if (string.IsNullOrEmpty(dependencies[i]))
                //    continue;

                int version = AssetBundleManager.GetResources().GetAssetbundleVersion(dependencies[i]);
                LoadAssetBundleInternal(dependencies[i], version, tp, false);

            }
        }

        // Unload assetbundle and its dependencies.
        //要保证资源assetBundleName已经load完成或者失败以后再调用
        public void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = true)
        {
            //UnityEngine.Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);
            bool removeDependencies = UnloadAssetBundleInternal(assetBundleName, unloadAllLoadedObjects);
            UnloadDependencies(assetBundleName, removeDependencies, unloadAllLoadedObjects);

            //UnityEngine.Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + assetBundleName);
        }

        protected void UnloadDependencies(string assetBundleName, bool bRemoveDependencies, bool unloadAllLoadedObjects)
        {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency, unloadAllLoadedObjects);
            }

            if (bRemoveDependencies)
                m_Dependencies.Remove(assetBundleName);
        }

        //返回值: 是否需要m_Dependencies.Remove(assetBundleName)
        protected bool UnloadAssetBundleInternal(string assetBundleName, bool unloadAllLoadedObjects)
        {            
            string error;
            LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle == null)//失败 //因为 UnloadAssetBundle() 要保证资源assetBundleName已经load完成或者失败以后再调用//TODO 要把这保证去掉, 很难做到
            {
                m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
                if(bundle == null)                
                    return true;
                
                if (--bundle.m_ReferencedCount == 0)
                {
                    if (bundle.m_AssetBundle != null)
                    {
                        bundle.m_AssetBundle.Unload(unloadAllLoadedObjects);
                    }
                    if (m_LoadedAssetBundles.ContainsKey(assetBundleName))
                        m_LoadedAssetBundles.Remove(assetBundleName);
                    return true;
                }
            }
            else if (--bundle.m_ReferencedCount == 0)
            {
                if (bundle.m_AssetBundle != null)
                {
                    bundle.m_AssetBundle.Unload(unloadAllLoadedObjects);
                }
                if (m_LoadedAssetBundles.ContainsKey(assetBundleName))
                    m_LoadedAssetBundles.Remove(assetBundleName);                  
                return true;
            }

            return false;
        }

        public LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
        {
            if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
                return null;

            LoadedAssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
                return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                //if (string.IsNullOrEmpty(dependency))
                //    continue;

                if (m_DownloadingErrors.TryGetValue(dependency, out error))//fix by xlm
                    return bundle;

                // Wait all the dependent assetBundles being loaded.
                LoadedAssetBundle dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                {
                    return null;
                }                    
            }

            return bundle;
        }

        #region 加载单个资源
        public delegate void OnLoaded(Object asset, string assetPath);
		/// <summary>
		/// 异步加载
		/// </summary>
		/// <returns>The asset async.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="version">Version.</param>
		/// <param name="assetName">Asset name.</param>
		/// <param name="type">Type.</param>
		/// <param name="Loaded">Loaded.</param>
		/// <param name="bHighPriority">If set to <c>true</c> b high priority.</param>
        public IEnumerator LoadAssetAsync(string assetBundleName, int version, string assetName, System.Type type, OnLoaded Loaded, bool bHighPriority = true)
        {
            bHighPriority = true;//都使用比较快的载入, effect之类的太慢了

            if (m_AssetBundleManifest == null)
            {
                UnityEngine.Debug.LogError(string.Format("LoadAssetAsync failed, Please initialize AssetBundleManifest first. asset={0}, ver={1}", assetBundleName, version));
                yield break;
            }

            //UnityEngine.Debug.Log("Start to load " + assetName + " at frame " + Time.frameCount);
            // Load asset from assetBundle.
            AssetBundleLoadAssetOperation request = LoadAssetAsync(assetBundleName, version, assetName, type, bHighPriority);
            if (request == null)
                yield break;           
            if (Loaded != null)
            {
                AssetBundle m_AssetBundle = request.GetAssetBundel();
                Object asset = m_AssetBundle.LoadAsset(assetName, typeof(Object));
                Loaded(asset, assetBundleName);
            }
            //yield return StartCoroutine(request);
                       
            //if (Loaded != null)
            //{
            //    // Get the asset.
            //    Object obj = request.GetAsset<Object>();
            //    Loaded(obj, assetBundleName);
            //}
        }

        protected string m_DownloadingError;
        protected Object m_asset = null;
		/// <summary>
		/// 同步加载
		/// </summary>
		/// <returns>The asset synchronization.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="version">Version.</param>
		/// <param name="assetName">Asset name.</param>
		/// <param name="type">Type.</param>
		/// <param name="asset">Asset.</param>
		/// <param name="bHighPriority">If set to <c>true</c> b high priority.</param>
        public void LoadAssetSynchronization(string assetBundleName, int version, string assetName, System.Type type, out Object asset, out AssetBundle m_AssetBundle, bool bHighPriority = false)
		{
            asset = null;
            m_AssetBundle = null; ;

			if (m_AssetBundleManifest == null)
			{
				UnityEngine.Debug.LogError(string.Format("LoadAssetAsync failed, Please initialize AssetBundleManifest first. asset={0}, ver={1}", assetBundleName, version));
				return;
			}

			//UnityEngine.Debug.Log("Start to load " + assetName + " at frame " + Time.frameCount);

			// Load asset from assetBundle.
			AssetBundleLoadAssetOperation request = LoadAssetAsync(assetBundleName, version, assetName, type, bHighPriority);
			if (request == null)
                return;
			//yield return CResourceManager.Instance StartCoroutine(request);
            
		    // Get the asset.
            m_AssetBundle = request.GetAssetBundel();
            asset = m_AssetBundle.LoadAsset(assetName, typeof(Object));		
		}

        // Load asset from the given assetBundle.
        private AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, int version, string assetName, System.Type type, bool bHighPriority)
        {
            AssetBundleLoadAssetOperation operation = null;

            ThreadPriority tp = bHighPriority ? ThreadPriority.High : ThreadPriority.Normal;
            LoadAssetBundle(assetBundleName, version, tp);
            operation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type, bHighPriority);

            m_InProgressOperations.Add(operation);

            return operation;
        }
        #endregion

        #region 加载场景
        public IEnumerator LoadLevel(string assetBundleName, int version, string levelName, bool isAdditive)
        {
            if (m_AssetBundleManifest == null)
            {
                UnityEngine.Debug.LogError(string.Format("LoadLevel failed, Please initialize AssetBundleManifest first. asset={0}, ver={1}", assetBundleName, version));
                yield break;
            }

            //UnityEngine.Debug.Log("Start to load scene " + levelName + " at frame " + Time.frameCount);

            // Load level from assetBundle.
            AssetBundleLoadOperation request = LoadLevelAsync(assetBundleName, version, levelName, isAdditive);
            if (request == null)
                yield break;
            yield return StartCoroutine(request);

            // This log will only be output when loading level additively.
            //UnityEngine.Debug.Log("Finish loading scene " + levelName + " at frame " + Time.frameCount);
        }

        // Load level from the given assetBundle.
        private AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, int version, string levelName, bool isAdditive)
        {
            AssetBundleLoadOperation operation = null;

            LoadAssetBundle(assetBundleName, version, ThreadPriority.High);
            operation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);

            m_InProgressOperations.Add(operation);

            return operation;
        }

        #endregion

        #region 加载所有的资源列表
        // Load all assets from the given assetBundle.
        public delegate void OnAllAssetsLoaded(Object[] assets, string[] assetNames, string assetPath);
        public IEnumerator LoadAllAssetsAsync(string assetBundleName, int version, OnAllAssetsLoaded Loaded)
        {
            if (m_AssetBundleManifest == null)
            {
                UnityEngine.Debug.LogError(string.Format("LoadAllAssetsAsync failed, Please initialize AssetBundleManifest first. asset={0}, ver={1}", assetBundleName, version));
                yield break;
            }

            // Load all assets from assetBundle.
            AssetBundleLoadAllAssetsOperation request = LoadAllAssetsAsync(assetBundleName, version);
            if (request == null)
                yield break;
            yield return StartCoroutine(request);

            if (Loaded != null)
            {
                // Get the asset.
                Object[] objs = request.GetAllAssets();
                string[] assetNames = request.GetAllAssetNames();
                Loaded(objs, assetNames, assetBundleName);
            }
        }

        // Load all assets from the given assetBundle.
        private AssetBundleLoadAllAssetsOperation LoadAllAssetsAsync(string assetBundleName, int version)
        {
            AssetBundleLoadAllAssetsOperation operation = null;

            LoadAssetBundle(assetBundleName, version, ThreadPriority.High);
            operation = new AssetBundleLoadAllAssetsOperation(assetBundleName);

            m_InProgressOperations.Add(operation);

            return operation;
        }
        #endregion
    }
}
