using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetupLabels))]
public class SetupLabelsEditor : Editor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		SetupLabels t = (SetupLabels)target;

		if(GUILayout.Button("Setup labels"))
		{
			t.Setup();
		}
	}
		
}
