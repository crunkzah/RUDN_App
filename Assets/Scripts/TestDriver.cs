#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TestDriver : MonoBehaviour {

	public float timeInterval = 0.5f;
	Node[] nodes;
	VolumetricLineRenderer vlr;


	void Awake()
	{
		vlr = FindObjectOfType<VolumetricLineRenderer>();
	}

	public void StartTest()
	{
		StopAllCoroutines();
		StartCoroutine(TestPathfinding());
	}

	public IEnumerator TestPathfinding()
	{
		nodes = FindObjectsOfType<Node>();

		for(int i = 0; i < nodes.Length; i++)
			for(int j = 0; j < nodes.Length; j++)
			{
				if(!nodes[i].isRoom || !nodes[j].isRoom)
					continue;
				if(i == j)
					continue;
				yield return new WaitForSeconds(timeInterval);

				List<Vector3> path;
				PathFinder.singleton.FindPath(nodes[i], nodes[j], out path);
				if(path == null)
				{
					Debug.LogWarning(nodes[i].nodeKey + " and " + nodes[j].nodeKey + " has problems");
					UnityEditor.EditorApplication.isPaused = true;

				}
				if(path.Count == 1)
				{
					Debug.LogWarning(nodes[i].nodeKey + " and " + nodes[j].nodeKey + " has no path");
					UnityEditor.EditorApplication.isPaused = true;
				}


				
				vlr.points = path;
				RectangleCreator.singleton.DrawRects(vlr.points[0], vlr.points[vlr.points.Count - 1]);
				vlr.RedrawLines();
			}
		Debug.Log("Test ended");
	}


	void Update()
	{
		if(Input.GetKeyDown(KeyCode.T))
		{
			StopAllCoroutines();
			StartTest();
		}

		if(Input.GetKeyDown(KeyCode.U))
			StopTest();
	}

	public void StopTest()
	{
		StopAllCoroutines();
	}
}
#endif