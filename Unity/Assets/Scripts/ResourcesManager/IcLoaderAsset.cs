using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mga
{
    public class IcLoaderAsset : IcLoader
    {
        public override void DoStartLoad()//public override void StartLoad()
        {
            if (m_loadedCallback == null)
                return;
            /*
             if (m_name == "fmz2jx_atk0")
             {
                 int iu = 0;
                 iu++;
             }*/
            string strNewFile = string.Empty;

            //if (MyTxtResources.GetTmpActorsFile(m_name, out strNewFile))//m_bTestRole)
            //{
            //    string pathOk = IcResources.GetPersistentDataPath_FileStream() + "actors/" + strNewFile;//IcResources.GetPersistentDataPath_WWW() + assetbundlePath;
            //    m_path = pathOk;
            //    LoadAssetbundleDirectly(pathOk, strNewFile, typeof(Object));

            //    return;
            //}
            //IcChangeFiles pChangeFile = MyTxtResources.GetChangeFile(m_name);
            //if (pChangeFile != null)
            //{
            //    m_name = pChangeFile.fileName;
            //    m_dir = pChangeFile.path;
            //}
            stResourcePath resourcePath = AssetBundleManager.GetResources().GetResourcePath(m_dir, m_name, m_extension);

            if (!AssetBundleManager.Instance.m_isLocaAsset)
            {

                if (resourcePath.asset != null)
                {
                    //(assetbundle 已经异步load好了)
                    Loaded(resourcePath.asset, resourcePath.path);
                }
                else if (resourcePath.bAssetbundle)
                {
                    StartCoroutine(LoadAssetBundle(resourcePath.path, resourcePath.ver, !loadLowPriority));
                }
                else
                {

                    if (loadLowPriority)
                    {
                        StartCoroutine(LoadResourceAssetAsync(resourcePath.path));
                    }
                    else
                    {
                        AssetBundleManifest m_AssetBundleManifest = AssetBundleManager.GetAssetBundleManager().m_AssetBundleManifest;
                        if(m_AssetBundleManifest==null)
                        {
                            Object asset = null;
                            string path = resourcePath.path;
                            asset = Resources.Load(path, typeof(Object));
                            if (asset == null)
                            {
                                Debug.LogError("ResourceLoad Error:" + path);
                            }
                            Loaded(asset, path);
                        }
                        else
                            StartCoroutine(LoadAssetBundle(resourcePath.AssetPath, resourcePath.ver, !loadLowPriority));
                    }
                }
            }
            else
            {
                Object obj = null;

                string path = IcResources.GetEditorAssetResourcePath(m_dir, m_name, m_extension);
#if UNITY_EDITOR
                obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
#endif

                if (obj == null)
                {
                    //同步
                    path = IcResources.GetEditorResourcePath(m_dir, m_name, m_extension);
                    obj = Resources.Load(path, typeof(Object));
                }

                Loaded(obj, path);
            }

        }


        private IEnumerator LoadResourceAssetAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
                yield break;

            ResourceRequest request = Resources.LoadAsync(path, typeof(Object));
            yield return request;

            Object asset = request.asset;

            //#if UNITY_EDITOR
            //        if (asset == null)
            //        {
            //            path = IcResources.GetEditorResourcePath(m_dir, m_name, m_extension);
            //            asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            //        }
            //#endif

            Loaded(asset, path);
        }

        IEnumerator LoadAssetBundle(string path, int version, bool bHighPriority)
        {
            m_path = path;
            yield return StartCoroutine(AssetBundleManager.GetAssetBundleManager().LoadAssetAsync(path, version, m_name, m_type, Loaded, bHighPriority));

            // Unload assetBundles.
            //该类不能作为monobehaviour实现: Loaded里如果有setActive(false)的调用, 使gameObject disable, 下面的UnloadAssetBundle就得不到调用, 导致严重的资源泄漏.  例子reconnection_panel
            //AssetBundleManager.GetAssetBundleManager().UnloadAssetBundle(path);//如果一个texture的assetbundle被www读入了两次, 则使用loadasset后, 内存里面会有两份texture的copy, 所以只能在window close(即texture使用完毕的时候) decrease ref count
        }
    }
}
