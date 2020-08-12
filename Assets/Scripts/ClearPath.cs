using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPath : MonoBehaviour {

	public void Click()
	{
		PathFinder.singleton.from_InputField.text = string.Empty;
        PathFinder.singleton.destination_InputField.text = string.Empty;
	}
}
