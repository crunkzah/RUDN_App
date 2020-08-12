using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinterModeButton : MonoBehaviour {

	public GameObject img_checkmark;

	public bool winterMode = false;
	public string yard_key = "Yard";
	public Node yard_node;
	int winterModePrefs;
	void Start()
	{
		if(yard_node == null)
		{
			yard_node = GameObject.Find("Point_1401").GetComponent<Node>();
			
		}

        foreach (Node neighbour in yard_node.neighbours)
        {
            yard_nonWinterMode_neighbours.Add(neighbour);
        }

		yard_winterMode_neighbours.Add(yard_node.neighbours[0]);

        winterModePrefs = PlayerPrefs.GetInt("WinterMode", 0);
		winterMode = (winterModePrefs == 1);

		ChangeNode(winterMode);

		img_checkmark.SetActive(winterMode);
	}

	public void Click()
	{
		winterMode = !winterMode;
		winterModePrefs = winterMode ? 1 : 0;
		PlayerPrefs.SetInt("WinterMode", winterModePrefs);

		img_checkmark.SetActive(winterMode);
		ChangeNode(winterMode);
		PathFinder.singleton.FindPathFromInputFields();
	}

	public List<Node> yard_nonWinterMode_neighbours = new List<Node>();
	public List<Node> yard_winterMode_neighbours = new List<Node>();

	public void ChangeNode(bool mode)
	{
		if(mode)
		{
			yard_node.neighbours.Clear();
			// yard_node.neighbours = new List<Node>(yard_winterMode_neighbours);
		}
		else
		{
			yard_node.neighbours = new List<Node>(yard_nonWinterMode_neighbours);
		}

		
	}
}
