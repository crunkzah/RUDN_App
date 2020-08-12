using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLinkOpener : MonoBehaviour 
{

	public TMPro.TextMeshProUGUI tmp_label;
	public string link = string.Empty;

	public void Click()
	{
		Application.OpenURL(link);
	}
}
