using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PostProcessing;
public class AntiAliasingSwitcher : MonoBehaviour {

	TextMeshProUGUI tmp;
	PostProcessingBehaviour ppb;
	void Start()
	{
		tmp = GetComponentInChildren<TextMeshProUGUI>();
		
		ppb = FindObjectOfType<Camera>().GetComponent<PostProcessingBehaviour>();
		tmp.text = ppb.profile.antialiasing.enabled ? "TAA: on" : "TAA: off";
	}


	public void SwitchAntiAliasing()
	{
		ppb.profile.antialiasing.enabled = !ppb.profile.antialiasing.enabled;
		tmp.text = ppb.profile.antialiasing.enabled ? "TAA: on" : "TAA: off";
		// GameObject[] labels = GameObject.FindGameObjectsWithTag("Label3d");
		// for(int i = 0; i < labels.Length; i++)
		// {
		// 	bool isEnabled = labels[i].GetComponent<Renderer>().enabled;
		// 	labels[i].GetComponent<Renderer>().enabled = !isEnabled; 
		// }
	}
}
