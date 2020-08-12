using UnityEngine;

[CreateAssetMenu(fileName = "New TranslatableString", menuName = "TranslatableString")]
public class TranslatableString : ScriptableObject 
{
	[TextArea(6, 10)]
	public string[] strings;
}
