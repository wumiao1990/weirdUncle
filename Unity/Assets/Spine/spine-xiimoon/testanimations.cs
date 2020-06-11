using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class testanimations : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		SkeletonAnimation _anim = gameObject.GetComponent<SkeletonAnimation>();
		_anim.state.SetAnimation(0,"idle",true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
