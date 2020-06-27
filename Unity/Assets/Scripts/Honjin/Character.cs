using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class Character : MonoBehaviour {

	private float MoveSpeed = 1.5f;
	private SkeletonAnimation sa;
	private List<Vector3> movePos;
	
	// Use this for initialization
	void Awake () {
		sa = gameObject.GetComponent<SkeletonAnimation>();
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

	private float waitTime = 1f;

	private float stayTime = 0;

	private bool isMove = false;
	// Update is called once per frame
	void Update ()
	{
		if (!isMove)
		{
			isMove = true;
			stayTime = 0;
			int r = Random.Range(0, movePos.Count);
			Target = GetTargetPos();
			StartMove(Target);
		}
	}

	Vector3 GetTargetPos()
	{
		int r = Random.Range(0, movePos.Count);
		Vector3 tarpos = movePos[r];
		if (tarpos == Target)
		{
			return GetTargetPos();
		}
		else
		{
			return tarpos;
		}
	}

	public void SetData(SDModel model)
	{
		movePos = model.movePos;
		waitTime = model.waitTime;
		MoveSpeed = model.speed;
	}
	// Update is called once per frame
	Vector3 Target = Vector3.zero;
	public void StartMove(Vector3 sdTarget)
	{
		Target = sdTarget;
		if (gameObject.transform.localPosition.x > Target.x)
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
		gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, Target, MoveSpeed * Time.deltaTime);
		if (Target == gameObject.transform.localPosition && isMove)
		{
			SetAnimation("B_idle01");
			stayTime += Time.deltaTime;
			if (stayTime >= waitTime)
			{
				isMove = false;
			}
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
