using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class HonjinManager : MonoBehaviour
{
	//private string[] sdPath = {"ABRes/PlayGround/art/sd-card/guangguo"};
	List<SkeletonAnimation> listSkeletonAnimation = new List<SkeletonAnimation>();
	public List<SDModel> listSDModel;
	
	// Use this for initialization
	void Start () {

		for (int i = 0; i< listSDModel.Count; i++)
		{
			string path = listSDModel[i].Path;
			GameObject skeletonGo = AssetBundleManager.Instance.InstantiatePrefab<GameObject>(path);
			SkeletonAnimation sa = skeletonGo.GetComponent<SkeletonAnimation>();
			Character ct = skeletonGo.GetComponent<Character>();
		
			sa.skeletonDataAsset = GameObject.Instantiate<SkeletonDataAsset>(sa.skeletonDataAsset);
			sa.initialSkinName = sa.initialSkinName;
			sa.loop = true;
			sa.Initialize(true);
			sa.gameObject.transform.localPosition = listSDModel[i].initalPos;

			ct.movePos = listSDModel[i].movePos;
			
			listSkeletonAnimation.Add(sa);
			//sa.AnimationName = "B_walk";//B_sit01,B_eat,B_walk,B_idle01
		}
	}
}

[Serializable]
public class SDModel
{
	public string Path;
	public Vector3 initalPos = Vector3.zero;
	public List<Vector3> movePos;
}
