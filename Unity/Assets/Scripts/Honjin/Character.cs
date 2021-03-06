﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Spine.Unity;
using UnityEngine;

public class Character : MonoBehaviour {

	private float MoveSpeed = 1.5f;
	private SkeletonAnimation sa;
	private List<GameObject> movePos;
	public Vector3 initalPos;
	private MeshRenderer _meshRenderer;
	public State st = State.B_idle01;
	public enum State
	{
		B_idle01,
		B_walk,
		work,
	}
	// Use this for initialization
	void Awake () {
		sa = gameObject.GetComponent<SkeletonAnimation>();
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();
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
		if (!isMove && movePos != null && movePos.Count > 0)
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
		Vector3 tarpos = movePos[r].transform.localPosition;
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
		initalPos = model.initalPos;
	}

	public void SetTargetPos(Vector3 target, float offets = 1)
	{
		Target = new Vector3(target.x - offets, target.y, target.z);
		if (gameObject.transform.localPosition.x > Target.x)
		{
			gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
		}
		else
		{
			gameObject.transform.rotation = new Quaternion(0, -200, 0, 0);
		}
		st = State.work;
	}
	
	// Update is called once per frame
	public Vector3 Target = Vector3.zero;
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

		SetAnimation(State.B_walk.ToString());
	}

	void MoveSD()
	{
		gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, Target, MoveSpeed * Time.deltaTime);
		int sortOrder = GroundManager.nodeHeight - (int)gameObject.transform.localPosition.y + 1;
		_meshRenderer.sortingOrder = sortOrder;
		if (Target == gameObject.transform.localPosition && isMove)
		{
			SetAnimation(State.B_walk.ToString());
			stayTime += Time.deltaTime;
			if (stayTime >= waitTime)
			{
				isMove = false;
			}
		}
		else if (st == State.work && Target == gameObject.transform.localPosition)
		{
			SetAnimation(st.ToString());
			gameObject.transform.rotation = new Quaternion(0, -200, 0, 0);
		}
		else
		{
			SetAnimation(State.B_walk.ToString());
		}
	}
	
	//有刚体的不勾选is trigger,  被动方没有刚体，但是勾选IS Trigger
	void OnTriggerEnter(Collider other)
	{
		//模拟子弹打到人，打到后子弹和被打目标同时消失
		Debug.Log("触发器开始:" + other.gameObject.name);
		GameObject.Destroy(gameObject);
		Destroy(other.gameObject);
		SoundManager.instance.PlaySound(SoundManager.instance.Tap1, false);
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
