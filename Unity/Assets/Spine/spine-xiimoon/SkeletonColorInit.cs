using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonColorInit : MonoBehaviour
{
    public Color skeletonColor = Color.white;
    public List<SlotSettings> slotSettings = new List<SlotSettings>();

    [System.Serializable]
    public class SlotSettings
    {
        [SpineSlot]
        public string slot = string.Empty;
        public Color color = Color.white;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        var skeletonComponent = GetComponent<ISkeletonComponent>();
        if (skeletonComponent != null)
        {
            skeletonComponent.Skeleton.SetSlotsToSetupPose();
            var animationStateComponent = GetComponent<IAnimationStateComponent>();
            if (animationStateComponent != null && animationStateComponent.AnimationState != null)
            {
                animationStateComponent.AnimationState.Apply(skeletonComponent.Skeleton);
            }
        }
        ApplySettings();
    }
#endif

    void Start()
    {
        ApplySettings();
    }

    void ApplySettings()
    {
        var skeletonComponent = GetComponent<ISkeletonComponent>();
        if (skeletonComponent != null)
        {
            var skeleton = skeletonComponent.Skeleton;
            skeleton.SetColor(skeletonColor);

            foreach (var s in slotSettings)
            {
                var slot = skeleton.FindSlot(s.slot);
                if (slot != null) slot.SetColor(s.color);
            }

        }
    }

}
