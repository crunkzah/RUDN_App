#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class GraphBuilder : MonoBehaviour {

	
	public GameObject nodePrefab;

	public Material default_mat;
	public Material selection_mat;


	public Transform cam_transform;

	public float moveSpeed = 8f;
	public float slowMoveSpeed = 3f;

	public enum gb_state { Moving, Naming, Connecting};

	public gb_state state = gb_state.Naming;
	
	public Node selectedNode;
	public void SelectNode(Node node)
	{
		if(selectedNode != null)
		{
			selectedNode.transform.localScale = Vector3.one;
			selectedNode.GetComponent<Renderer>().material = default_mat;
		}
		node.transform.localScale = Vector3.one * 1.3f;
		node.GetComponent<Renderer>().material = selection_mat;
		selectedNode = node;
	}
	public float heightAboveGround = 0.3f;

	public LayerMask pointMask;
	public LayerMask groundMask;

	public int nonRoomNextKey = 90000;
	public bool orthoMode = true;

	void Start()
	{
		
		CameraController2[] CameraRayCasters = FindObjectsOfType<CameraController2>();
		foreach(CameraController2 crc in CameraRayCasters)
			crc.enabled = false;
			
		SelectionManager2.singleton.enabled = false;

		if(cam_transform == null)
			cam_transform = FindObjectOfType<Camera>().transform;
		cam_transform.forward = Vector3.down;

		cam_transform.position = new Vector3(0f, cam_transform.position.y, 0f);
		nonRoomNextKey = FindNextNonRoomKey();
		
		cam_transform.GetComponent<Camera>().orthographic = orthoMode;
	}

	int FindNextNonRoomKey()
	{
		Node[] nodes = FindObjectsOfType<Node>();
		if(nodes == null || nodes.Length == 0)
			return 90000;

		int maxNumber = 90000;
		for(int i= 0; i < nodes.Length; i++)
		{
			if(!nodes[i].isRoom)
			{
				int intKey = int.Parse(nodes[i].nodeKey);
				if(intKey > maxNumber)
					maxNumber = intKey;
			}
		}
		Debug.Log("NonRoomNextKey is " + maxNumber.ToString());
		return maxNumber;
	}


	void Update()
	{
		if(Input.GetKeyDown(KeyCode.V))
		{
			switch(state)
			{
				case gb_state.Moving:
					state = gb_state.Connecting;
					break;
				case gb_state.Naming:
					Debug.LogWarning("Cant exit Naming state");
					break;
				case gb_state.Connecting:
					state = gb_state.Moving;
					break;
			}
		}
		switch(state)
		{
			case gb_state.Moving:
				Moving();
				break;
			case gb_state.Naming:
				Naming();
				break;
			case gb_state.Connecting:
				Connecting();
				break;
			default:
				Debug.LogError("State " + state.ToString() + " has no behaviour");
				break;
		}

	}
	void OnGUI()
	{
		if (Application.isEditor)  // or check the app debug flag
		{
			var w = 100;
			var h = 50;
			Rect rect = new Rect((Screen.width-w/2)/2, (Screen.height-h)/2, w, h);
			
			GUI.Label(rect, debugText);
			
			Rect rect2 = new Rect((Screen.width - w)/2, (Screen.height - h), w, h);
			GUI.Label(rect2, state.ToString());
		}
	}

	void Shooting()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Ray ray = cam_transform.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f,  0f));
			
			RaycastHit hit;
			Debug.Log(ray.origin);
			Debug.Log(ray.direction);
			
			if(Physics.Raycast(ray, out hit, 10000f, groundMask))
			{

				GameObject node_new = Instantiate(nodePrefab, hit.point + Vector3.up * heightAboveGround, Quaternion.identity);
				//lastPlacedNode = node_new.GetComponent<Node>();
				SelectNode(node_new.GetComponent<Node>());

				state = gb_state.Naming;
				Debug.Log("HIT " + hit.collider.name);
				Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue, 5f);
			}
		}
	}

	void Moving()
	{
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if(input.x != 0f || input.y != 0f)
			input.Normalize();

		// if(Input.GetKeyDown(KeyCode.Z) && selectedNode != null)
		// 	Destroy(selectedNode.gameObject);

		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = cam_transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 10000f, pointMask))
			{
				cam_transform.position = new Vector3(hit.collider.transform.position.x, cam_transform.position.y, hit.collider.transform.position.z);
			}
			else 
				if (Physics.Raycast(ray, out hit, 10000f, groundMask))
				{
					Vector3 point = RectangleCreator.singleton.CalculateCenterOfRectangle(hit.point, heightAboveGround);
					cam_transform.position = new Vector3(point.x, cam_transform.position.y, point.z);
				}
			
		}

		Shooting();
		float speed = Input.GetKey(KeyCode.LeftShift) ? slowMoveSpeed : moveSpeed;
		Vector3 motion = new Vector3(input.x, 0f, input.y) * speed;
		cam_transform.Translate(motion * Time.deltaTime, Space.World);
	}

	public string debugText;
	


	void Naming()
	{	
		foreach(char c in Input.inputString)
		{
			if(c == '\b')
			{
				if(debugText.Length > 0)
					debugText = debugText.Substring(0, debugText.Length - 1);
			}
			else if ((c == '\n') || (c == '\r'))
			{
				// state = gb_state.Connecting;
				

				// if(debugText != string.Empty)
				// {
				if(debugText == string.Empty)
					debugText = (++nonRoomNextKey).ToString();

				state = gb_state.Moving;
				selectedNode.name = "Point_" + debugText;
				selectedNode.nodeKey = debugText;
				
				selectedNode.isRoom = ((debugText[0] == '1') || (debugText[0] == '2') || (debugText[0] == '3') 
				|| (debugText[0] == '4') || (debugText[0] == '5') || (debugText[0] == '6')
				|| (debugText[0] == '7') || (debugText[0] == '8'));
				
				print("New node is " + "Point_" + debugText);
				debugText = string.Empty;
				// }
				
			}
			else
				debugText += c;
		}
	}

	void Connecting()
	{
		if(Input.GetMouseButtonDown(1))
		{
			Ray ray = cam_transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, 10000f, pointMask))
			{
				Node node = hit.collider.GetComponent<Node>();
				// lastPlacedNode = node;
				if(node != null)
				{
					SelectNode(node);
					Debug.Log("Selected node is " + node.name);
				}
			}
		}

		if(Input.GetMouseButtonDown(0))
		{
		
			if(selectedNode == null)
			{
				Debug.LogError("lastPlacedNode is null");
				return;
			}
			Ray ray = cam_transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, 10000f, pointMask))
			{
				Node node = hit.collider.GetComponent<Node>();
				if(node != null && selectedNode != null)
				{
					if(!Input.GetKey(KeyCode.LeftShift))
						PathFinder.singleton.AddRelation(node, selectedNode);
					else
						PathFinder.singleton.DeleteRelation(node, selectedNode);
				}

			}
		
		}

	}

}
#endif