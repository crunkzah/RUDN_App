using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Setup3DLabels))]
public class Setup3DLabelsEditor : Editor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		Setup3DLabels t = (Setup3DLabels)target;
		if(GUILayout.Button("Setup Labels"))
		{
			t.SetupLabelsInScene();
		}
		// if(GUILayout.Button("Lower nodes by 0.1"))
		// {
		// 	Node[] nodes = FindObjectsOfType<Node>();
		// 	for(int i = 0; i < nodes.Length; i++)
		// 		nodes[i].transform.position = new Vector3(nodes[i].transform.position.x, nodes[i].transform.position.y - 0.1f, nodes[i].transform.position.z);
		// }
		// if(GUILayout.Button("Raise nodes by 0.1"))
		// {
		// 	Node[] nodes = FindObjectsOfType<Node>();
		// 	for(int i = 0; i < nodes.Length; i++)
		// 		nodes[i].transform.position = new Vector3(nodes[i].transform.position.x, nodes[i].transform.position.y + 0.1f, nodes[i].transform.position.z);
		// }
		if(GUILayout.Button("Resize labels"))
		{
			t.ResizeLabels();
		}
	}
}
