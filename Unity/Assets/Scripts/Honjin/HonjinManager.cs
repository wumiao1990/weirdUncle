using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class HonjinManager : MonoBehaviour
{
	private string[] sdPath = {"ABRes/PlayGround/art/sd-card/guangguo"};
	List<SkeletonAnimation> listSkeletonAnimation = new List<SkeletonAnimation>();
	private float MoveSpeed = 1.5f;
	
	// Use this for initialization
	void Start () {

		for (int i = 0; i< sdPath.Length; i++)
		{
			string path = sdPath[i];
			GameObject skeletonGo = AssetBundleManager.Instance.InstantiatePrefab<GameObject>(path);
			SkeletonAnimation sa = skeletonGo.GetComponent<SkeletonAnimation>();
		
			sa.skeletonDataAsset = GameObject.Instantiate<SkeletonDataAsset>(sa.skeletonDataAsset);
			sa.initialSkinName = sa.initialSkinName;
			sa.loop = true;
			sa.Initialize(true);
			sa.gameObject.transform.localPosition = Vector3.zero;
			
			listSkeletonAnimation.Add(sa);

			//sa.AnimationName = "B_walk";//B_sit01,B_eat,B_walk,B_idle01
		}
	}
	
	// Update is called once per frame
	Vector3 Target = Vector3.zero;
	void Update () 
	{
		if (Input.GetMouseButtonDown (0) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began)) //点击鼠标右键
        {
         	object ray = Camera.main.ScreenPointToRay(Input.mousePosition); 	//屏幕坐标转射线
         	RaycastHit hit;                                                     //射线对象是：结构体类型（存储了相关信息）
         	bool isHit = Physics.Raycast((Ray) ray, out hit);             //发出射线检测到了碰撞   isHit返回的是 一个bool值
         	if (isHit)
         	{
                Target = hit.point;
                SkeletonAnimation sa = listSkeletonAnimation[0];
                sa.AnimationName = "B_walk";
                
                Debug.Log("curretnPostion：" + sa.transform.position + "TargetPostion：" + hit.point + " obj:" + hit.collider.gameObject.name);

                if (Target.x > sa.transform.position.x)
                {
	                sa.transform.rotation = new Quaternion(0,0,0,0);
                }
                else
                {
	                sa.transform.rotation = new Quaternion(0,-200,0,0);
                }
            }
        }

		MoveSD();
	}

	void MoveSD()
	{
		GameObject go = listSkeletonAnimation[0].gameObject;
		SkeletonAnimation sa = listSkeletonAnimation[0];
		go.transform.position = Vector3.MoveTowards(go.transform.position, Target, MoveSpeed * Time.deltaTime);

		if (go.transform.position == Target)
		{
			sa.AnimationName = "B_idle01";
		}
	}

}
