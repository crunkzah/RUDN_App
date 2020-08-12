using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VlrDriver : MonoBehaviour {

	void Start()
	{
		GetComponent<VolumetricLineRenderer>().RedrawLines();
	}
}
