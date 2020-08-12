using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class TogglePostEffects : MonoBehaviour {
	PostProcessingBehaviour pph;
	
	void Start()
	{
		Camera cam = FindObjectOfType<Camera>();
		pph = cam.GetComponent<PostProcessingBehaviour>();
	}

	public void TogglePostFx()
	{
		pph.enabled = !pph.enabled;
	}
}
