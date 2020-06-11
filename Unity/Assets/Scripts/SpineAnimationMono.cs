using UnityEngine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
using Spine;

public class SpineAnimationMono : MonoBehaviour
{
    public const string BaseAnimation_1 = "base_01";
    public const string BaseAnimation_2 = "base_02";
    public const string BaseEmotion = "base_normal";
    public const string BaseBlink = "base_eye";
    //public const string BaseSpeak = "base_mouth";
    public const int BaseTrack = 0;
    public const int EmotionTrack = 1;
    public const int EyeTrack = 2;
    //public const int MouthTrack = 3;
    public const string BattleIdle_0 = "idle";
    public const string BattleIdle_1 = "idle1";
    public const string BattleIdle_2 = "idle2";

    ISkeletonAnimation _animation;
    public Spine.AnimationState AnimationState
    {
        get
        {
            if (_animation is SkeletonGraphic) return (_animation as SkeletonGraphic).AnimationState;
            if (_animation is SkeletonAnimation) return (_animation as SkeletonAnimation).AnimationState;
            return null;
        }
    }
    public Skeleton skeleton
    {
        get
        {
            if (_animation is SkeletonGraphic) return (_animation as SkeletonGraphic).Skeleton;
            if (_animation is SkeletonAnimation) return (_animation as SkeletonAnimation).Skeleton;
            return null;
        }
    }

    List<string> animationList = new List<string>();

    void Awake()
    {
        var graphic = GetComponent<SkeletonGraphic>();
        if (graphic != null) _animation = graphic;
        else
        {
            var aimation = GetComponent<SkeletonAnimation>();
            if (aimation != null) _animation = aimation;
            else
            {
                Debug.LogErrorFormat("{0} can not find component SkeletonAnimation!", gameObject.name);
                return;
            }
        }


        var data = AnimationState.Data.skeletonData;
        for (int i = 0; i < data.animations.Count; i++)
        {
            var animation = data.animations.Items[i];
            animationList.Add(animation.name);
        }
        for (int i = 0; i < data.AnimationNames.Count; i++)
        {
            animationList.Add(data.AnimationNames.Items[i]);
        }
    }

    /// <summary>
    /// 设置并播放默认循环动画
    /// </summary>
    public void SetDefault(string name)
    {
        if (Contains(name, true))
        {
            var track = AnimationState.SetAnimation(0, name, true);
        }
    }

    /// <summary>
    /// 清除设置的默认循环动画
    /// </summary>
    public void ClearDefault()
    {
        ClearTrack(BaseTrack);
    }

    public void WaitToPlay(string name, int waitFrame = 3, bool isLoop = false, int layer = 1, float MixDuration = 0.2f)
    {
        //CUI.CBGameUIManager.Instance.StartCoroutine(WaitPlay(name, waitFrame, isLoop, layer, MixDuration));
    }

