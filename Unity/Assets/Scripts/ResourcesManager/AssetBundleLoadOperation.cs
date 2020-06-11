using UnityEngine;
using System.Collections;

namespace Mga
{
    public abstract class AssetBundleLoadOperation : IEnumerator
    {
        protected bool m_bAsyncOperation = false;
        public object Current
        {
            get
            {
                return null;
            }
        }
        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset()
        {
        }

        abstract public bool Update();

        abstract public bool IsDone();
    }

    public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
    {
        public AssetBundleLoadManifestOperation(string bundleName, string assetName, System.Type type)
            : base(bundleName, assetName, type, true)
        {
        }

        public override bool Update()
        {
            base.Update();

            if (IsDone())
            {
                AssetBundleManifest manifest = GetAsset<AssetBundleManifest>();
                if (manifest != null)
                {
                    AssetBundleManager.GetAssetBundleManager().AssetBundleManifestObject = manifest;
                }
                else
                {
                    UnityEngine.Debug.LogError("load AssetBundleManifest " + m_AssetBundleName + " failed.");
                }

                return false;
            }
            else
                return true;
        }
    }

    public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
    {
        public abstract T GetAsset<T>() where T : UnityEngine.Object;
        public virtual AssetBundle GetAssetBundel()
        {
            AssetBundle ab = null;
            return ab;
        }
    }



    public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
    {
        protected string m_AssetBundleName;
        protected string m_AssetName;
        protected string m_DownloadingError;
        protected System.Type m_Type;
        protected AssetBundleRequest m_Request = null;

        protected Object m_asset = null;//use for bAsyncOperation == true

        public AssetBundleLoadAssetOperationFull(string bundleName, string assetName, System.Type type, bool bHighPriority)
        {
            m_AssetBundleName = bundleName;
            m_AssetName = assetName;
            m_Type = type;
            m_bAsyncOperation = !bHighPriority;
        }

        public override T GetAsset<T>()
        {
            if (m_bAsyncOperation)
            {
                if (m_Request != null && m_Request.isDone)
                {
                    //如果assetbundle被提前unload了, access m_Request.asset 会返回error: (m_Request.asset == null 这样的check不行)
                    //MissingReferenceException: The object of type 'AssetBundle' has been destroyed but you are still trying to access it.												
                    //Your script should either check if it is null or you should not destroy the object.												

                    //return m_Request.asset as T;

                    LoadedAssetBundle bundle = AssetBundleManager.GetAssetBundleManager().GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
                    if (bundle != null && bundle.m_AssetBundle != null)
                        return m_Request.asset as T;
                    else
                        return null;
                }
                else
                {
                    LoadedAssetBundle bundle = AssetBundleManager.GetAssetBundleManager().GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
                    if (bundle != null && bundle.m_AssetBundle != null)
                        return  bundle.m_AssetBundle.LoadAsset(m_AssetName, m_Type) as T;
                    else
                        return null;                    
                }                
            }
            else
            {
                return m_asset as T;
            }
        }

        public override AssetBundle GetAssetBundel()
        {
            LoadedAssetBundle bundle = AssetBundleManager.GetAssetBundleManager().GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null && bundle.m_AssetBundle != null)
                return bundle.m_AssetBundle;
            else
                return null; 
        }

        // Returns true if more Update calls are required.
        public override bool Update()
        {
            if (m_Request != null || m_asset != null)
                return false;

            LoadedAssetBundle bundle = AssetBundleManager.GetAssetBundleManager().GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null && bundle.m_AssetBundle != null)
            {
                if (m_bAsyncOperation)
                {
                    m_Request = bundle.m_AssetBundle.LoadAssetAsync(m_AssetName, m_Type);
                    return false;
                }
                else
                {
                    m_asset = bundle.m_AssetBundle.LoadAsset(m_AssetName, m_Type);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_DownloadingError != null)
            {
                UnityEngine.Debug.LogError("load " + m_AssetBundleName + " failed. error = " + m_DownloadingError);
                return true;
            }

            if (m_bAsyncOperation)
            {
                return m_Request != null && m_Request.isDone;
            }
            else
            {
                return m_asset != null;
            }
        }
    }

    public class AssetBundleLoadAllAssetsOperation : AssetBundleLoadOperation
    {
        protected string m_AssetBundleName;
        protected string m_DownloadingError;
        protected AssetBundleRequest m_Request = null;
        private string[] m_assetNames;

        private Object[] m_assets = null;

        public AssetBundleLoadAllAssetsOperation(string bundleName)
        {
            m_AssetBundleName = bundleName;
        }

        // Returns true if more Update calls are required.
        public override bool Update()
        {
            if (m_Request != null || m_assets != null)
                return false;

            LoadedAssetBundle bundle = AssetBundleManager.GetAssetBundleManager().GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null && bundle.m_AssetBundle != null)
            {
                m_assetNames = bundle.m_AssetBundle.GetAllAssetNames();
                if (m_bAsyncOperation)
                {
                    m_Request = bundle.m_AssetBundle.LoadAllAssetsAsync();
                    return false;
                }
                else
                {
                    m_assets = bundle.m_AssetBundle.LoadAllAssets();
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_DownloadingError != null)
            {
                UnityEngine.Debug.LogError("load " + m_AssetBundleName + " failed. error = " + m_DownloadingError);
                return true;
            }

            if (m_bAsyncOperation)
            {
                return m_Request != null && m_Request.isDone;
            }
            else
            {
                return m_assets != null;
            }
        }

        public Object[] GetAllAssets()
        {
            if (m_bAsyncOperation)
            {
                if (m_Request != null && m_Request.isDone)
                    return m_Request.allAssets;
                else
                    return null;
            }
            else
            {
                return m_assets;
            }
        }

        public string[] GetAllAssetNames()
        {
            return m_assetNames;
        }
    }

    public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
    {
        protected string m_AssetBundleName;
        protected string m_LevelName;
        protected bool m_IsAdditive;
        protected string m_DownloadingError;
        protected AsyncOperation m_Request;

        protected bool m_bDone = false;//use for bAsyncOperation == true

        public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
        {
            m_AssetBundleName = assetbundleName;
            m_LevelName = levelName;
            m_IsAdditive = isAdditive;
        }

        public override bool Update()
        {
            if (m_Request != null || m_bDone)
                return false;

            LoadedAssetBundle bundle = AssetBundleManager.GetAssetBundleManager().GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null)
            {
                if (m_bAsyncOperation)
                {
                    if (m_IsAdditive)
                    {
                        m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    }
                    else
                    {
                        m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
                    }
                    return false;
                }
                else
                {
                    if (m_IsAdditive)
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    }
                    else
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
                    }

                    m_bDone = true;
                    return false;
                }

            }
            else
                return true;
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_DownloadingError != null)
            {
                UnityEngine.Debug.LogError("load " + m_AssetBundleName + " failed. error = " + m_DownloadingError);
                return true;
            }

            if (m_bAsyncOperation)
            {
                return m_Request != null && m_Request.isDone;
            }
            else
            {
                return m_bDone;
            }
        }
    }
}