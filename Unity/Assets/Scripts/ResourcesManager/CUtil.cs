using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mga;
using System.Security.Cryptography;

public class CUtil
{
    public static int GroundLayer_0_31 = LayerMask.NameToLayer("Ground");
    public static int PlayerLayer_0_31 = LayerMask.NameToLayer("Player");
    public static int GuideLayer_0_31 = LayerMask.NameToLayer("Guide");
    public static int MapItemLayer_o_31 = LayerMask.NameToLayer("Ignore Raycast");
    public static int UILayer_0_31 = LayerMask.NameToLayer("GUI");

    static public bool FindChildTransorm(GameObject go)
    {
        if (go == null)
            return false;
        if (go.transform.childCount > 0)
            return true;
        else
           return false;
    }

    /// <summary>
    /// 16位MD5加密
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string MD5Encrypt16(string password)
    {
        var md5 = new MD5CryptoServiceProvider();
        string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(password)), 4, 8);
        t2 = t2.Replace("-", "");
        return t2.ToLower();
    }  

    static public void DeleteAllChildren(GameObject go)
    {
        if (go == null)
            return;

        DeleteAllChildren(go.transform);
    }

    static public void DeleteAllChildren(Transform t)
    {
        if (t == null)
            return;

        List<GameObject> goDelete = new List<GameObject>();
        for (int i = 0, imax = t.childCount; i < imax; ++i)//t.GetChildCount()
        {
            Transform child = t.GetChild(i);
            goDelete.Add(child.gameObject);
        }

        foreach (GameObject o in goDelete)
        {
            Destroy(o);
        }
    }

    public static long ParseLong(string str)
    {
        long num = 0;
        try
        {
            try
            {
                if (str == null)
                {
                    num = 0;
                }
                else
                {
                    num = long.Parse(str);
                }
            }
            catch (FormatException ex)
            {
                ex.GetType();
                float f = float.Parse(str);
                num = (long)f;
            }
            catch (OverflowException ex)//9999999999
            {
                num = long.MaxValue;
                Debug.LogWarning(string.Format("ParseLong exception={0}, originalString={1}", ex, str));
            }
        }
        catch (Exception ex)
        {
            num = 0;
            UnityEngine.Debug.LogError(string.Format("ParseLong, exception={0}, originalString={1}", ex, str));
        }

        return num;
    }

    public static string Md5Sum(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        try
        {
            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            if (hashBytes != null)
            {
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        return sb.ToString();
    }

    static public void Destroy(UnityEngine.Object obj, float time)
    {
        if (obj != null)
        {
            if (obj is GameObject)
            {
                GameObject go = obj as GameObject;
                //go.transform.parent = null;
            }

            UnityEngine.Object.Destroy(obj, time);
        }
    }
    static public void Destroy(UnityEngine.Object obj)
    {
        if (obj is Transform)
        {
            UnityEngine.Debug.LogError("Can't destroy Transform component", obj);
            return;
        }

        if (obj != null)
        {
            if (Application.isPlaying)
            {
                //注释掉以下代码的原因: https://forum.unity3d.com/threads/cant-destroy-transform-component.209444/
                //get the error (Can't destroy Transform component) in my console when I exit the editor play mode occasionally.

                //if (obj is GameObject)
                //{
                //    GameObject go = obj as GameObject;
                //    go.transform.parent = null;
                //}

                UnityEngine.Object.Destroy(obj);
            }
            else UnityEngine.Object.DestroyImmediate(obj);
        }
    }

    public static int ParseInt(string str)
    {
        int num = 0;
        try
        {
            try
            {
                if (str == null)
                {
                    num = 0;
                }
                else
                {
                    num = int.Parse(str);
                }
            }
            catch (FormatException ex)
            {
                ex.GetType();
                float f = float.Parse(str);
                num = (int)f;
            }
            catch (OverflowException ex)//9999999999
            {
                num = int.MaxValue;
                Debug.LogWarning(string.Format("ParseInt exception={0}, originalString={1}", ex, str));
            }
        }
        catch (Exception ex)
        {
            num = 0;
            UnityEngine.Debug.LogError(string.Format("ParseInt, exception={0}, originalString={1}", ex, str));
        }

        return num;
    }

    public static string UrlWithRandomSeed(string url)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        string randStr = string.Format("{0}{1}", Time.realtimeSinceStartup, UnityEngine.Random.Range(0, 100));
        string realUrl = string.Format("{0}?t={1}", url, randStr);
        return realUrl;
    }

    public static string ConvertFileNameForAssetbundleMd5(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            //替换所有的".", " ", "(", ")" , "#" 为 "_"
            StringBuilder sb = new StringBuilder(fileName);
            sb.Replace('.', '_');
            sb.Replace(' ', '_');
            sb.Replace('(', '_');
            sb.Replace(')', '_');
            sb.Replace('#', '_');

            sb.Replace('[', '_');
            sb.Replace(']', '_');

            //loadfromcacheordownlad需要这个(以防重名), 但是没有文件夹结构的话, 资源看起来太乱了
            sb.Replace('/', '_', 7, sb.Length - 7);//"Assets_" => "Assets/"//其中从 startIndex 到 startIndex + count -1 范围内的 oldChar 被 newChar 替换。 
            string[] tempFileNameList = sb.ToString().Split('/');
            string title = "";
            string content = "";

            if (tempFileNameList.Length > 1)
            {
                title = tempFileNameList[0];
                content = MD5Encrypt16(tempFileNameList[1].ToLower());

                fileName = title + "/" + content;
            }
            else
                fileName = sb.ToString();

        }

        return fileName;
    }

    public static string ConvertFileNameForAssetbundleTwo(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            //替换所有的".", " ", "(", ")" , "#" 为 "_"
            StringBuilder sb = new StringBuilder(fileName);
            sb.Replace('.', '_');
            sb.Replace(' ', '_');
            sb.Replace('(', '_');
            sb.Replace(')', '_');
            sb.Replace('#', '_');

            sb.Replace('[', '_');
            sb.Replace(']', '_');

            //loadfromcacheordownlad需要这个(以防重名), 但是没有文件夹结构的话, 资源看起来太乱了
            sb.Replace('/', '_');//"Assets_" => "Assets/"//其中从 startIndex 到 startIndex + count -1 范围内的 oldChar 被 newChar 替换。 

            fileName = sb.ToString();
        }

        return fileName;
    }

    public static string ConvertFileNameForAssetbundle(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            //替换所有的".", " ", "(", ")" , "#" 为 "_"
            StringBuilder sb = new StringBuilder(fileName);
            sb.Replace('.', '_');
            sb.Replace(' ', '_');
            sb.Replace('(', '_');
            sb.Replace(')', '_');
            sb.Replace('#', '_');

            sb.Replace('[', '_');
            sb.Replace(']', '_');

            //loadfromcacheordownlad需要这个(以防重名), 但是没有文件夹结构的话, 资源看起来太乱了
            sb.Replace('/', '_', 7, sb.Length - 7);//"Assets_" => "Assets/"//其中从 startIndex 到 startIndex + count -1 范围内的 oldChar 被 newChar 替换。 

            fileName = sb.ToString();
        }

        return fileName;
    }
    public static string FileMD5(string fullPath)
    {
        using (FileStream fs = File.Open(fullPath, FileMode.Open))
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);

            StringBuilder sb = new StringBuilder();

            if (retVal != null)
            {
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
            }

            return sb.ToString();
        }
    }

    public static void SplitPath(string path, out string dir, out string name)
    {
        string extension;
        SplitPath(path, out dir, out name, out extension);
    }

    public static void SplitPath(string path, out string dir, out string name, out string extension)
    {
        //otherdata/attrres/consumelibao7day
        dir = string.Empty;
        name = string.Empty;
        extension = string.Empty;

        if (!string.IsNullOrEmpty(path))
        {
            //各种类型的path
            //AssetBundles/Windows/unpackTest/monsterd003.fbm
            //monsterd003.fbm
            //monsterd003
            //
            //AssetBundles/Windows/unpackTest/monsterd003.fbm/
            //AssetBundles/Windows/unpackTest/monsterd003.fbm/digongkulougwu_base_tga


            //if (path.EndsWith("/"))//处理类似这样的dir: "AssetBundles/Windows/unpackTest/monsterd003.fbm/"
            //{
            //    dir = path;
            //}
            //else
            //{
            //    //'.'
            //    int pos = path.LastIndexOf('.');
            //    if (pos != -1)
            //    {
            //        //Substring : 一个 String 对象，它等于此实例中从 startIndex 开始的子字符串，如果 startIndex 等于此实例的长度，则为 Empty
            //        extension = path.Substring(pos + 1);
            //        path = path.Substring(0, pos);//dir + name (不包含".")
            //        name = path;//如果不含'/', 下面的代码段不执行, 这儿先设好
            //    }

            //    //'/'
            //    if (!string.IsNullOrEmpty(path))
            //    {
            //        pos = path.LastIndexOf('/');
            //        if (pos != -1)
            //        {
            //            name = path.Substring(pos + 1);
            //            dir = path.Substring(0, pos + 1);//包含"/"
            //        }
            //    }
            //}

            //'/'
            int pos = path.LastIndexOf('/');
            if (pos != -1)
            {
                dir = path.Substring(0, pos + 1);//包括"/" 

                //Substring : 一个 String 对象，它等于此实例中从 startIndex 开始的子字符串，如果 startIndex 等于此实例的长度，则为 Empty
                path = path.Substring(pos + 1);//name + extension
            }

            //'.'
            if (!string.IsNullOrEmpty(path))
            {
                pos = path.LastIndexOf('.');
                if (pos != -1)
                {
                    extension = path.Substring(pos + 1);//(不包含".")
                    name = path.Substring(0, pos);//(不包含".")
                }
                else
                {
                    //无extension
                    name = path;
                }
            }
        }
    }

    static public GameObject AddChild(GameObject parent, UnityEngine.Object prefab, bool bSetLayerRecursively = false)
    {
        if (prefab == null)
            return null;

        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            ResetLocalTransform(t);

            if (!bSetLayerRecursively)
                go.layer = parent.layer;
            else
                SetLayerRecursively(go, parent.layer);
        }
        return go;
    }

    static public void SetLayerRecursively(GameObject go, int layer)
    {
        if (go == null)
            return;

        go.layer = layer;

        Transform t = go.transform;
        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    static public void ResetLocalTransform(Transform t)
    {
        if (t != null)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
    }
}
