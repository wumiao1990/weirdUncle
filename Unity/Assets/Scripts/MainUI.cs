using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MainUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetBindComponents(gameObject);
		
		m_BtnTest2.onClick.AddListener(() =>
		{
			Debug.LogError("dsdsdfsaf");
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
