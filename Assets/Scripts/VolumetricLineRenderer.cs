using UnityEngine;
using System.Collections.Generic;

public class VolumetricLineRenderer : MonoBehaviour 
{

    [Header("Pointer settings:")]
    public GameObject   pointer_prefab;
    public float        distance_sqr_threshold = 0f;
    public Vector3      pointer_scale = Vector3.one;

    [Header("Points:")]
    public List<Vector3> points;
    [SerializeField]float radius = 1f;
    [SerializeField]int circleResolution = 32;

    [SerializeField]GameObject lineObj;
    MeshFilter meshFilter;
    [Header("Material")]
    public Material material;
    public float lineWidth = 0.75f;



    new Renderer renderer;
    void Start()
    {
        lineObj = new GameObject("Line GameObject");
        lineObj.AddComponent(typeof(MeshFilter));
        lineObj.AddComponent(typeof(MeshRenderer));
        
        lineObj.GetComponent<Renderer>().material = this.material;
        lineObj.GetComponent<Renderer>().receiveShadows = false;
        lineObj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        
        meshFilter = lineObj.GetComponent<MeshFilter>();

        renderer = lineObj.GetComponent<Renderer>();
        //Init();
    }

    public void Hide()
    { 
        // renderer.enabled = false;
        lineObj.SetActive(false);
    }

    public void RedrawLines(float pathLength = 0f)
    {
        
        Debug.Log(string.Format("<color=yellow>Path Length is {0}</color>", pathLength));
        
        lineObj.SetActive(true);
        meshFilter.mesh = CreateMesh(pathLength);    
    }


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

    Vector3 GetVectorRotatedY(Vector3 v, float angle)
    {
        angle *= Mathf.Deg2Rad;
        return v;
    }

    void MakeCircle(Vector3 center, float r, int resolution, ref List<Vector3> verts, ref int vertIndex, ref int[] tris, ref int trisIndex)
    {
        int vertIndexCenter = vertIndex;
        
        verts[vertIndex++] = center;
        float angleStep = 2 * Mathf.PI / resolution;

        float angle = 0f;
        for(int i = 0; i < resolution; i++)
        {
            Vector3 point = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * r;
            verts[vertIndex] = point;
            angle += angleStep;

            vertIndex++;
        }

        int k;
        for(k = 0; k < resolution - 1; k++)
        {
            tris[trisIndex] = vertIndexCenter;
            
            tris[trisIndex + 2] = vertIndexCenter + k + 1;
            tris[trisIndex + 1] = vertIndexCenter + k + 2;
               
            trisIndex += 3;
        }
        tris[trisIndex] = vertIndexCenter;
        
        tris[trisIndex + 1] = vertIndexCenter + 1;
        tris[trisIndex + 2] = vertIndexCenter + k + 1;
        trisIndex += 3;
    }

    void MakeBoxLine(Vector3 p1, Vector3 p2, float width, ref List<Vector3> verts, ref int vertIndex, ref int[] tris, ref int trisIndex)
    {
        //Vector3 dir = (p2 - p1).normalized;
        Vector3 dir = p2 - p1;
        dir.y = 0f;
        dir.Normalize();
        Vector3 offset = GetVectorRotatedXZ(dir);

        Vector3 a = p1 + offset * width;
        Vector3 b = p1 - offset * width;

        Vector3 c = p2 + offset * width;
        Vector3 d = p2 - offset * width;
        verts[vertIndex++] = a;
        verts[vertIndex++] = b;
        verts[vertIndex++] = c;
        verts[vertIndex++] = d;

        tris[trisIndex] = vertIndex - 4;//a
        tris[trisIndex + 1] = vertIndex - 2;//c
        tris[trisIndex + 2] = vertIndex - 3;//b

        tris[trisIndex + 3] = vertIndex - 2;//c
        tris[trisIndex + 4] = vertIndex - 1;//d
        tris[trisIndex + 5] = vertIndex - 3;//b

        trisIndex += 6;
    }

    int GetCenterIndex(int circleNumber)
    {
        return (circleResolution + 1) * circleNumber;
    }

    List<Vector3> verts;
    int[] tris;
    int currentPointsNum = 0;

    void Init()
    {
        if(points == null)
        {
            Debug.LogError("Trying to init 0 points !!!");
        }


        int vertCount = (circleResolution + 1) * points.Count + (points.Count - 1) * 4;
        int trisCount = (circleResolution * points.Count + (points.Count - 1) * 2) * 3;
        verts = new List<Vector3>(vertCount);
        for(int i = 0; i< verts.Capacity; i++)
            verts.Add(Vector3.zero);
        tris = new int[trisCount];
        currentPointsNum = points.Count;
    }

