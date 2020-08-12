using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyAnimator : MonoBehaviour {

	
	public Material[] materialsToAnimate;
	public Color[] colors;
	public float animationSpeed = 1f;
	public bool animateMaterials = true;


	void Start()
	{
		colors = new Color[materialsToAnimate.Length];
		for(int i = 0; i < materialsToAnimate.Length; i++)
			colors[i] = materialsToAnimate[i].GetColor("_Color");
	}

	public float current_value;

	public AnimationCurve curve;

	float GetAnimSpeed(float val)
	{
		return (val < 0.5f) ? animationSpeed / 2f : animationSpeed;
	}
	
	void Update()
	{
		if(materialsToAnimate == null || !animateMaterials)
			return;

		//current_value = Mathf.Cos(GetAnimSpeed(current_value) * Time.time);
		current_value = curve.Evaluate(Time.time);

		current_value =(current_value + 1f) / 2f;
		

		for(int i = 0; i < materialsToAnimate.Length; i++)
		{
			Color col = colors[i];
			col.a = current_value;
			materialsToAnimate[i].SetColor("_Color", col);
			

		}
	}

}
