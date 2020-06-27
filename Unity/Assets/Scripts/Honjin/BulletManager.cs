﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

	public GameObject startPostObj;
	// Use this for initialization
	void Start () {
		
	}
	
	void Update () 
	{
		if (Input.GetMouseButtonDown (0) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began)) //点击鼠标右键
		{
			object ray = Camera.main.ScreenPointToRay(Input.mousePosition); 	//屏幕坐标转射线
			RaycastHit hit;                                                     //射线对象是：结构体类型（存储了相关信息）
			bool isHit = Physics.Raycast((Ray) ray, out hit);             //发出射线检测到了碰撞   isHit返回的是 一个bool值
			if (isHit)
			{
                
				Debug.Log("TargetPostion：" + hit.point + " obj:" + hit.collider.gameObject.name);
				InstantiateBullet(hit.point);
			}
		}
	}

	void InstantiateBullet(Vector3 targetPos)
	{
		GameObject gobullet = AssetBundleManager.Instance.InstantiatePrefab<GameObject>("ABRes/bullet");
		Bullet bullet = gobullet.GetComponent<Bullet>();
		bullet.pointA = startPostObj.transform.position;
		bullet.pointB = targetPos;
	}
}