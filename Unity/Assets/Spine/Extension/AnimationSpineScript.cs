using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpineScript : StateMachineBehaviour {
    public string animationName;
    public float speed;
    public WrapMode mode = WrapMode.Once;
    public bool loop = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SkeletonAnimation anim = animator.GetComponent<SkeletonAnimation>();
        if (anim != null)
        {
            TrackEntry te = anim.state.SetAnimation(0, animationName, loop);
            te.timeScale = speed;
        }

        SkeletonGraphic sg = animator.GetComponent<SkeletonGraphic>();
        if (sg != null)
        {
            TrackEntry te = sg.AnimationState.SetAnimation(0, animationName, loop);
            te.timeScale = speed;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
