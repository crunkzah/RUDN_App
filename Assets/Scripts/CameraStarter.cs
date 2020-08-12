using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStarter : MonoBehaviour {


    // CameraRayCaster cameraController;
    CameraController2 cameraController;


	public float farLevel = 3f;
	public float timeToStart = 1f;
	public Vector3 startPos, endPos;

	bool isStarted = false;
	////////

	void Awake()
	{
        // cameraController = GetComponent<CameraRayCaster>();
		SelectionManager2.singleton.InitialFloorSetup();
        SelectionManager2.singleton.enabled = false;
        cameraController = GetComponent<CameraController2>();

		startPos = transform.position - transform.forward * farLevel; 
		endPos = transform.position;

		cameraController.enabled = false;
	}
	void Start()
	{
        timeToActivate = Time.time + timeToStart;
	}

	public float timeToActivate;

	Vector3 vel =  Vector3.zero;

	void Update()
	{
		if(!isStarted)
		{
			isStarted = true;
			transform.position = startPos;
		}

		if(Time.time > timeToActivate * 2)
		{
			cameraController.enabled = true;
			SelectionManager2.singleton.enabled = true;
			Destroy(this);
		}

		transform.position = Vector3.SmoothDamp(this.transform.position, endPos, ref vel, timeToStart);
	}


}
