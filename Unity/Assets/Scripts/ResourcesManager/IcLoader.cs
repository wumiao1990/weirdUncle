using UnityEngine;
using System.Collections;

namespace Mga
{
    public class IcLoader : MonoBehaviour
    {

        //load and init
        //protected long m_loaderId;
        protected Object m_loadedAsset;

        public delegate void LoadedCB(Object asset);
        protected LoadedCB m_loadedCallback;
        protected System.Type m_type;

        protected string m_dir;
        protected string m_name;
        protected string m_extension;

        protected string m_path;

        public Object GetObject()
        {
            return m_loadedAsset;
        }
        public bool loadLowPriority
        {
            get;
            set;
        }

        bool m_bStartLoad = false;
        public void StartLoad() { m_bStartLoad = true; }
        public virtual void DoStartLoad() { }

        void Update()
        {
            if (m_bStartLoad)
            {
                m_bStartLoad = false;
                DoStartLoad();
            }
        }

        //为了适应各种载入方式, 传入参数定义如下
        //asset: "assets/assetresources/syncload/audio/music/back_1.mp3"
        //dir: syncload/audio/music/
        //name: back_1
        //extension: mp3
        public void SetAssetPath(string dir, string name, string extension, System.Type type = null)
        {
            Clear();

            m_dir = dir;
            m_name = name;
            m_extension = extension;
            //m_assetPath = assetPath;//string.Format("{0}/{1}{2}", IcConfig.PathResources, assetPath, fileExtention == null ? "" : fileExtention);
            m_type = type == null ? typeof(Object) : type;
        }

        //relativePath: "syncload/audio/music/back_1.mp3"
        public void SetAssetPath(string relativePath, System.Type type = null)
        {
            Clear();

            CUtil.SplitPath(relativePath, out m_dir, out m_name, out m_extension);
            m_type = type == null ? typeof(Object) : type;
        }

        //relativePath: "syncload/audio/music/back_1" 
        //extension : mp3
        public void SetAssetPath(string relativePath, string extension, System.Type type = null)
        {
            Clear();

            CUtil.SplitPath(relativePath, out m_dir, out m_name, out m_extension);
            m_extension = extension;
            m_type = type == null ? typeof(Object) : type;
        }

        public void SetLoadedCallback(LoadedCB cb)
        {
            m_loadedCallback = cb;
        }

        protected void Loaded(Object asset, string assetPath)
        {
#if ICDEBUG
        if (asset == null)
            IcLog.LogError("Load " + assetPath + " failed.");
#endif

            //m_eInitProcess = eInitProcess.loaded_fail;

            m_loadedAsset = asset;
            //m_eInitProcess = eInitProcess.loaded_success;
            RunCallBack();
        }

        protected void RunCallBack()
        {
            if (m_loadedCallback != null)
            {
                m_loadedCallback(m_loadedAsset);
            }

            //m_eInitProcess = eInitProcess.Initialized;
            //CUtil.Destroy(this);
        }

        void OnDestroy()
        {
            UnloadAssetBundle();
        }

        public void UnloadAssetBundle(bool unloadAllLoadedObjects = true)
        {
            if (m_path != null)
            {
                if (!AssetBundleManager.GetResources().UnLoadAssetbundleDirectly(m_path))
                {
                    AssetBundleManager.GetAssetBundleManager().UnloadAssetBundle(m_path, unloadAllLoadedObjects);
                }

                m_path = null;
            }

            //有一些data, 等scene或者Asset unload的时候再随着gameObject删掉
            //CUtil.Destroy(this);
        }

        #region directly load assetbundle
        public bool m_bTestRole = false;

        //TODO 资源管理
        public void LoadAssetbundleDirectly(string path, string assetName, System.Type type)
        {
            Object asset = AssetBundleManager.GetResources().LoadAssetbundleDirectly(path, assetName, type);
            Loaded(asset, path);
        }

        protected void LoadAssetbundleSceneDirectly(string path, string levelName, bool isAdditive)
        {
            AssetBundleManager.GetResources().LoadAssetbundleScene(path, levelName, isAdditive);
            /*
            //最快的方式, 但是不压缩的话, 资源接近1G, 占用太大
            AssetBundle ab = IcResources.LoadUncompressedAssetbundle(path);
            if (ab != null)
            {
                if (isAdditive)
                    Application.LoadLevelAdditive(levelName);
                else
                    Application.LoadLevel(levelName);
            }
            else
            {
                IcLog.LogError("LoadAssetbundleSceneDirectly failed. levelName=" + levelName);
            }*/
        }

        #endregion

        private void Clear()
        {
            //m_loaderId = 0;
            m_loadedAsset = null;
            m_loadedCallback = null;
            m_type = null;

            m_dir = null;
            m_name = null;
            m_extension = null;
            m_path = null;

            loadLowPriority = false;
        }
    }
}
