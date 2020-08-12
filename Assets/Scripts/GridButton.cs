using UnityEngine;
using TMPro;
public class GridButton : MonoBehaviour 
{
	public GameObject parentCanvas;

	public TMP_InputField inputField;
	TextMeshProUGUI tmp;


	public bool overrideNodeKey = false;
	public string nodeKeyToOverride = "";

	void Awake()
	{
		tmp = GetComponentInChildren<TextMeshProUGUI>();
		
	}

	public void FindNode()
	{
		inputField = SelectionManager2.singleton.chooseFrom ? PathFinder.singleton.from_InputField : PathFinder.singleton.destination_InputField;

		if(overrideNodeKey)
		{	
			inputField.text = nodeKeyToOverride;
		}
		else
			inputField.text = tmp.text;
			
		DisableCanvas();
	}

	public void DisableCanvas()
	{
		if(parentCanvas != null)
			parentCanvas.SetActive(false);
	}
}
