using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class Character : MonoBehaviour {

	private float MoveSpeed = 1.5f;
	private SkeletonAnimation sa;
	
	// Use this for initialization
	void Awake () {
		sa = gameObject.GetComponent<SkeletonAnimation>();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void SetAnimation(string name)
	{
		if(sa != null)
			sa.AnimationName = name;
	}
	
	private void FixedUpdate()
	{
		MoveSD();
	}

	// Update is called once per frame
	Vector3 Target = Vector3.zero;
	public void StartMove(Vector3 sdTarget)
	{
		Target = sdTarget;
		if (gameObject.transform.position.x > Target.x)
		{
			gameObject.transform.rotation = new Quaternion(0, -200, 0, 0);
		}
		else
		{
			gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
		}

		SetAnimation("B_walk");
	}

	void MoveSD()
	{
		gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Target, MoveSpeed * Time.deltaTime);
		if (Target == gameObject.transform.position)
		{
			SetAnimation("B_idle01");
		}
	}
	
	//有刚体的不勾选is trigger,  被动方没有刚体，但是勾选IS Trigger
	void OnTriggerEnter(Collider other)
	{
		//模拟子弹打到人，打到后子弹和被打目标同时消失
		Debug.Log("触发器开始:" + other.gameObject.name);
		GameObject.Destroy(gameObject);
		Destroy(other.gameObject);
	}
		
	void OnTriggerStay(Collider other)
	{
		Debug.Log("触发器检测中:" + other.gameObject.name);
	}

	void OnTriggerExit(Collider other)
	{
		Debug.Log("触发器结束:" + other.gameObject.name);	
	}
}
