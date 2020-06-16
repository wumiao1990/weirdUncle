using UnityEngine;
using UnityEngine.UI;

public class HonJin : MonoBehaviour
{
	public Button btn;
	// Use this for initialization
	void Start () {
		btn.onClick.AddListener(() =>
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Main");//直接加载，销毁掉原来的场景
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
