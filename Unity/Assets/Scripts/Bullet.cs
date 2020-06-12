using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float time=3;//代表从A点出发到B经过的时长
	public Vector3 pointA;//点A
	public Vector3 pointB;//点B
	public float g=-10;//重力加速度
	// Use this for initialization
	private Vector3 speed;//初速度向量
	private Vector3 Gravity;//重力向量
	void Start () {

		transform.position = pointA;//将物体置于A点
		//通过一个式子计算初速度
		speed = new Vector3((pointB.x - pointA.x)/time,
			(pointB.y - pointA.y) / time- 0.5f * g * time, (pointB.z - pointA.z) / time);
		Gravity = Vector3.zero;//重力初始速度为0
	}
	private float dTime=0;
	// Update is called once per frame
	void FixedUpdate () {

		Gravity.y = g * (dTime += Time.fixedDeltaTime);//v=at
		//模拟位移
		transform.Translate(speed*Time.fixedDeltaTime);
		transform.Translate(Gravity * Time.fixedDeltaTime);
	}

	private float onDestroyTime = 1.5f;
	private float t = 0;
	private void Update()
	{
		t = t + Time.deltaTime;
		if (t > 3)
		{
			GameObject.Destroy(gameObject);
		}
	}
}
