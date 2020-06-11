using System;
using Spine;
using Spine.Unity;
using UnityEngine;

    public class BoneScaleFollower : MonoBehaviour
    {
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
        [SpineBone(dataField: "skeletonRenderer")]
        public String boneName;

        public Vector2 flags = Vector2.one;
        public bool ChangeXY = false;

        [NonSerialized] public bool valid;
        [NonSerialized] public Bone bone;
        private Vector2 m_lastScale = Vector2.zero;
        private Vector2 m_curScale;
        private Transform m_tranCache;

        public void Awake()
        {
            m_tranCache = transform;
            Initialize();
        }

        public bool Initialize()
        {
            bone = null;
            valid = skeletonRenderer != null && skeletonRenderer.valid;
            if (!valid) return false;

            //skeletonTransform = skeletonRenderer.transform;

            return true;
        }

        void OnDestroy()
        {
            
        }

        public void LateUpdate()
        {
            if (!valid)
            {
                Initialize();
                return;
            }

            if (bone == null)
            {
                if (string.IsNullOrEmpty(boneName)) return;

                bone = skeletonRenderer.skeleton.FindBone(boneName);
                if (bone == null)
                {
                    Debug.LogError("Bone not found: " + boneName, this);
                    enabled = false;
                    return;
                }
            }

            if (ChangeXY)
            {
                m_curScale.y = Mathf.Sign(bone.ScaleX);
                m_curScale.x = Mathf.Sign(bone.ScaleY);
            }
            else
            {
                m_curScale.x = Mathf.Sign(bone.ScaleX);
                m_curScale.y = Mathf.Sign(bone.ScaleY);
            }

            m_curScale.Scale(flags);
            if ((m_curScale - m_lastScale).SqrMagnitude() > Mathf.Epsilon)
            {
                m_lastScale = m_curScale;
                m_tranCache.localScale = m_lastScale;
            }
        }
    }