    IEnumerator WaitPlay(string name, int waitFrame, bool isLoop = false, int layer = 1, float MixDuration = 0.2f)
    {
        yield return waitFrame;
        Play(name, isLoop, layer, MixDuration);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public TrackEntry Play(string name, bool isLoop = false, int layer = 1, float MixDuration = 0.2f)
    {
        TrackEntry track = null;
        if (Contains(name, true))
        {
            if (layer == SpineAnimationMono.BaseTrack)
            {
                var current = AnimationState.GetCurrent(SpineAnimationMono.BaseTrack);
                if (current != null && 
                    current.Loop == isLoop &&
                    current.trackIndex == layer &&
                    current.animation != null && 
                    isBaseAnimation(current.animation.name) && 
                    isBaseAnimation(name))
                {
                    return current;
                }
            }
            track = AnimationState.SetAnimation(layer, name, isLoop);
            track.MixDuration = MixDuration;
        }

        return track;
    }

    public void ClearTrack(int trackIndex)
    {
        if (AnimationState != null)
            AnimationState.ClearTrack(trackIndex);
    }

    public void WaitToAdd(string name, int waitFrame = 3, bool isLoop = false, int layer = 1, float MixDuration = 0.2f, float delay = 0)
    {
        //CUI.CBGameUIManager.Instance.StartCoroutine(WaitAdd(name, waitFrame, isLoop, layer, MixDuration, delay));
    }

    IEnumerator WaitAdd(string name, int waitFrame, bool isLoop = false, int layer = 1, float MixDuration = 0.2f, float delay = 0)
    {
        yield return waitFrame;
        PlayQueue(name, isLoop, delay, layer, MixDuration);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public float PlayQueue(string name, bool isLoop, float delay = 0, int layer = 1, float MixDuration = 0.2f)
    {
        if (Contains(name, true))
        {
            var track = AnimationState.AddAnimation(layer, name, isLoop, delay);
            track.MixDuration = MixDuration;
            track.timeScale = 1;
            track.TrackTime = 0;
            return track.Animation.Duration;
        }

        return 0;
    }

    public void Stop(int trackIndex = 1)
    {
        AnimationState.ClearTrack(trackIndex);
    }

    public void ForceUpdateOneFrame()
    {
        if (_animation != null)
        {
            if (_animation is SkeletonGraphic)
                ((SkeletonGraphic)_animation).Update(0.0333f);
            else if (_animation is SkeletonAnimation)
                ((SkeletonAnimation)_animation).Update(0.0333f);
        }
    }

    public void SetTimeScale(float timeScale)
    {
        if (_animation != null)
        {
            if (_animation is SkeletonGraphic)
                ((SkeletonGraphic)_animation).timeScale = timeScale;
            else if (_animation is SkeletonAnimation)
                ((SkeletonAnimation)_animation).timeScale = timeScale;
        }
    }

    /// <summary>
    /// 播放待机动画
    /// </summary>
    public void PlayIdle()
    {
        var aniName = "";
        if (Contains("idle"))
            aniName = "idle";
        else if (Contains("run"))
            aniName = "run";
        else if (Contains("run4"))
            aniName = "run4";
        else if (Contains("base_01"))
            aniName = "base_01";
        Play(aniName, true);
    }

    /// <summary>
    /// 播放待机动画
    /// </summary>
    public void PlayRun()
    {
        var aniName = "";
        if (Contains("run"))
            aniName = "run";
        else if (Contains("run4"))
            aniName = "run4";
        Play(aniName, true);
    }

    /// <summary>
    /// 停止播放动画
    /// </summary>
    public void Stop()
    {
        AnimationState.ClearTracks();
    }

    public bool IsPlaying(string name)
    {
        return (CurrentAnimationName == name);
    }

    private string CurrentAnimationName
    {
        get
        {
            string name = null;
            for (int i = 0; i < AnimationState.Tracks.Count; i++)
            {
                TrackEntry entry = AnimationState.GetCurrent(i);
                name = (entry == null ? null : entry.Animation.Name);
                if (!string.IsNullOrEmpty(name)) break;
            }
            return name;
        }
    }

    public string GetCurrentAnimation(int track)
    {
        TrackEntry entry = AnimationState.GetCurrent(track);
        return (entry == null) ? null : entry.Animation.Name;
    }

    /// <summary>
    /// 是否包含动画
    /// </summary>
    public bool Contains(string name, bool isLog = false)
    {
        if (animationList.Contains(name))
        {
            return true;
        }
        if (isLog)
            Debug.LogWarningFormat("{0} 不包含动画名 {1}", transform.name, name);
        return false;
    }

    Spine.Animation GetAnimation(string aniName)
    {
        return _animation.Skeleton.data.FindAnimation(aniName);
    }

    /// <summary>
    /// 获取指定动画名字时长
    /// </summary>
    public float GetAniamtionTime(string name)
    {
        // if (Contains(name, true))
        // {
        //     // return animationDic[name].duration;
        //     return GetAniamtion(name).duration;
        // }
        Spine.Animation _ani = GetAniamtion(name);
        if (_ani != null) return _ani.duration;
        return 0;
    }

    /// <summary>
    /// 获取指定动画名字时长
    /// </summary>
    public Spine.Animation GetAniamtion(string name)
    {
        if (Contains(name, true))
        {
            return _animation.Skeleton.data.FindAnimation(name);
        }
        return null;
    }

    /// <summary>
    /// 隐藏指定骨骼
    /// </summary>
    /// <param name="boneName"></param>
    public void HideBone(string boneName)
    {
        if (skeleton.FindSlotIndex(boneName) > 0)
            skeleton.SetAttachment(boneName, null);
    }

    /// <summary>
    /// 用指定贴图显示指定骨骼
    /// </summary>
    /// <param name="boneName"></param>
    /// <param name="textureName"></param>
    public void ShowBone(string boneName, string textureName)
    {
        try
        {
            if (skeleton.FindSlotIndex(boneName) > 0)
                skeleton.SetAttachment(boneName, textureName);
        }
        catch (Exception exp)
        {
            Debug.LogErrorFormat("SpineAnimationMono.ShowBone exp {0}", exp.Message);
        }
    }

    public static bool isBaseAnimation(string animation)
    {
        return animation == SpineAnimationMono.BaseAnimation_1;
    }
}
