using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HonJin : MonoBehaviour
{
	public Button btn;
	// Use this for initialization
	void Start () {
		btn.onClick.AddListener(() =>
		{
			SceneManager.LoadScene("Main");//直接加载，销毁掉原来的场景
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
