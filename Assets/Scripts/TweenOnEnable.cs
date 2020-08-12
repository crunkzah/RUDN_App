using UnityEngine;
using System.Collections;

public class TweenOnEnable : MonoBehaviour {

	public float tweeningTime = 0.3f;

	public float hiddenYCoord = -1200f;
	float velocity;

	RectTransform rectTransform;
	Camera cam;
	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		cam = FindObjectOfType<Camera>();
		if(rectTransform == null)
			Debug.LogError("RectTransform not found !!!");
	}

	void OnEnable()
	{
		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, hiddenYCoord);
		//hiddenYCoord = -Screen.currentResolution.height;
		hiddenYCoord = -cam.pixelHeight;
		ShowUp();
	}


	public void ShowUp()
	{
		hiddenYCoord = -cam.pixelHeight;
		StopAllCoroutines();
		StartCoroutine(TweenUp());
	}

	public void HideDown()
	{
		hiddenYCoord = -cam.pixelHeight;
		StopAllCoroutines();
		StartCoroutine(TweenDown());
	}

	IEnumerator TweenUp()
	{
		float timePassed = 0f;
		while(timePassed < tweeningTime * 5f)
		{
			
			float y = Mathf.SmoothDamp(rectTransform.anchoredPosition.y, 0f, ref velocity, tweeningTime);

			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);

			timePassed += Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator TweenDown()
	{
		float timePassed = 0f;
		while(timePassed < tweeningTime * 5f)
		{
			
			float y = Mathf.SmoothDamp(rectTransform.anchoredPosition.y, hiddenYCoord, ref velocity, tweeningTime);

			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);

			timePassed += Time.deltaTime;
			yield return null;
		}

		//HACK: NOT COOL TO DISABLE PARENT CANVAS;
		this.transform.parent.gameObject.SetActive(false);
	}

}
