/******************************************************************************
 * Spine Runtimes Software License v2.5
 *
 * Copyright (c) 2013-2016, Esoteric Software
 * All rights reserved.
 *
 * You are granted a perpetual, non-exclusive, non-sublicensable, and
 * non-transferable license to use, install, execute, and perform the Spine
 * Runtimes software and derivative works solely for personal or internal
 * use. Without the written permission of Esoteric Software (see Section 2 of
 * the Spine Software License Agreement), you may not (a) modify, translate,
 * adapt, or develop new applications using the Spine Runtimes or otherwise
 * create derivative works or improvements of the Spine Runtimes or (b) remove,
 * delete, alter, or obscure any trademarks or any copyright, trademark, patent,
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 *
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
 * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

#if (UNITY_5_6_OR_NEWER || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_WSA || UNITY_WP8 || UNITY_WP8_1)
#define IS_UNITY
#endif

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Spine.Unity;
using Spine.Unity.Editor;
using System.Collections.Generic;

#if WINDOWS_STOREAPP
using System.Threading.Tasks;
using Windows.Storage;
#endif

namespace Spine
{
    public class SplitAnimationTool
    {
        public static void DoSplit(string assetPath, string exportAnimationPath, int limitCount, bool fromBackUp = false)
        {
            SkeletonDataAsset skelAsset = (SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(assetPath, typeof(SkeletonDataAsset));

            if (skelAsset == null) return;


            TextAsset _asset = skelAsset.skeletonJSON;
            string _skelPath = AssetDatabase.GetAssetPath(_asset);

            if (fromBackUp)
            {
                int _length = _skelPath.LastIndexOf(".");
                string _backupPath = _skelPath.Insert(_length, "_backup");
                TextAsset _assetBackup = AssetDatabase.LoadAssetAtPath(_backupPath, typeof(TextAsset)) as TextAsset;
                if (_assetBackup != null)
                {
                    AssetDatabase.CopyAsset(_backupPath, _skelPath);
                    AssetDatabase.Refresh();
                }
            }

            if (!_skelPath.ToLower().Contains(".skel"))
            {
                Debug.LogErrorFormat("skipped json format, path:{0}", assetPath);
                return;
            }

            // if (skelAsset.controller == null) //没有生成，有了更新。
            // SkeletonBaker.GenerateMecanimAnimationClips(skelAsset);
            SkeletonBaker.GenerateMecanimAnimationClips(skelAsset);

            //拆分并备份
            GetRequiredAtlasRegions(_skelPath, exportAnimationPath, limitCount);

            skelAsset.Clear();
            skelAsset.GetSkeletonData(true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void AddRequiredAtlasRegionsFromBinary(string skeletonDataPath, List<string> requiredPaths, string _animDir, int limitCount)
        {
            SkeletonBinary_Xiimoon binary = new SkeletonBinary_Xiimoon(new SpineEditorUtilities.AtlasRequirementLoader(requiredPaths));
            TextAsset data = (TextAsset)AssetDatabase.LoadAssetAtPath(skeletonDataPath, typeof(TextAsset));
            MemoryStream input = new MemoryStream(data.bytes);
            binary.ReadSkeletonData(input, skeletonDataPath, _animDir, limitCount);
            binary = null;
        }

        static List<string> GetRequiredAtlasRegions(string skeletonDataPath, string _animDir, int limitCount)
        {
            List<string> requiredPaths = new List<string>();

            AddRequiredAtlasRegionsFromBinary(skeletonDataPath, requiredPaths, _animDir, limitCount);

            return requiredPaths;
        }
    }
}
