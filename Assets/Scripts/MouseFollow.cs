using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour {

	VolumetricLineRenderer vlr;
	public LayerMask mask;
	Camera cam;
	void Start()
	{
		vlr = GetComponent<VolumetricLineRenderer>();
		cam = FindObjectOfType<Camera>();
		//cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
	}

	// GameObject cube;

	void Update()
	{
		if(Input.GetMouseButton(0))
		{
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, 150f, mask))
			{
				Vector3 p = new Vector3(hit.point.x, vlr.points[vlr.points.Count - 1].y, hit.point.z);
				vlr.points[vlr.points.Count - 1] = p;
				//cube.transform.position = p;
			}
			vlr.RedrawLines();
		}
	}


}
