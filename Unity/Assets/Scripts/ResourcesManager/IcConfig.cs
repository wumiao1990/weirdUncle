using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IcConfig
{
    public const string PathAssetResources = "Assets/AssetResources/";
    
    public const string PATH_SHARE_PREFEB = "Ui/NewPerfebUI/Share/";
    public const string PATH_LOGIN_PREFEB = "Ui/NewPerfebUI/Login/";
    public const string PATH_LOBBY_PREFEB = "Ui/NewPerfebUI/Lobby/";
    public const string PATH_BATTLE_PREFEB = "Ui/NewPerfebUI/Battle/";
	public const string PATH_LOGO_PREFEB = "Ui/NewPerfebUI/Logo/";
    public const string DIR_UI_WINDOW = "Ui/NewPerfebUI/";

    public const string DIR_UI_SYNCLOAD = "/syncLoad/";
    public const string DIR_CT_WINDOW = "/ControllerPrefab";
    public const string PathResources = "Assets/Resources";
    public const string TableBinary = "TableBinary";
    public const string Language = "Language";
    public const string picCache = "picCache";


	public const string PathAssetBundles = "Assets/AssetBundles";
    public const string PathDebugResources = "Assets/Debug";
    public const string PathEditor = "Assets/Editor";
    public const string PathAssetPreview = "Assets/Editor/AssetPreviews/";
	
	public const string PathConfig = "config/config";
    //public const string PathEditorAssets = "Assets/Editor/EditorAssets";

    //world map
	public static string PathWorldMap = "scenes/bigmap/bigmap";
	public static string PathLoginShow = "scenes/amazing_login/amazing_login";
	//public static string PathTarLvlEffect = "effects/sceneffect/mappointef02";
	//public static string PathTarLandEffect = "effects/sceneffect/landpoint_effect";
    public static int PathTarLvlEffectId = 61;
    public static int PathTarLandEffectId = 62;

	//actor
	public const string PathActor = "actors/";
    public const string PathActorAnimatorInfo = "otherdata/attrres/animator/animatorInfo";

	//datares
	public const string PathDataRes = "otherdata";
	
	//shadow
	public const string EasyShadowPrefab = "effects/common/ShadowPrefab";
	

	//UIText prefab
    public const string UITextPrefab = "syncLoad/syncLoad/UITextPrefab";
	
	//screen text prefab
    public const string ScreenTextPrefab = "syncLoad/syncLoad/screenTextPrefab";

    public const string ScreenTextPrefabEx = "syncLoad/syncLoad/screenTextPrefabEx";
	
	
	//lightmap config
 	public const string PathLightmap = "lightmap";
	
	//file extension
	public const string FileExtAssetBundle = ".unity3d"; //assetbundle file name extension
    public const string FilePostfixStreamedScene = "_streamed";
    public const string FilePostfixServerBlock = "_block";
	
	//file suffix
	public const string FileSuffixResourceRequest = "_IcResourceRequest";
	
	//richtext prefab
	public const string RichTextPrefab = "ui/control/richtext";
	


	//sound
	public const string AudioDirMusic = "syncLoad/audio/music/";
	public const string AudioDirSound = "syncLoad/audio/sound/";
	//public const string SoundUI = "syncLoad/audio/ui/";

	//get award
	public const string GetAward = "ui/control/GetAwardPrefab";
	public const string GetSingerAward = "ui/control/GetAwardPrefab2";

    //rule chekc
    public const string RuleCheck = "ui/control/RuleCheckPrefab";

    // gem
    public const string GemPath = "ui/control/zuanshi";
	
	//version control
#if UNITY_ANDROID
    public const string Version_Update_URL = "http://mgame.game080.com/FMZLM_CS_test/android/update.txt";
#else
    public const string Version_Update_URL = "http://mgame.game080.com/FMZLM_CS_test/ios/update.txt";
#endif

    //runtime animatorController
    public const string RuntimeAnimatorController = "syncLoad/animator/";


    //npcPath
   // public const string PathNPCPath = "otherdata/attrres/npcPath";

    //RenderSettings
    public static Color AmbientLight = new Color(1.0f, 1.0f, 1.0f, 1.0f);//new Color(150f / 255f, 150f / 255f, 150f / 255f, 1.0f);

    //fight role point light (up on head)
    //public static Color PointLight = new Color(255 / 255f, 188 / 255f, 103 / 255f, 1.0f);
	

    //用于一些time的初始化 (Time.time : This is the time in seconds since the start of the game)
    public const float OneDayAgo = -60f * 60f * 24f;//sec 

    //map id
    public const int FirstLoginShow_Map_ID = 2001;//1000;//999;


    //新的assetbundle打包相关的一些定义
    public const bool UNCOMPRESS_ASSETBUNDLE = false;


    public static bool PRELOAD_ASSETBUNDLE = false;//s!UNCOMPRESS_ASSETBUNDLE;// 只在压缩bundle时进行预载

    public const string NORMAL_TXT = "normal_txt";//otherdata...
    //public const string SCENE_TXT = "scene_txt";//scene相关的txt文件,  entity, config, dynamicObstacle, block map, height map
    public const string SYNCLOAD_ASSETS = "syncload_assets";// 也包含了 SCENE_TXT

    public const string ASSETS = "assets";
    public const string ASSETS_ZIP = "assets_ZIP";
    public const string VERSION_TXT = "version.txt";
    public const string ZIP_VERSION_TXT = "zipVersion.txt";
    public const string TxtVer = "TxtVer.txt";
    public const string DIFF_TXT = "diff.txt";
    public const string PACK_TXT = "pack.txt";
    public const string ZIP_SIZE_TXT = "size.txt";
    public const string MD5_TXT = "md5.txt";

    public const string EXTENSION_OGG = "ogg";
    public const string EXTENSION_WAV = "wav";
    public const string EXTENSION_Texture = "tga";
    public const string EXTENSION_PREFAB = "prefab";
	public const string EXTENSION_PNG = "png";
    public const string EXTENSION_UNITY = "unity";
    public const string PetFightNormal = "PetFightNormal";
    public const string PetFightPVP = "PetFightPVP";
    public const string PetFightChallenging = "PetFightChallenging";

    //command
    public const string CommandPath = "otherdata/attrres/cmd/{0}";

#if UNITY_EDITOR
    //BuildTarget
    //public const BuildTarget AssetBundleBuildTarget = BuildTarget.iPhone;

    public static BuildTarget GetBuildTarget()
    {
        //BuildTarget rt = BuildTarget.iPhone;	
        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    rt = BuildTarget.StandaloneWindows;
        //}

        //IcLog.Log(rt);

        BuildTarget rt = EditorUserBuildSettings.activeBuildTarget;

        return rt;
    }
     
    //server path define
    public static bool GetServerPath(string relativePath, out string fullPath)
    {
        bool rt = true;

        fullPath = Application.dataPath;
        fullPath = fullPath.Replace("client/Assets", "server/");
        if (!System.IO.Directory.Exists(fullPath))
        {
            rt = false;
        }
        else
        {
            fullPath += relativePath;
        }

        return rt;
    }

    //这些直接更新服务器路径的文件要注意或者
    public const string ServerPath_NPCPath = "otherdata/attrres/serverPath/npcPath.txt";
    public const string ServerPath_MapEntityPath = "otherdata/map/entity/{0}.mon";//101.mon
    public const string ServerPath_MapBlockPath = "otherdata/map/blocks/";
#endif

    public static string GetPlatform()
    {
        string platformFolderForAssetBundles =
#if UNITY_EDITOR
 GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			GetPlatformFolderForAssetBundles(Application.platform);
#endif
        return platformFolderForAssetBundles;
    }

#if UNITY_EDITOR
    public static string GetPlatformFolderForAssetBundles(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "android";
            case BuildTarget.iOS:
                return "ios";
            case BuildTarget.WebGL:
                return "webplayer";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "windows";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
                return "osx";
            default:
                return "android";
        }
    }
#endif

    private static string GetPlatformFolderForAssetBundles(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            case RuntimePlatform.WebGLPlayer:
                return "webplayer";
            case RuntimePlatform.WindowsPlayer:
                return "windows";
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                return "osx";
            default:
                return "android";
        }
    }
}
