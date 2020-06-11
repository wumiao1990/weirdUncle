using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Spine.Unity;
using Spine.Unity.Editor;
using System.IO;

namespace Spine
{
    public class SplitAnimationWindow : EditorWindow
    {
        [MenuItem("Xiimoon/Tools/SplitAnimationWindow")]
        public static void OpenToolsBox()
        {
            SplitAnimationWindow _window = EditorWindow.GetWindow<SplitAnimationWindow>("Spine动画拆分工具", true);
            _window.GetConfig();
            assetPaths = new List<string>();
        }

        [MenuItem("Assets/Spine/SplitAnimation")]
        public static void Split()
        {
            UnityEngine.Object _activeObject = Selection.activeObject;
            if (_activeObject.GetType() == typeof(SkeletonDataAsset))
            {
                string _path = AssetDatabase.GetAssetPath(_activeObject);

                SpineXiimoonConfig _config = AssetDatabase.LoadAssetAtPath(configPath, typeof(SpineXiimoonConfig)) as SpineXiimoonConfig;
                if (_config == null) return;

                string exportPath = GetAnimationExportPath(_path, _config);
                if (string.IsNullOrEmpty(exportPath)) return;
                _config = null;

                SplitAnimationTool.DoSplit(_path, exportPath, 0);
            }
        }

        string animExportPath = string.Empty;
        bool backup = true;

        private Vector2 scrollPosition;
        private SpineXiimoonConfig _config;
        const string configPath = "Assets/Spine/spine-xiimoon/xiimoonconfig.asset";
        public void GetConfig()
        {
            if (_config == null)
            {
                _config = AssetDatabase.LoadAssetAtPath(configPath, typeof(SpineXiimoonConfig)) as SpineXiimoonConfig;
                if (_config == null)
                {
                    _config = SkeletonDataAsset.CreateInstance<SpineXiimoonConfig>();
                    AssetDatabase.CreateAsset(_config, configPath);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(600));
            GUILayout.Space(10);

            GUILayout.Label("操作的路径：");
            for (int i = 0; i < _config.dopaths.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("操作路径：" + (i + 1) + "    " + _config.dopaths[i] + " >>> 动画导出到 >>> " + GetPathExportPath(_config.dopaths[i]));
                if (GUILayout.Button("Remove"))
                {
                    _config.dopaths.RemoveAt(i);
                    EditorUtility.SetDirty(_config);
                    AssetDatabase.SaveAssets();
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("添加目录"))
            {
                string _selectPath = EditorUtility.OpenFolderPanel("选择要添加的目录", "Assets/NScene/imported", string.Empty);
                if (!string.IsNullOrEmpty(_selectPath))
                {
                    _selectPath = "Assets" + _selectPath.Replace(Application.dataPath, "");
                    if (!_config.dopaths.Contains(_selectPath))
                    {
                        _config.dopaths.Add(_selectPath);
                        EditorUtility.SetDirty(_config);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            if (GUILayout.Button("拆分"))
            {
                assetPaths = new List<string>();
                for (int i = 0; i < _config.dopaths.Count; i++)
                {
                    DirectoryInfo dire = new DirectoryInfo(_config.dopaths[i]);
                    CollectSkeletonDataAssets(dire);
                }
                int totalCount = assetPaths.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    try
                    {
                        SplitAnimationTool.DoSplit(assetPaths[i], GetAnimationExportPath(assetPaths[i], _config), _config.needSplitCount);
                        EditorUtility.DisplayProgressBar(string.Format("拆分动画中... {0}/{1}", i, totalCount), string.Format("{0}", Path.GetFileName(assetPaths[i])), (float)i / (float)totalCount);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogErrorFormat(AssetDatabase.LoadAssetAtPath<Object>(assetPaths[i]), e.ToString());
                    }

                }

                EditorUtility.ClearProgressBar();
            }

            // if (GUILayout.Button("从备份拆分"))
            // {
            //     assetPaths = new List<string>();
            //     for (int i = 0; i < _config.dopaths.Count; i++)
            //     {
            //         DirectoryInfo dire = new DirectoryInfo(_config.dopaths[i]);
            //         CollectSkeletonDataAssets(dire);
            //     }
            //     for (int i = 0; i < assetPaths.Count; i++)
            //     {
            //         SplitAnimationTool.DoSplit(assetPaths[i], GetAnimationExportPath(assetPaths[i], _config), _config.needSplitCount,true);
            //     }
            // }

            // GUILayout.Label("AssetPaths");
            // for (int i = 0; i < assetPaths.Count; i++)
            // {
            //     GUILayout.Label(i + " : " + assetPaths[i] + "   >>>  " + GetAnimationExportPath(assetPaths[i], _config));
            // }


            GUILayout.EndScrollView();
        }

        static List<string> assetPaths = new List<string>();
        /// <summary>
        /// 获取指定目录下所有SkeletonDataAsset，并将其路径添加到assetPaths中
        /// </summary>
        /// <param name="dire"></param>
        static void CollectSkeletonDataAssets(DirectoryInfo dire)
        {
            if (dire.Exists)
            {
                foreach (FileInfo fi in dire.GetFiles("*.asset"))
                {
                    string fullName = fi.FullName.Replace(Path.DirectorySeparatorChar, '/');
                    string relativePath = "Assets" + fullName.Substring(Application.dataPath.Length);
                    SkeletonDataAsset selectObj = AssetDatabase.LoadAssetAtPath(relativePath, typeof(SkeletonDataAsset)) as SkeletonDataAsset;
                    if (selectObj != null)
                    {
                        assetPaths.Add(relativePath);
                    }
                }
                foreach (DirectoryInfo child in dire.GetDirectories())
                    CollectSkeletonDataAssets(child);
            }
        }

        //目录对应规则
        static string GetPathExportPath(string path)
        {
            var dirName = new DirectoryInfo(path).Name;
            var resolver = "";//new Ngame.ABSystem.AssetBundlePathResolver();wumiao

            //return path.Replace("imported", "Resources") + "Animations";
            return "";//resolver.ABResPlayGroundAssetPath + "/spine/" + dirName + "_anims"; wumiao
        }

        static int _splitIndex = -1;
        static string GetAnimationExportPath(string assetPath, SpineXiimoonConfig config)
        {
            var resolver = "";//new Ngame.ABSystem.AssetBundlePathResolver();wumiao

            for (int i = 0; i < config.dopaths.Count; i++)
            {
                if (assetPath.Contains(config.dopaths[i]))
                {
                    string _result = assetPath.Replace(config.dopaths[i], GetPathExportPath(config.dopaths[i]));

                    _splitIndex = _result.LastIndexOf("/");
                    _result = _result.Substring(0, _splitIndex);

                    // get the bundle path
                    // add by adam
                    //Debug.Assert(_result.StartsWith(resolver.ABResPlayGroundAssetPath)); wumiao
                    //_result = _result.Substring(resolver.ABResPlayGroundAssetPath.Length + 1); wumiao

                    return _result;
                }
            }
            return string.Empty;
        }
    }
}