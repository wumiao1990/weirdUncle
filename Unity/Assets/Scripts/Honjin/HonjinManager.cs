using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class HonjinManager : MonoBehaviour
{
	public Transform SDObj;
	//private string[] sdPath = {"ABRes/PlayGround/art/sd-card/guangguo"};
	List<SkeletonAnimation> listSkeletonAnimation = new List<SkeletonAnimation>();
	public List<SDModel> listSDModel;
	public Dictionary<int, Character> dicSDModel = new Dictionary<int, Character>();
	public static HonjinManager instance;
	// Use this for initialization
	void Start ()
	{
		instance = this;	
		for (int i = 0; i< listSDModel.Count; i++)
		{
			string path = listSDModel[i].Path;
			GameObject skeletonGo = AssetBundleManager.Instance.InstantiatePrefab<GameObject>(path);
			SkeletonAnimation sa = skeletonGo.GetComponent<SkeletonAnimation>();
			Character ct = skeletonGo.GetComponent<Character>();
			skeletonGo.transform.parent = SDObj;
		
			sa.skeletonDataAsset = GameObject.Instantiate<SkeletonDataAsset>(sa.skeletonDataAsset);
			sa.initialSkinName = sa.initialSkinName;
			sa.AnimationName = "B_walk";
			sa.loop = true;
			sa.Initialize(true);
			sa.gameObject.transform.localPosition = listSDModel[i].initalPos;
			ct.Target = listSDModel[i].initalPos;
			ct.SetData(listSDModel[i]);
			listSkeletonAnimation.Add(sa);

			if (path.Contains("daji"))
			{
				dicSDModel.Add(3823, ct);
			}
			else if(path.Contains("guangguo"))
			{
				dicSDModel.Add(8833, ct);
			}
			else if(path.Contains("zhou_2"))
			{
				dicSDModel.Add(6871, ct);
			}
			//sa.AnimationName = "B_walk";//B_sit01,B_eat,B_walk,B_idle01
		}
		
		SoundManager.instance.StopAllSounds();
		SoundManager.instance.PlaySound(SoundManager.instance.BGM, true, 0.4f);
	}

	private void Update()
	{
//		if (Input.GetMouseButtonDown(0)) {//鼠标左键按下
//			if (this.s.isPlaying)
//			{
//				this.s.Pause();
//			}
//			else {
//				this.s.Play();
//			}
//		}
//		if (Input.GetMouseButtonDown(1)) {//鼠标右键按下
//			this.s.Stop();
//		}
	}
}

[Serializable]
public class SDModel
{
	public string Path;
	public float waitTime;
	public float speed;
	public Vector3 initalPos = Vector3.zero;
	public List<GameObject> movePos;
}
