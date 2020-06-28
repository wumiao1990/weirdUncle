using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainManager : MonoBehaviour
{
	public RectTransform canvasRT;
	public Button btn;
	public Button btn1;
	public Button btn2;
	public Button btn3;
	public RawImage rawImage;
	public VideoPlayer vPlayer;
	// Use this for initialization
	void Start () {
		btn.onClick.AddListener(() =>
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("HonJin");//直接加载，销毁掉原来的场景
		});
		
		btn1.onClick.AddListener(() =>
		{
			#if UNITY_EDITOR
				PlayVideo("1");
			#else
				Handheld.PlayFullScreenMovie("1.mp4", Color.white, FullScreenMovieControlMode.CancelOnInput );
			#endif
		});
		btn2.onClick.AddListener(() =>
        {
			#if UNITY_EDITOR
				PlayVideo("2");
			#else
				Handheld.PlayFullScreenMovie("2.mp4", Color.white, FullScreenMovieControlMode.CancelOnInput );
			#endif
        });
		btn3.onClick.AddListener(() =>
		{
			#if UNITY_EDITOR
				PlayVideo("1");
			#else
				Handheld.PlayFullScreenMovie("3.mp4", Color.white, FullScreenMovieControlMode.CancelOnInput );
			#endif
		});
		
		ScaleRawImage();
	}

	void PlayVideo(string name)
	{
		VideoClip clip = Resources.Load<VideoClip>(name);
		rawImage.Active();
		vPlayer.clip = clip;
		RenderTexture renderTexture = Resources.Load<RenderTexture>("videoTexture");
		if (renderTexture != null)
		{
			vPlayer.targetTexture = renderTexture;
			rawImage.texture = renderTexture;
		}
		vPlayer.loopPointReached += EndReached;
		vPlayer.Play();
	}

	void EndReached(VideoPlayer vPlayer)
	{
		rawImage.SetActive(false);
	}
	
	void ScaleRawImage()
	{
//		Vector2 spriteSize = new Vector2();
//		Vector2 imgSize = new Vector2(rawImage.texture.width, rawImage.texture.height);//图片的实现大小
//		Vector2 canvasSize = canvasRT.sizeDelta;    // 画布的大小，代码手机屏幕的大小
//		if (imgSize.y / imgSize.x > canvasSize.y / canvasSize.x)
//		{
//			spriteSize.x = canvasSize.x;
//			spriteSize.y = imgSize.y * (spriteSize.x / imgSize.x);
//		}
//		else
//		{
//			spriteSize.y = canvasSize.y;
//			spriteSize.x = imgSize.x * (spriteSize.y / imgSize.y);
//		}
		rawImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
	}
}
