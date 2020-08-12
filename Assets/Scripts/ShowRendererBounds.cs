using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRendererBounds : MonoBehaviour {

	public bool drawGizmos = true;

	
	Renderer rend;

	void OnEnable()
	{
		rend = GetComponent<Renderer>();
	}


	void OnDrawGizmosSelected()
	{
		if(drawGizmos)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(rend.bounds.center, rend.bounds.size);
		}
	}
}
