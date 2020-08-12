using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCustomScaler : MonoBehaviour {

	RectTransform rectTransform;
	
	public Rect safeArea;
	
	Rect originalRect;
	Rect iphoneXRect;
	Rect androidRect;
	
	
	
	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}
	
	void Start()
	{
		safeArea = Screen.safeArea;
	}
	
}