    void ClearPointers()
    {
        if (lineObj.transform.childCount > 0)
        {
            foreach (Transform child in lineObj.transform)
            {
                GameObject child_to_delete = child.gameObject;

                if (Application.isEditor)
                {
                    if (Application.isPlaying)
                        Destroy(child_to_delete);
                    else
                        DestroyImmediate(child_to_delete);
                }
                else
                {
                    Destroy(child_to_delete);
                }
            }
            
            Debug.Log("<color=yellow>ClearPointers()</color>");
        }
    }

    public float distancePerPointer = 4f;

    public Mesh CreateMesh(float pathLength = 0f)
    {
        if(currentPointsNum != points.Count)
            Init();

        Mesh mesh = new Mesh();
        
        int vertIndex = 0;
        int trisIndex = 0;
        for(int i = 0; i < points.Count; i++)
        {
//            float r = (i == 0 || i == points.Length - 1) ? radius : radius * 0.8f;
            float r = (i == 0 || i == points.Count - 1) ? radius : lineWidth;
            MakeCircle(points[i], r, circleResolution, ref verts, ref vertIndex, ref tris, ref trisIndex);
        }

        ClearPointers();

        for(int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p1 = verts[GetCenterIndex(i)];
            Vector3 p2 = verts[GetCenterIndex(i + 1)];
            MakeBoxLine(p1, p2, lineWidth, ref verts, ref vertIndex, ref tris, ref trisIndex);
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        
        MakePointers(pathLength);

        return mesh;
    }
    
    List<Transform> pointers = new List<Transform>();
    
    void MakePointersBetweenPoints(Vector3 p1, Vector3 p2)
    {
        float distance = Vector3.Distance(p1, p2);
        int pointersNum = (int)(distance / distancePerPointer);
        
        Vector3 dir = (p2 - p1).normalized;
        
        Debug.Log("<color=yellow>MakePointersBetweenPoints()</color>");
        Vector3 pos;
        pos = (p1 + p2)/2;
        
        GameObject pointer = Instantiate(pointer_prefab, pos, Quaternion.identity, lineObj.transform);
        
        pointer.transform.forward = dir;
            
        // pointer.transform.forward = dir;
        
        // for(int i = 0; i < pointersNum; i++)
        // {
        //     //pos = p1 + dir * distancePerPointer * i + Vector3.up * 0.02f;
            
        //     // if(i != 0 && i != pointersNum-1)
        //     // {
        //     // }
        //     // else
        //     // {
        //     //     if(i == 0)
        //     //     {
        //     //         pos = p1 + dir * distancePerPointer * i + Vector3.up * 0.02f;
        //     //     }
        //     //     if(i == pointersNum -1)
        //     //     {
                    
        //     //     }
        //     // }
            
        //     GameObject pointer = Instantiate(pointer_prefab, pos, Quaternion.identity, this.transform);
            
        //     pointer.transform.forward = dir;
                
        //     pointers.Add(pointer.transform);
        // }
    }
    
    void MakePointers(float pathLength = 0f)
    {
        if(pathLength <= 0f)
        {
            Debug.Log("<color=red>PathLength is 0</color>");
            return;
        }
            
        pointers.Clear();
        
        for(int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p1 = verts[GetCenterIndex(i)];
            Vector3 p2 = verts[GetCenterIndex(i + 1)];
            
            MakePointersBetweenPoints(p1, p2);
        }
        
    }
    
    void CreateCube(Vector3 pos, float scale = 0.07f)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = Vector3.one * scale;
        cube.transform.position = pos;
    }
    
    
}

[System.Serializable]
public struct Circle3d
{
    public Vector3 center;
    public float radius;
    public Vector3[] vertices;
    public int[] triangles;
    void CreateCube(Vector3 pos)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = Vector3.one * 0.07f;
        cube.transform.position = pos;
    }
    public Circle3d(Vector3 center, float radius, int resolution, int startIndice)
    {
        this.center = center;
        this.radius = radius;

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        
        float angleStep = 2 * Mathf.PI / resolution;

        float angle = 0f;
        verts.Add(center);
        for(int i = startIndice; i < startIndice + resolution/1; i++)
        {
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            
            verts.Add(center + offset);

            // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // cube.transform.localScale = Vector3.one * 0.07f;
            // cube.transform.position = center + offset;

            angle += angleStep;
        }
        for(int i = startIndice; i < startIndice + verts.Count - 2; i++)
        {
            tris.Add(startIndice);
            
            tris.Add(i + 2);
            tris.Add(i + 1);
        }

        tris.Add(startIndice);
        
        tris.Add(startIndice + 1);
        tris.Add(startIndice + verts.Count - 1);

        triangles = tris.ToArray();
        vertices = verts.ToArray();
    }

    public Vector3 GetCenter()
    {
        return center;
    }

    public Vector3 GetLeftPoint()
    {
        return vertices[1];
    }

    public Vector3 GetRightPoint()
    {
        return vertices[vertices.Length - 1];
    }

    public int GetLeftIndice()
    {
        return triangles[1];
    }

    public int GetRightIndice()
    {
        return triangles[triangles.Length - 1];
    }

}