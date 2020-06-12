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

using System;
using UnityEngine;

namespace Spine.Unity
{
    /// <summary>Sets a GameObject's transform to match a bone on a Spine skeleton.</summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Spine/SlotFollower")]
    public class SlotFollower : MonoBehaviour
    {

        #region Inspector
        public SkeletonRenderer skeletonRenderer;
        public SkeletonRenderer SkeletonRenderer
        {
            get { return skeletonRenderer; }
            set
            {
                skeletonRenderer = value;
                Initialize();
            }
        }

        public bool LoadSkeletonRenderer(SkeletonRenderer skr)
        {
            skeletonRenderer = skr;
            return Initialize();
        }

        /// <summary>If a bone isn't set in code, boneName is used to find the bone.</summary>
        [SpineSlot(dataField: "skeletonRenderer")]
        public String slotName;

        public bool followZPosition = true;
        public bool followBoneRotation = true;

        [Tooltip("Follows the skeleton's flip state by controlling this Transform's local scale.")]
        public bool followSkeletonFlip = false;

        [UnityEngine.Serialization.FormerlySerializedAs("resetOnAwake")]
        public bool initializeOnAwake = false;
        #endregion

        [NonSerialized] public bool valid;
        [NonSerialized] public Slot slot;
        Transform skeletonTransform;

        public void Awake()
        {
            if (initializeOnAwake) Initialize();
        }

        public void HandleRebuildRenderer(SkeletonRenderer skeletonRenderer)
        {
            Initialize();
        }

        public bool Initialize()
        {
            slot = null;
            valid = skeletonRenderer != null && skeletonRenderer.valid;
            if (!valid) return false;

            skeletonTransform = skeletonRenderer.transform;
            skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
            skeletonRenderer.OnRebuild += HandleRebuildRenderer;

            if (string.IsNullOrEmpty(slotName)) return false;

            slot = skeletonRenderer.skeleton.FindSlot(slotName);
            if (slot == null)
            {
                Debug.LogErrorFormat(this, "Slot not found: {0}", slotName);
                return false;
            }

#if UNITY_EDITOR
            if (Application.isEditor)
                LateUpdate();
#endif
            return true;
        }

        void OnDestroy()
        {
            if (skeletonRenderer != null)
                skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
        }

        public void LateUpdate()
        {
            if (!valid)
            {
                Initialize();
                return;
            }

            if (slot == null)
            {
                return;
            }

            Transform thisTransform = this.transform;
            if (thisTransform.parent == skeletonTransform)
            {
                // Recommended setup: Use local transform properties if Spine GameObject is the immediate parent
                thisTransform.localPosition = new Vector3(slot.Bone.worldX, slot.Bone.worldY, followZPosition ? 0f : thisTransform.localPosition.z);
                if (followBoneRotation) thisTransform.localRotation = Quaternion.Euler(0f, 0f, slot.Bone.WorldRotationX);

            }
            else
            {
                // For special cases: Use transform world properties if transform relationship is complicated
                Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(slot.Bone.worldX, slot.Bone.worldY, 0f));
                if (!followZPosition) targetWorldPosition.z = thisTransform.position.z;
                thisTransform.position = targetWorldPosition;

                if (followBoneRotation)
                {
                    Vector3 worldRotation = skeletonTransform.rotation.eulerAngles;
                    thisTransform.rotation = Quaternion.Euler(worldRotation.x, worldRotation.y, skeletonTransform.rotation.eulerAngles.z + slot.Bone.WorldRotationX);
                }
            }

            if (followSkeletonFlip)
            {
                float flipScaleY = slot.Bone.skeleton.flipX ^ slot.Bone.skeleton.flipY ? -1f : 1f;
                thisTransform.localScale = new Vector3(1f, flipScaleY, 1f);
            }
        }
    }

}