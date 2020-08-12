
#if UNITY_EDITOR
using UnityEngine;



public class Placer : MonoBehaviour {

	public Transform buildingHolder;
	public float gridSize = 50f;

	public void RevertPositions()
	{
		if(buildingHolder == null)
		{
			Debug.Log("BuildingHolder is null");
			return;
		}

		foreach(Transform child in buildingHolder)
		{
			Debug.Log(child.name);
			child.localPosition = Vector3.zero;
		}
	}

	public void PlaceToGrid()
	{
		if(buildingHolder == null)
		{
			Debug.Log("BuildingHolder is null");
			return;
		}

		int k = 0;
		foreach(Transform child in buildingHolder) 
		{
			child.localPosition = Vector3.zero + Vector3.right * gridSize * k;
			k++;
		}
	}

}
#endif