using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleCreator : MonoBehaviour {

	#region Singleton
	static RectangleCreator _instance;
	public static RectangleCreator singleton
	{
		get{
			if(_instance == null)
				_instance = FindObjectOfType<RectangleCreator>();
			return _instance;
		}
	}
	#endregion

	public GameObject fromRectangle;
	public GameObject toRectangle;
	public LayerMask buildingMask;
	public float rayLength = 75f;
	public float rayEpsilon = 0.01f;
	public float rectAboveGround = 0.5f;

	MeshFilter fromMesh;
	MeshFilter toMesh;



	[Header("Test fields:")]
	public Transform pointTransformFrom;
	public Transform pointTransformTo;

	public void Awake()
	{
		fromMesh = fromRectangle.GetComponent<MeshFilter>();
		toMesh = toRectangle.GetComponent<MeshFilter>();
		if(fromMesh == null)
			Debug.LogError("FromMesh is null");
		if(toMesh == null)
			Debug.LogError("ToMesh is null");
	}

	void Start()
	{
		// fromRectangle.transform.position = pointTransformFrom.position;
		fromRectangle.transform.position = Vector3.zero;
		//toRectangle.transform.position = pointTransformTo.position;
		toRectangle.transform.position = Vector3.zero;

		//fromMesh.mesh = CreateRectangle(pointTransformFrom.position, rectAboveGround);
		//toMesh.mesh = CreateRectangle(pointTransformTo.position, rectAboveGround);
	}

	public void Hide()
	{
		fromMesh.GetComponent<Renderer>().enabled = false;
		toMesh.GetComponent<Renderer>().enabled = false;
	}

	public void DrawRects(Vector3 rectPoint1, Vector3 rectPoint2)
	{		
		fromMesh.GetComponent<Renderer>().enabled = true;
		fromMesh.mesh = CreateRectangle(rectPoint1, rectAboveGround);

		toMesh.GetComponent<Renderer>().enabled = true;
		toMesh.mesh = CreateRectangle(rectPoint2, rectAboveGround);	
	}


	//TODO: Before calculating rect need to enable coresponding floor!!!
	public void DrawRect1(Vector3 rectPoint1)
	{
		fromMesh.GetComponent<Renderer>().enabled = true;
		fromMesh.mesh = CreateRectangle(rectPoint1,rectAboveGround);
	}

	public void DrawRect2(Vector3 rectPoint2)
	{
		fromMesh.GetComponent<Renderer>().enabled = true;
		toMesh.mesh = CreateRectangle(rectPoint2, rectAboveGround);
	}


	public Vector3 CalculateCenterOfRectangle(Vector3 point, float aboveGroundOffset)
	{
		Vector3 result = Vector3.zero;
		RaycastHit hit;
		if(Physics.Raycast(point, Vector3.down, out hit, 10f, buildingMask))
			point.y = hit.point.y + aboveGroundOffset;
		Ray[] rays = new Ray[4];

		rays[0] = new Ray(point, Vector3.left);
		rays[1] = new Ray(point, Vector3.forward);
		rays[2] = new Ray(point, Vector3.right);
		rays[3] = new Ray(point, Vector3.back);

		Vector3[] r_points = new Vector3[4];
		for(int i = 0; i < r_points.Length; i++)
		{
			if(Physics.Raycast(rays[i], out hit, rayLength, buildingMask))
			{
				r_points[i] = hit.point;
				//Debug.DrawLine(rays[i].origin, hit.point, Color.magenta, 5f);
			}
		}

		Vector3 pointNearest = GetNearestPoint(point, r_points);
		Vector3 direction = (pointNearest - point).normalized;
		
		Debug.DrawLine(pointNearest, point, Color.cyan, 5f);


		List<Vector3> vertices = new List<Vector3>(4);
		
		Ray ray = new Ray(pointNearest - direction * rayEpsilon, GetVectorRotatedXZ(direction, 90f));
		Color[] colors = {Color.red, Color.blue, Color.green, Color.yellow};

		for(int i = 0; i < 4; i++)
		{
			if(Physics.Raycast(ray, out hit, rayLength, buildingMask))
			{
				
				Vector3 point_with_offset = hit.point - ray.direction * rayEpsilon;


				vertices.Add(point_with_offset); // abcd 0123
				Debug.DrawLine(ray.origin, point_with_offset, colors[i], 5f);

				ray.origin = point_with_offset;
				ray.direction = GetVectorRotatedXZ(ray.direction, 90f);
			}
			else
			{
				Debug.LogWarning("Ray didnt hit anything ");
				break;
			}
		}

		
		result = (vertices[0] + vertices[1] + vertices[2] + vertices[3]) / 4f;
		
		return result;
	}

	public Mesh CreateRectangle(Vector3 point, float aboveGroundOffset = 0.5f)
	{
		Mesh mesh = new Mesh();
		RaycastHit hit;
		if(Physics.Raycast(point, Vector3.down, out hit, 10f, buildingMask))
			point.y = hit.point.y + aboveGroundOffset;
		Ray[] rays = new Ray[4];

		rays[0] = new Ray(point, Vector3.left);
		rays[1] = new Ray(point, Vector3.forward);
		rays[2] = new Ray(point, Vector3.right);
		rays[3] = new Ray(point, Vector3.back);

		Vector3[] r_points = new Vector3[4];
		for(int i = 0; i < r_points.Length; i++)
		{
			if(Physics.Raycast(rays[i], out hit, rayLength, buildingMask))
			{
				r_points[i] = hit.point;
				//Debug.DrawLine(rays[i].origin, hit.point, Color.magenta, 5f);
			}
		}

		Vector3 pointNearest = GetNearestPoint(point, r_points);
		Vector3 direction = (pointNearest - point).normalized;
		
		//Debug.DrawLine(pointNearest, point, Color.cyan, 5f);


		List<Vector3> vertices = new List<Vector3>(4);
		
		Ray ray = new Ray(pointNearest - direction * rayEpsilon, GetVectorRotatedXZ(direction, 90f));
		Color[] colors = {Color.red, Color.blue, Color.green, Color.yellow};

		for(int i = 0; i < 4; i++)
		{
			if(Physics.Raycast(ray, out hit, rayLength, buildingMask))
			{
				
				Vector3 point_with_offset = hit.point - ray.direction * rayEpsilon;


				vertices.Add(point_with_offset); // abcd 0123
				Debug.DrawLine(ray.origin, point_with_offset, colors[i], 5f);

				ray.origin = point_with_offset;
				ray.direction = GetVectorRotatedXZ(ray.direction, 90f);
			}
			else
			{
				Debug.LogWarning("Ray didnt hit anything ");
				break;
			}
		}

		int[] tris = {0, 2, 1, 0, 3, 2};
		List<Vector2> uvs = new List<Vector2>();
		uvs.Add(Vector2.up);// a
		uvs.Add(Vector2.one);// b
		uvs.Add(Vector2.right);//c
		uvs.Add(Vector2.zero);//d

		mesh.SetVertices(vertices);
		mesh.SetTriangles(tris, 0);
		mesh.SetUVs(0, uvs);

		return mesh;
	}

// 	public Mesh CreateRectangle(Vector3 point)
// 	{
// 		//Offset point to ground:
// 		RaycastHit hit;
// 		if(Physics.Raycast(point, Vector3.down, out hit, 10f, buildingMask))
// 		{
// 			point.y = hit.point.y + 0.125f;
// 		}



// 		Mesh mesh = new Mesh();
// 		List<Vector3> vertices = new List<Vector3>(4);
// 		int[] tris = new int[6];

		
// 		//Ray r_right = new Ray(point, Vector3.right), r_left = new Ray(point, Vector3.left), r_forward = new Ray(point, Vector3.forward), r_back = new Ray(point, Vector3.back);

// 		Ray[] rays = new Ray[4];
// 		rays[0] = new Ray(point, Vector3.left);
		
// 		rays[1] = new Ray(point, Vector3.forward);
// 		rays[2] = new Ray(point, Vector3.right);
// 		rays[3] = new Ray(point, Vector3.back);
		
// 		Vector3[] r_points = new Vector3[4];
// 		for(int i = 0; i < 4; i++)
// 		{
// 			if(Physics.Raycast(rays[i], out hit, 75f, buildingMask))
// 			{
// 				r_points[i] = hit.point;
// 				Debug.DrawLine(rays[i].origin, hit.point, Color.magenta, 5f);
// 			}
// 		}

// 		Vector3 pointNearest = GetNearestPoint(point, r_points);

// 		Vector3 dirToNearest = (pointNearest - point).normalized;
// 		Vector3 toOrigin = pointNearest - point;
// //		Vector3 dirToNearest_Rotated = GetVectorRotatedXZ(dirToNearest, 90f);

		
// 		Debug.DrawLine(point, pointNearest, Color.green, 5f);

// 		Vector3 dir = (pointNearest - point).normalized;
// 		Vector3 dir2 = GetVectorRotatedXZ(dir, 90f);

// 		Ray ray2_positive = new Ray(pointNearest + dir2 * rayEpsilon, dir2);
// 		Ray ray2_negative = new Ray(pointNearest + dir2 * rayEpsilon, -dir2);
// 		Debug.Log("RayEpsilon is " + rayEpsilon.ToString());
// 		Vector3[] r_points2 = new Vector3[2];
// 		if(Physics.Raycast(ray2_positive, out hit, 75f, buildingMask))
// 		{
// 			r_points2[0] = hit.point;
// 			Debug.DrawLine(ray2_positive.origin, hit.point, Color.red, 5f);

// 			//points[0] = hit.point;
// 			vertices.Add(hit.point);//0 a
// 			Debug.DrawLine(hit.point, hit.point - toOrigin * 2f, Color.yellow, 5f);

// 			vertices.Add(hit.point - toOrigin * 2f);//1 b
// 		}

// 		if(Physics.Raycast(ray2_negative, out hit, 75f, buildingMask))
// 		{
// 			r_points2[1] = hit.point;
// 			Debug.DrawLine(ray2_negative.origin, hit.point, Color.blue, 5f);

// 			// points[1] = hit.point;
// 			vertices.Add(hit.point);//2 d

// 			Debug.DrawLine(hit.point, hit.point - toOrigin * 2f, Color.cyan, 5f);
// 			vertices.Add(hit.point - toOrigin * 2f);//3 c
// 		}

// 		tris[0] = 0; //a
// 		tris[1] = 3; //c
// 		tris[2] = 1; //b

// 		tris[3] = 0; //a
// 		tris[4] = 2; //d;
// 		tris[5] = 3; //c
		
// 		List<Vector2> uvs = new List<Vector2>(vertices.Count);

// 		uvs.Add(Vector2.up);//a
// 		uvs.Add(Vector2.one);//b
// 		uvs.Add(Vector2.zero);//c
// 		uvs.Add(Vector2.right);//d

		
// 		mesh.SetVertices(vertices);
// 		mesh.SetTriangles(tris, 0);
// 		mesh.SetUVs(0, uvs);
// 		//mesh.SetNormals(normals);
		
// 		return mesh;
// 	}

	Vector3 GetVectorRotatedXZ(Vector3 v, float angle = 90f)
    {
        angle *= Mathf.Deg2Rad;
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);

        Vector3 r = new Vector3();
        r.x = v.x * cos - v.z * sin;
        r.y = v.y;
        r.z = v.x * sin + v.z * cos;

        return r;
    }

	Vector3 GetNearestPoint(Vector3 origin, Vector3[] points)
	{
		Vector3 result = Vector3.zero;
		float minDist = float.MaxValue;
		for(int i = 0; i < points.Length; i++)
		{
			float dist = Vector3.Distance(origin, points[i]);
			if(dist < minDist)
			{
				minDist = dist;
				result = points[i];
			}

		}	

		return result;
	}

}
