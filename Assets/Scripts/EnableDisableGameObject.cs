using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableGameObject : MonoBehaviour {

	public GameObject gameObjectToToggle;
	public void ToggleGameObject()
	{
		if(gameObjectToToggle != null)
			gameObjectToToggle.SetActive(!gameObjectToToggle.activeInHierarchy);
	}
}
