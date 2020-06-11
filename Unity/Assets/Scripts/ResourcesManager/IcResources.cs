using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Mga
{
    public enum eEntityType
    {
        none = 0,
        role = 1,
        eventTrigger = 2,
    }

    public enum eResourceProcessType
    {
        general = 1,
        ui = 2,//atlasRef, fontRef
    }

    //refs of Resources folder asset
    public class IcResources : MonoBehaviour
    {
        [HideInInspector]
        public List<string> m_loadAb = new List<string>();
        Dictionary<string, AudioClip> m_AudioClipCache = new Dictionary<string, AudioClip>();
        [HideInInspector]
        public Dictionary<string, int> m_mapResourceVer = new Dictionary<string, int>();        
        Dictionary<string, Object> uiPrefabs = new Dictionary<string, Object>();
        int m_iResourceVer = 0;
        int m_ipkgVer = 0;
        int m_assetbundleVersion = 0;//更新完成的最新的版本号

        [HideInInspector]
        public int m_downloadingVersion = 0;//正在更新中的版本号

        [HideInInspector]
        public int m_downloadedCount = 0;//更新完成(下载及解压)的AB ID

        [HideInInspector]
        public int m_downloadTxtVersion = 0;//当前表格版本号

        [HideInInspector]
        public int m_downloadedZipCount = 0;//更新完成(下载及解压)的zip数(@正在更新中的版本号)
        [HideInInspector]
        public bool isFirstDownLoad = false;//是否第一次下载资源包

        public static string GetEditorResourcePath(string dir, string name, string extension)
        {
            string path = dir + name;
            return path;
        }

		public static string GetEditorAssetResourcePath(string dir, string name, string extension)
		{
            string path = IcConfig.PathAssetResources + dir + name + "." + extension;
			return path;
		}     
  
        public AudioClip GetAudioClip(string name)
        {
            if (m_AudioClipCache.ContainsKey(name))
                return m_AudioClipCache[name];
            else
                return null;
        }

        public void AddAudioClip(string name ,AudioClip audio)
        {
            if (!m_AudioClipCache.ContainsKey(name))
                m_AudioClipCache.Add(name, audio);
        }

        #region 读取Txt或资源

        public Object GetUIPrefab(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            Object retVal = null;
            uiPrefabs.TryGetValue(typeName, out retVal);
            return retVal;
        }

        public static byte[] LoadTextFileByte(string url)
        {
            System.IO.FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream(url, System.IO.FileMode.Open);
                long lengh = fs.Length;
                byte[] bytes = new byte[lengh];
                fs.Read(bytes, 0, (int)lengh);
                fs.Close();
                fs = null;
                return bytes;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("LoadTextFileByte " + url + " failed. Exception = " + e);

                if (fs != null)
                    fs.Close();
            }
            return null;
        }
        public string GetTxtFile(string relativePath)
        {
            return GetTxtContent(relativePath, "txt");
        }
        private byte[] LoadLatestResource(string relativePath, string fileExtention)
        {
            return null;
        }
        private string GetTxtContent(string relativePath, string extention)
        {
            string content = null;
            byte[] bytes = LoadLatestResource(relativePath, extention);
            if (bytes != null)
            {
                content = System.Text.Encoding.UTF8.GetString(bytes);
            }
            else
            {
                TextAsset asset = Load<TextAsset>(relativePath, extention);

                if (asset == null)
                {
                    UnityEngine.Debug.LogError(string.Format("{0} not found", relativePath));
                }
                else
                {
                    content = asset.text;
                    //Resources.UnloadAsset(asset);
                }
            }

            return content;
        }

        public TextAsset GetTextAsset(string relativePath, string fileExtention="txt")
        {         

            TextAsset asset = Load<TextAsset>(relativePath, fileExtention);

            if (asset == null)
            {
                UnityEngine.Debug.LogError(string.Format("{0} not found", relativePath));
            }

            return asset;
        }

        public IcCSVFile GetCSVAsset(string szInfo)
        {
            IcCSVFile csv = new IcCSVFile(szInfo);

            return csv;
        }

        public IcCSVFile GetCSVFile(string relativePath, bool bSpecial = false)
        {
            IcCSVFile csv = null;
            //csv = MyTxtResources.GetTmpFile(relativePath);
            //if (csv != null)
            //    return csv;

            string fileExtention = "txt";

            TextAsset asset = Load<TextAsset>(relativePath, fileExtention, bSpecial);

            if (asset == null)
            {
                UnityEngine.Debug.LogError(string.Format("{0} not found", relativePath));
            }
            else
            {
                csv = new IcCSVFile(asset, relativePath);
                //Resources.UnloadAsset(asset);
            }
            // }

            return csv;
        }

        //bSpecial : 非编辑器模式, alaways load from resources
        private T Load<T>(string relativePath, string fileExtention, bool bSpecial = false) where T : UnityEngine.Object
        {
            T asset = default(T);

            //#if UNITY_EDITOR
            //        if (AssetBundleManager.GetResources().IsLoadAssetBundle())
            //        {
            //#endif
            if (bSpecial)
            {   //只从Resources里读取, 用于gameConfig
                //load from resource
                asset = Resources.Load(relativePath, typeof(T)) as T;
            }
            else
            {
                string dir;
                string name;
                CUtil.SplitPath(relativePath, out dir, out name);

                stResourcePath resourcePath = GetResourcePath(dir, name, fileExtention);

                if (!AssetBundleManager.Instance.m_isLocaAsset)
                {
                    if (resourcePath.asset != null)
                    {
                        //(assetbundle 已经异步load好了)
                        asset = resourcePath.asset as T;
                    }
                    else if (resourcePath.bAssetbundle)
                    {
                        //load assetbundle
                        //同步load的, 不支持异步assetbundleload, 应该提前load好, 由resourcePath.asset返回    

                        //使用CreateFromMemoryImmediate和createFromFile都会引起crash (unity3d, 5.0.4)
                        //Unity5.4以上使用AssetBundle.LoadFromFile
                        asset = LoadAssetbundle<T>(resourcePath.path, resourcePath.ver, name);
                    }
                    else
                    {
                        //load from resource
                        asset = Resources.Load(resourcePath.path, typeof(T)) as T;
                    }
                }
                else
                {

                    #if UNITY_EDITOR
                    if (asset == null)
                    {                         
                        string path = IcResources.GetEditorAssetResourcePath(dir, name, fileExtention);
                        asset = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                    }
                    #endif
                }
                
            }

            return asset;
        }
        #endregion

        #region sync load
        private T LoadAssetbundle<T>(string assetBundleName, int version, string assetName) where T : UnityEngine.Object
        {
            T asset = default(T);
            string url = StreamingLoadFromFilePathURL() + "/" + version.ToString() + "/" + assetBundleName;

            AssetBundle bundle = LoadAssetbundleFromFile(url); //LoadAssetbundle(url);
            if (bundle != null)
            {
                asset = bundle.LoadAsset<T>(assetName);

                bundle.Unload(false);
            }

            return asset;
        }
        #endregion

        #region 资源读取路径

        /// <summary>
        /// WWWW 读取StreamingAssets的路径
        /// </summary>
        /// <returns></returns>
        public static string StreamingAssetsPathURL()
        {
            string pathURL = "";
#if UNITY_ANDROID && !UNITY_EDITOR
           pathURL = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
	        return "file:///" + Application.streamingAssetsPath+"/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            pathURL = "file://" + Application.dataPath + "/StreamingAssets/";
#else
                string.Empty;
#endif
            return pathURL;
        }

        /// <summary>
        ///AssetBundle.LoadFromFile读取的路径
        /// </summary>
        /// <returns></returns>
        public static string StreamingLoadFromFilePathURL()
        {
            string pathURL = "";
#if UNITY_ANDROID && !UNITY_EDITOR
			pathURL=Application.dataPath + "!assets";   // 安卓平台
#elif UNITY_IPHONE && !UNITY_EDITOR
            pathURL = Application.streamingAssetsPath;  // IOS平台
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            pathURL = Application.streamingAssetsPath;  // win平台
#endif
            return pathURL;
        }

        public static void WriteTextFileSync(string url, string content)
        {
            try
            {
                using (System.IO.StreamWriter sr = new System.IO.StreamWriter(url))
                {
                    sr.Write(content);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(string.Format("WriteTextFileSync failed. /n url={0}, /n Exception = {1}", url, e));
            }
        }

        static string m_persistentDataPath_WWW = null;
        public static string GetPersistentDataPath_WWW()
        {
            if (m_persistentDataPath_WWW == null)
            {
                try
                {
                    string fullPath = Application.persistentDataPath + "/";
                    System.Uri fileUri = new System.Uri(fullPath);
                    m_persistentDataPath_WWW = fileUri.ToString();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return m_persistentDataPath_WWW;
        }

        static string m_persistentDataPath_FileStream = null;
        public static string GetPersistentDataPath_FileStream()
        {
            if (m_persistentDataPath_FileStream == null)
            {
                string path = GetPersistentDataPath_WWW();
                if (!string.IsNullOrEmpty(path))
                {
                    if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                    {
                        m_persistentDataPath_FileStream = path.Replace("file:///", "");
                    }
                    else
                    {
                        m_persistentDataPath_FileStream = path.Replace("file://", "");
                    }
                }
            }

            return m_persistentDataPath_FileStream;
        }
        public static string LoadTextFileSync(string url)
        {
            string content = null;
            try
            {
                if (System.IO.File.Exists(url))
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(url))
                    {
                        content = sr.ReadToEnd();
                    }
                }
                // else
                // {
                //    IcLog.LogError("file not found. " + url);
                // }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            return content;
        }

        public static string GetDataPath_WWW(string platform)
        {
            string path = null;
            try
            {
                string dataPath = Application.dataPath;
                int pos = dataPath.LastIndexOf('/');//如果找到该字符，则为 value 的索引位置；否则如果未找到，则为 -1。 
                if (pos != -1)
                    dataPath = dataPath.Remove(pos);

                string fullPath = dataPath + "/" + "AssetBundles/" + platform + "/";

                System.Uri fileUri = new System.Uri(fullPath);
                path = fileUri.ToString();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }


            return path;
        }

        //GetDataPath_FileStream 在iOS设备上无权限
        public static string GetDataPath_FileStream(string platform)
        {
            string path = GetDataPath_WWW(platform);
            if (!string.IsNullOrEmpty(path))
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    path = path.Replace("file:///", "");
                }
                else
                {
                    path = path.Replace("file://", "");
                }
            }

            return path;
        }
        #endregion

        #region Asset加载到map

        public bool IsLoadAssetBundle()
        {
            return m_assetbundleVersion > m_ipkgVer;
        }
        public IEnumerator InitResourcePath()
        {
            yield return StartCoroutine(GetAssetbundleVer());

            //if (IsLoadAssetBundle())
            //{
            //    yield return StartCoroutine(ConstructResourcePathMap());
            //}
        }

        #region 获取版本号

        //返回 m_ipkgVer, m_assetbundleVersion之中比较大的那个, 用于更新资源
        public int GetResourceVer()
        {
            return m_assetbundleVersion > m_ipkgVer ? m_assetbundleVersion : m_ipkgVer;
        }

        public int GetAssetbundleVersion(string assetbundlePath)
        {
            int ver = 0;
            if (!m_mapResourceVer.TryGetValue(assetbundlePath, out ver))
            {
                ver = 0;
            }
            return ver;
        }       

        //assetbundle文件夹(更新完成的最新的版本)
        // persistentDataPath/vertion.txt
        IEnumerator GetAssetbundleVer()
        {
            m_assetbundleVersion = 0;
            string url = GetPersistentDataPath_WWW() + IcConfig.VERSION_TXT;           

            WWW www = new WWW(url);
            www.threadPriority = ThreadPriority.High;
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                string content = www.text;
                if (!string.IsNullOrEmpty(content))
                {
                    string[] nums = content.Split('|');
                    if (nums != null && nums.Length >= 3)
                    {
                        m_assetbundleVersion = CUtil.ParseInt(nums[0]);

                        m_downloadingVersion = CUtil.ParseInt(nums[1]);//正在更新中的版本号
                        m_downloadedCount = CUtil.ParseInt(nums[2]);//更新完成(下载及解压)的zip数(@正在更新中的版本号)
                    }
                }

                //protect
                if (m_assetbundleVersion < 0)
                {
                    m_assetbundleVersion = 0;
                    UnityEngine.Debug.LogError("assetbundleVersion < 0");
                }

                if (m_downloadingVersion <= 0)
                {
                    m_downloadingVersion = 0;
                    m_downloadedCount = 0;
                }

                if (m_downloadedCount <= 0)
                {
                    m_downloadingVersion = 0;
                    m_downloadedCount = 0;
                }
            }
            else
            {
                Debug.Log("Load Assetbundle version description 'version.txt' failed /n" + www.error);
                isFirstDownLoad = true;
            }


            string urlZip = GetPersistentDataPath_WWW() + IcConfig.ZIP_VERSION_TXT;
            WWW wwwZip = new WWW(urlZip);
            wwwZip.threadPriority = ThreadPriority.High;
            yield return wwwZip;

            if (string.IsNullOrEmpty(wwwZip.error))
            {
                string content = wwwZip.text;
                if (!string.IsNullOrEmpty(content))
                {
                    m_downloadedZipCount = CUtil.ParseInt(content);
                    if (m_downloadedZipCount != 0)
                    {
                        m_downloadedCount = m_downloadedZipCount;
                        isFirstDownLoad = true;
                    }                                          
                }
            }
            else
            {
                Debug.Log("Load Assetbundle version description 'zipVersion.txt' failed /n" + www.error);
                isFirstDownLoad = true;
            }

            string urlTxt = GetPersistentDataPath_WWW() + IcConfig.TxtVer;
            WWW wwwTxt = new WWW(urlTxt);
            wwwTxt.threadPriority = ThreadPriority.High;
            yield return wwwTxt;
            if (string.IsNullOrEmpty(wwwTxt.error))
            {
                string content = wwwTxt.text;
                if (!string.IsNullOrEmpty(content))
                {
                    m_downloadTxtVersion = CUtil.ParseInt(content);
                }
            }
            else
            {
                Debug.Log("Load Assetbundle version description 'txtVersion.txt' failed /n" + www.error);
                m_downloadTxtVersion = 0;
            }
        }
        #endregion

        #region 将资源添加到MAP
        private void AddPackFiles(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string[] name_list = content.Split(':');
                if (name_list != null && name_list.Length >= 2)
                {
                    string bundleName = name_list[0];
                    string assetsList = name_list[1];
                    if (!string.IsNullOrEmpty(bundleName) && !string.IsNullOrEmpty(assetsList))
                    {
                        //check是否有资源
                        int ver = 0;
                        int specialVer = -1;

                        //针对bundleName, 设置specialVer, 在GetResourcePath对各个specialVer做处理
                        if (bundleName == IcConfig.NORMAL_TXT)
                        {
                            specialVer = -2;
                        }
                        else
                        {
                            //bundleName == IcConfig.SCENE_TXT
                            specialVer = -1;
                        }

                        //
                        string[] fileList = assetsList.Split('|');
                        if (fileList != null && fileList.Length > 0)
                        {
                            for (int i = 0; i < fileList.Length; i++)
                            {
                                if (!m_mapResourceVer.ContainsKey(fileList[i]))
                                {
                                    m_mapResourceVer.Add(fileList[i], specialVer);
                                }
                                else
                                {
                                    UnityEngine.Debug.LogError(string.Format("{0} duplicate in {1}, ver = {2}.", fileList[i], bundleName, ver));
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

        #region 资源同步加载加载 syncload_assets
        public IEnumerator LoadSyncLoadAssets()
        {
            //#if UNITY_EDITOR
            //        if (AssetBundleManager.GetResources().IsLoadAssetBundle())
            //        {
            //#endif
            stResourcePath path = GetResourcePath(string.Empty, IcConfig.SYNCLOAD_ASSETS, string.Empty, false, string.Empty);

            if (path.bAssetbundle)
            {
                yield return StartCoroutine(AssetBundleManager.GetAssetBundleManager().LoadAllAssetsAsync(path.path, path.ver, OnAssetsLoaded));

                // Unload assetBundles.
                AssetBundleManager.GetAssetBundleManager().UnloadAssetBundle(path.path, false);
            }

            //#if UNITY_EDITOR
            //        }
            //#endif
        }

        Dictionary<string, Object> m_preLoadedAssets = new Dictionary<string, Object>();
        private void OnAssetsLoaded(Object[] objs, string[] assetNames, string assetPath)
        {
            if (objs == null || objs.Length < 1 || assetNames == null || assetNames.Length != objs.Length)
            {
                UnityEngine.Debug.LogError("Load " + assetPath + " failed.");
                return;
            }

            for (int i = 0; i < objs.Length; i++)
            {
                Object obj = objs[i];
                string assetName = CUtil.ConvertFileNameForAssetbundle(assetNames[i]);
                if (!string.IsNullOrEmpty(assetName))
                {
                    if (!m_preLoadedAssets.ContainsKey(assetName))
                    {
                        m_preLoadedAssets.Add(assetName, obj);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(string.Format("{0} duplicate in {1}", assetName, "preLoadedAssets"));
                    }
                }
            }
        }
        #endregion

        #region 资源加载 Txt

        public IEnumerator PreloadAfterGameWorldInit()
        {
            //lua
            //IcLuaEngine.ExecLuaFile("otherdata/lua/preload");

            //读入预先载入的assetbundle(txt, 及其它需要同步载入的assetbundle)	
            yield return StartCoroutine(LoadSyncLoadAssets());
        }

        public IEnumerator LoadNormalTxt()
        {
            //#if UNITY_EDITOR
            //        if (AssetBundleManager.GetResources().IsLoadAssetBundle())
            //        {
            //#endif
            UnloadNormalTxt();//LoadNormalTxt() 可能会多次调用, 先清空再load

            stResourcePath path = GetResourcePath(string.Empty, IcConfig.NORMAL_TXT, string.Empty, false, string.Empty);

            yield return StartCoroutine(AssetBundleManager.GetAssetBundleManager().LoadAllAssetsAsync(path.path, path.ver, OnNormalTxtLoaded));
            // Unload assetBundles.
            AssetBundleManager.GetAssetBundleManager().UnloadAssetBundle(path.path, false);           
        }

        Dictionary<string, Object> m_preLoadedTxt = new Dictionary<string, Object>();
        private void OnNormalTxtLoaded(Object[] objs, string[] assetNames, string assetPath)
        {
            if (objs == null || objs.Length < 1 || assetNames == null || assetNames.Length != objs.Length)
            {
                UnityEngine.Debug.LogError("Load " + assetPath + " failed.");
                return;
            }

            for (int i = 0; i < objs.Length; i++)
            {
                Object obj = objs[i];
                string assetName = CUtil.ConvertFileNameForAssetbundle(assetNames[i]);
                if (!string.IsNullOrEmpty(assetName))
                {
                    if (!m_preLoadedTxt.ContainsKey(assetName))
                    {
                        m_preLoadedTxt.Add(assetName, obj);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(string.Format("{0} duplicate in {1}", assetName, "preLoadedTxt"));
                    }
                }
            }
        }
        public void UnloadNormalTxt()
        {
            m_preLoadedTxt.Clear();
        }

        #endregion

        #region 预载入资源 读取syncload_assets和normal_txt

        //直接加载整个资源
        //call without fileExtention
        //只用于极个别的情况, 不好清空asset (不好调用 Resources.UnloadAsset(asset);)
        public Object GetPrefab(string relativePath, string extension)
        {
            return Load<Object>(relativePath, extension);
        }

        /// <summary>
        /// 有资源依赖的情况
        /// </summary>
        /// <returns></returns>
        public Object GetPrefabDepend(string assetPath, string extension = IcConfig.EXTENSION_PREFAB)
        {
            string m_loaderPath = null;
            Object obj = null;
            AssetBundle ab = null;
            if (string.IsNullOrEmpty(assetPath))
                return null;


            string dir;
            string name;
            CUtil.SplitPath(assetPath, out dir, out name);
            stResourcePath resourcePath = AssetBundleManager.GetResources().GetResourcePath(dir, name, extension);

            if (!AssetBundleManager.Instance.m_isLocaAsset)
            {
                if (resourcePath.asset != null)
                {
                    //(assetbundle 已经异步load好了)
                    obj = resourcePath.asset;
                }
                else if (resourcePath.bAssetbundle)
                {
                    string path = resourcePath.path;
                    AssetBundleManager.GetAssetBundleManager().LoadAssetSynchronization(path, resourcePath.ver, name, typeof(Object), out obj, out ab);
                    if (!m_loadAb.Contains(resourcePath.path) && ab != null)
                        m_loadAb.Add(resourcePath.path);
                }
                else
                {

                    string path = resourcePath.AssetPath;
                    AssetBundleManager.GetAssetBundleManager().LoadAssetSynchronization(path, resourcePath.ver, name, typeof(Object), out obj, out ab);
                    if (!m_loadAb.Contains(resourcePath.path) && ab != null)
                        m_loadAb.Add(resourcePath.path);
                }
            }
            else
            {
                string path = IcResources.GetEditorAssetResourcePath(dir, name, extension);
                #if UNITY_EDITOR                
                obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));                                
                #endif

                if (obj == null)
                {
                    //同步
                    path = IcResources.GetEditorResourcePath(dir, name, extension);
                    obj = Resources.Load(path, typeof(Object));
                }
            }
            return obj;
        }

        public stResourcePath GetResourcePath(string dir, string name, string extension, bool bLevel = false, string pathPrifix = IcConfig.PathAssetResources)
        {
            bool bAssetbundle = false;
            string path = null;
            Object asset = null;
            int ver = 0;

            string assetbundlePath = pathPrifix + dir + name;
            if (!string.IsNullOrEmpty(extension))
                assetbundlePath += "_" + extension;

            assetbundlePath = assetbundlePath.ToLower();//打出去的assetbundle路径是小写
            if (dir.Contains("TableBinary"))
                assetbundlePath = CUtil.ConvertFileNameForAssetbundle(assetbundlePath);
            else
                assetbundlePath = CUtil.ConvertFileNameForAssetbundleMd5(assetbundlePath);

            if (m_mapResourceVer.TryGetValue(assetbundlePath, out ver))
            {
                bAssetbundle = true;
                path = assetbundlePath;//assetbundle资源路径

                //specialVer的特殊处理
                if (ver < 0)
                {
                    if (ver == -2)
                    {
                        if (!m_preLoadedTxt.TryGetValue(assetbundlePath, out asset))
                        {
                            UnityEngine.Debug.LogError(assetbundlePath + " not found in preLoadedSceneTxt.");
                        }
                    }
                    //todo other ver process...
                    else
                    {
                        //others
                        //ver == -1
                        if (!m_preLoadedAssets.TryGetValue(assetbundlePath, out asset))
                        {
                            UnityEngine.Debug.LogError(assetbundlePath + " not found in preLoaded resource.");
                        }
                    }
                }
            }
            else
            {
                bAssetbundle = false;
                if (bLevel)
                {
                    path = name;//场景, 直接返回name就行了
                }
                else
                {
                    //The path is relative to any Resources folder inside the Assets folder of your project
                    //, extensions must be omitted
                    path = dir + name;//Resource里的路径
                }
            }

            return new stResourcePath(path, bAssetbundle, ver, assetbundlePath, asset);
        }

        #endregion

        #region 资源管理
        Dictionary<string, stLoadActor> m_strAllLoad = new Dictionary<string, stLoadActor>();
        public bool UnLoadAssetbundleDirectly(string assetName)
        {
            stLoadActor pLoadInfo = null;
            if (m_strAllLoad.TryGetValue(assetName, out pLoadInfo))
            {
                if (pLoadInfo != null)
                {
                    pLoadInfo.m_iCount--;
                    if (pLoadInfo.m_iCount <= 0)
                    {


                        if (pLoadInfo.pAsset != null)
                        {
                            pLoadInfo.pAsset.Unload(false);
                        }

                        m_strAllLoad.Remove(assetName);


                        pLoadInfo.pObj = null;

                    }
                }
                return true;
            }
            return false;
        }

        public AssetBundle LoadAssetbundleDirectlyReturnAB(string path, string assetName, System.Type type)
        {
            //  if (ajfs != null)
            //     return ajfs;

            stLoadActor pLoadInfo = null;
            if (m_strAllLoad.TryGetValue(path, out pLoadInfo))
            {
                if (pLoadInfo != null)
                {
                    pLoadInfo.m_iCount++;
                    return pLoadInfo.pAsset;
                }
                return null;
            }

            AssetBundle ab = IcResources.LoadAssetbundleFromFile(path);
            if (ab != null)
            {
                Object ajfs = ab.LoadAsset(assetName, type);
                if (ajfs != null)
                {
                    stLoadActor pInfo = new stLoadActor();
                    pInfo.assetName = assetName;
                    pInfo.path = path;
                    pInfo.pObj = ajfs;
                    pInfo.m_iCount = 1;
                    pInfo.pAsset = ab;
                    m_strAllLoad[path] = pInfo;

                }
                return ab;
            }
            return null;
        }
       
        public Object LoadAssetbundleDirectly(string path, string assetName, System.Type type)
        {
            //  if (ajfs != null)
            //     return ajfs;

            stLoadActor pLoadInfo = null;
            if (m_strAllLoad.TryGetValue(path, out pLoadInfo))
            {
                if (pLoadInfo != null)
                {
                    pLoadInfo.m_iCount++;
                    return pLoadInfo.pObj;
                }
                return null;
            }

            AssetBundle ab = IcResources.LoadAssetbundleFromFile(path);
            if (ab != null)
            {
                Object ajfs = ab.LoadAsset(assetName, type);
                if (ajfs != null)
                {
                    stLoadActor pInfo = new stLoadActor();
                    pInfo.assetName = assetName;
                    pInfo.path = path;
                    pInfo.pObj = ajfs;
                    pInfo.m_iCount = 1;
                    pInfo.pAsset = ab;
                    m_strAllLoad[path] = pInfo;

                }
                return ajfs;
            }
            return null;

        }

        public static AssetBundle LoadAssetbundleFromFile(string url)//LoadUncompressedAssetbundle
        {
            //old: Only uncompressed asset bundles are supported by this function. This is the fastest way to load an asset bundle.
            //return AssetBundle.CreateFromFile(url);

            //from unity5.3
            //The function supports bundles of any compression type. In case of lzma compression, the data will be decompressed to the memory.
            //Uncompressed and chunk-compressed bundles can be read directly from disk.
            //打算用于lz4的

            AssetBundle bundle = null;
            
            bundle = AssetBundle.LoadFromFile(url);
            
            if (bundle == null)
            {
                return null;                
            }

            return bundle;
        }
        private static AssetBundle LoadAssetbundleFromMemory(string url)
        {
            AssetBundle bundle = null;
            System.IO.FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream(url, System.IO.FileMode.Open);
                long lengh = fs.Length;
                byte[] bytes = new byte[lengh];
                fs.Read(bytes, 0, (int)lengh);
                fs.Close();
                fs = null;

                bundle = AssetBundle.LoadFromMemory(bytes);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("LoadAssetbundleSync " + url + " failed. Exception = " + e);

                if (fs != null)
                    fs.Close();
            }

            return bundle;
        }

        public void LoadAssetbundleScene(string path, string levelName, bool isAdditive)
        {

            UnLoadAssetbundleDirectly(path);

            AssetBundle ab = IcResources.LoadAssetbundleFromFile(path);
            if (ab != null)
            {
                //if (isAdditive)
                //    Application.LoadLevelAdditive(levelName);
                //else
                //    Application.LoadLevel(levelName);

                if (isAdditive)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(levelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(levelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
                }

                stLoadActor pInfo = new stLoadActor();
                pInfo.assetName = levelName;
                pInfo.path = path;
                pInfo.pObj = null;
                pInfo.m_iCount = 1;
                pInfo.pAsset = ab;
                m_strAllLoad[path] = pInfo;
            }
            else
            {
                UnityEngine.Debug.LogError("LoadAssetbundleSceneDirectly failed. levelName=" + levelName);
            }

        }
        #endregion

        #region 资源更新
        public void UpdateAssetbundleVerInfo(int downloadedVer, int downloadingVer, int downloadedCount)
        {
            if (downloadedVer > 0)
                m_assetbundleVersion = downloadedVer;

            m_downloadingVersion = downloadingVer;
            m_downloadedCount = downloadedCount;

            // Save
            string content = string.Format("{0}|{1}|{2}", m_assetbundleVersion, m_downloadingVersion, m_downloadedCount);
            string url = GetPersistentDataPath_FileStream() + IcConfig.VERSION_TXT;
            WriteTextFileSync(url, content);

            string urlZip = GetPersistentDataPath_FileStream() + IcConfig.ZIP_VERSION_TXT;
            string zipCount= m_downloadedZipCount.ToString();
            WriteTextFileSync(urlZip, zipCount);

            string urlTxt = GetPersistentDataPath_FileStream() + IcConfig.TxtVer;
            string txtVersion = m_downloadTxtVersion.ToString();
            WriteTextFileSync(urlTxt, txtVersion);
        }
        #endregion

    }

    public struct stResourcePath
    {
        //asset: "assets/assetresources/syncload/audio/music/back_1.mp3"
        //relativePath: /syncload/audio/music/
        //name: back_1
        //extension: mp3

        public stResourcePath(string path, bool bAssetbundle, int ver, string assetPath,Object asset = null)
        {
            this.AssetPath = assetPath;
            this.path = path;
            this.bAssetbundle = bAssetbundle;
            this.ver = ver;
            this.asset = asset;
        }

        public bool bAssetbundle;//是不是assetbundle, 不是的话直接使用Resources.Load, 并且path为null
        public Object asset;//特殊处理类型, 缓存着的资源就直接返回资源了
        public string AssetPath;
        public string path;
        public int ver;//assetbundle情况下的版本号
    }
    public class stLoadActor
    {
        public string path;
        public string assetName;
        public Object pObj = null;
        public AssetBundle pAsset = null;

        public int m_iCount = 0;
    };
}