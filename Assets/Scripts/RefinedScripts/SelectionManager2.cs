using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectionManager2 : MonoBehaviour 
{
	static SelectionManager2 _instance;

	public static SelectionManager2 singleton
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<SelectionManager2>();
			}

			return _instance;
		}
	}


    [Header("GUI Bindings:")]
    public GameObject popUpCanvas;
    public RoutePopUp route_pop_up;
    public RoomInfoPanel roomInfoPanel;
    public TextMeshProUGUI floorLabel;

    [Header("Floors:")]
    public int currentFloor = 1;
    public GameObject[] floors;
    public GameObject[] labelsByFloors;

	[Header("Selected node:")]
	public Node selectedNode;

    float selectedNodeLift = 1f;
    
    bool firstStartUp = true;


	void Awake()
	{
        if(popUpCanvas != null)
        {
            route_pop_up = popUpCanvas.GetComponent<RoutePopUp>();
        }
        InitialFloorSetup();
        RedrawFloorGUILabel();
	}	

	public void InitialFloorSetup()
	{
        for (int i = 0; i < labelsByFloors.Length; i++)
        {
            bool should_labels_be_active = false;

            if (i == (currentFloor - 1))
            {
                should_labels_be_active = true;
            }

            SwitchMeshRenderer(labelsByFloors[i], should_labels_be_active);
        }
        RedrawFloorGUILabel();
	}
    
    public bool chooseFrom = true;
    public void ChooseFromOrDestination(bool from)
    {
        chooseFrom = from;
    }

    public void RedrawFloorGUILabel()
    {
        
        if(floorLabel != null)
        {
            switch(TextTranslator.current_language)
            {
                case TextTranslator.LANG.RU:
                    floorLabel.SetText("Этаж " + currentFloor.ToString());
                    break;

                case TextTranslator.LANG.ENG:
                    floorLabel.SetText("Floor " + currentFloor.ToString());
                    break;
            }
            
            
        }
    }

    public void FloorUp()
    {
		DeselectNode();

        currentFloor++;
		if(currentFloor >= 1 && currentFloor <= 8)
		{
			CameraController2.singleton.FloorUp();
		}

		currentFloor = Mathf.Clamp(currentFloor, 1, 8);

		SetFloor(currentFloor);
    }

    public void FloorDown()
    {
        DeselectNode();

        currentFloor--;
        if(currentFloor >= 1 && currentFloor <= 8)
		{
        	CameraController2.singleton.FloorDown();
		}

        currentFloor = Mathf.Clamp(currentFloor, 1, 8);

        SetFloor(currentFloor);
    }

    void SwitchMeshRenderer(GameObject parent, bool value)
    {
        foreach (Transform child in parent.transform)
        {
            child.GetComponent<MeshRenderer>().enabled = value;
            MeshRenderer mr;
            Collider collider;
            foreach (Transform grandChild in child)
            {
                mr = grandChild.GetComponent<MeshRenderer>();
                collider = grandChild.GetComponent<Collider>();
                if (mr != null)
                    mr.enabled = value;
                if (collider != null)
                    collider.enabled = value;
            }
        }
    }

    public void CheckNode_visibility(Node node_to_check)
    {

        if (currentFloor <= labelsByFloors.Length)
        {

            if (node_to_check != null && !node_to_check.nodeKey.Equals("1401"))
            {
                int floorIndex = currentFloor - 1;
                if (node_to_check.label.transform.parent != labelsByFloors[floorIndex])
                {
                    bool val = false;
                    if (PathFinder.singleton.startNode != null)
                    {
                        val = (node_to_check.nodeKey.Equals(PathFinder.singleton.startNode.nodeKey));
                    }

                    if (PathFinder.singleton.endNode != null)
                    {
                        val = val || (node_to_check.nodeKey.Equals(PathFinder.singleton.endNode.nodeKey));
                    }

                    node_to_check.label.GetComponent<MeshRenderer>().enabled = val;

                    Transform node_collider_plane = node_to_check.label.transform.GetChild(0);
                    if (node_collider_plane != null)
                    {
                        MeshRenderer mr = node_collider_plane.GetComponent<MeshRenderer>();
                        if (mr != null)
                            mr.enabled = val;
                    }

                    Transform tmp_submesh = node_to_check.label.transform.GetChild(1);
                    if (tmp_submesh != null)
                        tmp_submesh.GetComponent<MeshRenderer>().enabled = val;
                }
            }
        }
    }

    public void SetFloor(int newFloor)
    {
        // if (selectedNode != null)
        //     return;

        newFloor = Mathf.Clamp(newFloor, 1, 8);

        RedrawFloorGUILabel();


        for (int i = 0; i < floors.Length; i++)
        {
			bool should_floor_be_active = false;

			if(i == (newFloor - 1))
			{
                should_floor_be_active = true;
			}

			floors[i].SetActive(should_floor_be_active);
        }

		for (int i = 0; i < labelsByFloors.Length; i++)
		{
            bool should_labels_be_active = false;

			if (i == (newFloor - 1))
			{
                should_labels_be_active = true;
			}

			SwitchMeshRenderer(labelsByFloors[i], should_labels_be_active);
		}

        CheckNode_visibility(PathFinder.singleton.startNode);
        CheckNode_visibility(PathFinder.singleton.endNode);

    }

    public void SelectNode(Node node)
    {
        if(selectedNode != null)
            DeselectNode();

        selectedNode = node;


        if (selectedNode.label != null)
        {
            selectedNode.label.transform.localScale = Vector3.one * 1.3f;
            selectedNode.label.transform.Translate(Vector3.up * selectedNodeLift, Space.World);
        }

        if (popUpCanvas != null)
        {
            route_pop_up.Show();
            AdditionalRoomInfo room_info = node.GetComponent<AdditionalRoomInfo>();
            if (room_info != null)
            {
                if (room_info.room_name != null)
                {
                    roomInfoPanel.SetRoomName(room_info.room_name);
                }
                if (!string.IsNullOrEmpty(room_info.link))
                {
                    roomInfoPanel.SetLink(room_info.link, room_info.link_label);
                }
                else
                {
                    roomInfoPanel.linkButton.gameObject.SetActive(false);
                }
                roomInfoPanel.SetRoomInfo(room_info.t_string);
                roomInfoPanel.Show();
            }
            //	popUpCanvas.SetActive(true);
        }

    }

    public void DeselectNode(bool returnToDefaultPosition = true)
    {
        if (selectedNode == null)
            return;

        if (selectedNode.label != null)
        {
            selectedNode.label.transform.localScale = Vector3.one;
            selectedNode.label.transform.Translate(Vector3.down * selectedNodeLift, Space.World);
        }

        selectedNode = null;

        if (popUpCanvas != null)
        {
            route_pop_up.Hide(true);

            roomInfoPanel.Hide();
        }
    }

    public void AssignFromNode()
    {
        PathFinder.singleton.SetStartNode(selectedNode);
    }

    public void AssignToNode()
    {
        PathFinder.singleton.SetEndNode(selectedNode);
    }


	void Update()
	{
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            FloorUp();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            FloorDown();
        }
        
        
        if(firstStartUp)
        {
            firstStartUp = false;
            FloorDown();
        }
	}

}
