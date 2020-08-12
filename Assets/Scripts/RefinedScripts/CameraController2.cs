using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController2 : MonoBehaviour 
{
    static CameraController2 _instance;

    public static CameraController2 singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraController2>();
            }

            return _instance;
        }
    }


	[Header("Settings:")]
	// (Marat) 	raycastMask should contain layer of Labels
	//			raycastMask - маска должна содержать слой с label на карте
	public LayerMask raycastMask;
    public float moveScale 		= 35f;
    public float timeToSwipe 	= 0.15f;
	public float floorHeight 	=  4.341f;

    public Vector3 selectedNodeOffset = new Vector3(0f, 27.2f, -28.05f);

    // (Marat) 	world space coord limits, so camera does not fly out of bounds
    //			координаты в глобальном пространстве, чтобы камера не улетала за границы
    public Vector2 coordXLimit = new Vector2(-500f, 500f);
    public Vector2 coordZLimit = new Vector2(-500f, 500f);

	public bool wasUIClicked;

	Camera cam;

    // (Marat) Touches:
    Vector3 touchDownPos;
	Vector3 prevTouchPos;
	Vector3 cameraTouchDownPos;

	// (Marat) 	Camera related positions and zoom variables:
	//			Позиции камеры и переменные для зума:
	[Header("Zooming:")]
	public float maxY 		= 210f;
	public float minY		= 16f;
	public float smoothTime = 0.25f;
	public float zoomScale 	= 80f;
	

	Vector3 unzoomedPosition;
	[SerializeField] Vector3 targetPosition;

	Vector3 dampVector;
	float nextTimeToSwipe;

	void Start()
	{
		cam = GetComponent<Camera>();
        unzoomedPosition = transform.position;
        targetPosition = unzoomedPosition;
	}


    List<RaycastResult> results = new List<RaycastResult>();

    bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        results.Clear();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

	public void FloorUp()
	{
		targetPosition += Vector3.up * floorHeight;
		targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        minY += floorHeight;
	}

	public void FloorDown()
	{
        targetPosition += Vector3.down * floorHeight;
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

		minY -= floorHeight;
	}

    public void ZoomIn(float amount)
    {
		Vector3 zoomedPosition = targetPosition + transform.forward * zoomScale * amount;
		if(zoomedPosition.y > minY && zoomedPosition.y < maxY)
		{
			transform.position = zoomedPosition;
			targetPosition = zoomedPosition;
		}
    }

    public void ZoomOut(float amount)
    {
        Vector3 zoomedPosition = targetPosition - transform.forward * zoomScale * amount;
        if (zoomedPosition.y > minY && zoomedPosition.y < maxY)
        {
            transform.position = zoomedPosition;
            targetPosition = zoomedPosition;
        }
    }

    void KeyboardInput()
	{
        float mouseScroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if(mouseScroll > 0f)
        {
            ZoomIn(0.1f);
        }

        if(mouseScroll < 0f)
        {
            ZoomOut(0.1f);
        }

        if(Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
        {
            ZoomIn(0.2f);
        }

        if(Input.GetKeyDown(KeyCode.Minus))
        {
            ZoomOut(0.2f);
        }

		

        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     floor++;
        //     floor = Mathf.Clamp(floor, 1, 8);
        //     SetFloor(floor);
        // }
        // else
        //     if (Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     floor--;
        //     floor = Mathf.Clamp(floor, 1, 8);
        //     SetFloor(floor);
        // }
    }

	void Update()
	{
        if(IsPointerOverUIObject())
        {
            if(Input.GetMouseButtonDown(0))
            {
                wasUIClicked = true;
            }
        }

        List<Touch> touches = InputHelper.GetTouches();


        if(!wasUIClicked && Input.GetMouseButtonDown(0))
        //			if(touches[0].phase == TouchPhase.Began)
        {

            touchDownPos = cam.ScreenToViewportPoint(Input.mousePosition);
            cameraTouchDownPos = transform.position;

            nextTimeToSwipe = Time.time + timeToSwipe;
        }
        if(!wasUIClicked && Input.GetMouseButton(0))
        //			if(touches[0].phase == TouchPhase.Moved)
        {
            //if(SelectionManager.singleton.selectedNode != null)
            //	return;

            Vector3 dir = touchDownPos - cam.ScreenToViewportPoint(Input.mousePosition);
            dir.z = dir.y;
            dir.y = 0;

            Vector3 newPosition = cameraTouchDownPos + dir * moveScale;
            newPosition.x = Mathf.Clamp(newPosition.x, coordXLimit.x, coordXLimit.y);
            newPosition.z = Mathf.Clamp(newPosition.z, coordZLimit.x, coordZLimit.y);

            transform.position = newPosition;

			targetPosition = newPosition;
			

        }
        else
            prevTouchPos = Vector3.zero;

        if(Input.GetMouseButtonUp(0))
        //			if(touches[0].phase == TouchPhase.Ended)
        {
            wasUIClicked = false;

            if(Time.time < nextTimeToSwipe)
			{
                RaycastToLabel();
			}

        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
		KeyboardInput();
#endif

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref dampVector, smoothTime);
        //transform.position = Vector3.SmoothDamp(transform.position, GetZoomedTargetPosition(), ref dampVector, smoothTime);
        
    }

	// (Marat) 	Shooting ray from camera, hoping to touch labels
	//			Пускает луч из камеры по лейблам
    void RaycastToLabel()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 300f, raycastMask))
        {
			Node nodeThatWasHit = hit.collider.GetComponentInParent<Label_node>().node;

			if(SelectionManager2.singleton.selectedNode == null 
				|| (SelectionManager2.singleton.selectedNode != nodeThatWasHit))
			{
				SelectionManager2.singleton.SelectNode(nodeThatWasHit);
			}
			else
			{
				targetPosition = nodeThatWasHit.labelPos + selectedNodeOffset;
                //targetPos = node.labelPos + selectNodeOffset;
            }
        }
    }

    public void SetMiddlePoint(Vector3 p1, Vector3 p2)
    {
        float zMiddleCoord = (p1.z + p2.z) / 2f;
        float yMiddleCoord = (p1.y + p2.y) / 2f;

        float xMiddleCoord = (p1.x + p2.x) / 2f;

        targetPosition = new Vector3(xMiddleCoord, targetPosition.y, targetPosition.z);

        SelectionManager2.singleton.DeselectNode(false);
    }

	
}
