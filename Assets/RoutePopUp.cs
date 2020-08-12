using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoutePopUp : MonoBehaviour 
{

	public Image[] images;
	public TextMeshProUGUI[] tmps;


	public float time_to_animate = 0.66f;

	[SerializeField]
	bool isAnimating = false;
	[SerializeField]
	bool isActivating = false;

	public void Show()
	{
		this.gameObject.SetActive(true);
		
	}

	// void Update()
	// {
	// 	if(isAnimating)
	// 	{

	// 	}
	// }

	public void Hide(bool force = false)
	{
		if(force)
			this.gameObject.SetActive(false);
		else
            this.gameObject.SetActive(false);
	}
}
