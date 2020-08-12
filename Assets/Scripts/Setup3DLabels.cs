#if UNITY_EDITOR
using UnityEngine;
using TMPro;

[System.Serializable]
public struct LabelAndTranslation
{
	public string key;
	public TranslatableString translatable_string;
}

public class Setup3DLabels : MonoBehaviour {

	public LabelAndTranslation[] LabelsAndTranslations;

	public void SetupLabelsInScene()
	{
		if(UnityEditor.EditorApplication.isPlaying)
			return;
		
		GameObject label;
		

		for(int i = 0; i < LabelsAndTranslations.Length; i++)
		{
			label = GameObject.Find(LabelsAndTranslations[i].key + " Label3D");
			if(label == null)
				Debug.LogError("##### Label with key " + LabelsAndTranslations[i].key + " not found!!! #####");
			else
			{
				
				TextTranslator tt = label.GetComponent<TextTranslator>();
				if(tt == null)
					tt = label.AddComponent(typeof(TextTranslator)) as TextTranslator;
				
				tt.translatable_string = LabelsAndTranslations[i].translatable_string;
				tt.TranslateString();
			}
		}

		
	}

	public void ResizeLabels()
	{
		if(tmpsByName != null)
		{
			for(int i = 0; i < tmpsByName.Length; i++)
			{
				GameObject labelGo = GameObject.Find(tmpsByName[i] + " Label3D");
				if(labelGo != null)
				{
					labelGo.GetComponent<TextMeshPro>().fontSize = fontSize;
					print("Label " + tmpsByName[i] + " is now resized");
				}
				else
					Debug.LogWarning("Label with name " + tmpsByName[i] + " not found !!!");

			}
		}

		GameObject label301 = GameObject.Find(LabelNumber + " Label3D");
		if(label301 != null)
		{
			label301.transform.position = properPosition;
			print("Label 301 is fixed now.");
		}
	}

	[Header("Labels to resize:")]
	public string[] tmpsByName;
	public float fontSize = 14f;

	[Header("Label 301:")]
	public string LabelNumber = "301";
	public Vector3 properPosition;


	
}
#endif