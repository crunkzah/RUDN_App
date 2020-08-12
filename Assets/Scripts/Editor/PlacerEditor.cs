#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Placer))]
public class PlacerEditor : Editor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		Placer t = (Placer)target;
		if(GUILayout.Button("Place to grid"))
		{
			t.PlaceToGrid();
		}
		if(GUILayout.Button("Revert positions"))
		{
			t.RevertPositions();
		}
	}
}
#endif