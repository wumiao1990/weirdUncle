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

using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SpineBlinkPlayer : MonoBehaviour
{
    const int FaceTrack = 1;
    const int BlinkTrack = 2;
    const int MouthTrack = 3;

    const string NormalFace = "base_normal";

    [SpineAnimation]
    public string faceAnimation;

    string currentFace;

    [SpineAnimation]
    public string blinkAnimation;

    [SpineAnimation]
    public string mouthAnimation;


    public float minimumDelay = 0.15f;
    public float maximumDelay = 3f;

    IEnumerator Start()
    {
        var skeletonAnimation = GetComponent<SkeletonAnimation>(); if (skeletonAnimation == null) yield break;

        StartCoroutine(Blink(skeletonAnimation));
        StartCoroutine(Speak(skeletonAnimation));

        while (true)
        {
            if (currentFace != faceAnimation)
            {
                currentFace = faceAnimation;
                if (currentFace != SpineBlinkPlayer.NormalFace)
                {
                    skeletonAnimation.state.SetEmptyAnimation(SpineBlinkPlayer.BlinkTrack, 0f);
                    skeletonAnimation.state.SetEmptyAnimation(SpineBlinkPlayer.MouthTrack, 0f);
                }

                skeletonAnimation.state.SetAnimation(SpineBlinkPlayer.FaceTrack, faceAnimation, true);
            }

            yield return null;
        }
    }

    IEnumerator Blink(SkeletonAnimation sk)
    {
        while (true)
        {
            if (currentFace == SpineBlinkPlayer.NormalFace)
            {
                sk.state.SetAnimation(SpineBlinkPlayer.BlinkTrack, blinkAnimation, false);
                yield return new WaitForSeconds(Random.Range(minimumDelay, maximumDelay));
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator Speak(SkeletonAnimation sk)
    {
        while (true)
        {
            if (currentFace == SpineBlinkPlayer.NormalFace)
            {
                var t = sk.state.GetCurrent(SpineBlinkPlayer.MouthTrack);
                if (t != null && t.animation != null)
                {
                    sk.state.AddEmptyAnimation(MouthTrack, 0.1f, 0f);
                }
                else
                {
                    sk.state.SetAnimation(SpineBlinkPlayer.MouthTrack, mouthAnimation, false);
                }

                yield return new WaitForSeconds(Random.Range(minimumDelay, maximumDelay));
            }
            else
            {
                yield return null;
            }
        }
    }
}
