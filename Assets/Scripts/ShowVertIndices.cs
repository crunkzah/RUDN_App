
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShowVertIndices : MonoBehaviour
{

    void OnEnable()
    {
        ReadMesh();
    }

    public void ReadMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        vertPositions = mesh.vertices;
        tris = mesh.GetTriangles(0);
    }

    public int[] tris;

    Vector3[] vertPositions;
    public bool showVertPos = false;
    [Range(8, 16)]
    public int fontSize = 10;
    public void OnDrawGizmos()
    {
        if(this.enabled)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.cyan;
            style.fontSize = fontSize;
            if(vertPositions != null)
            {
                for(int i = 0; i < vertPositions.Length; i++)
                {
                    if(showVertPos)
                        UnityEditor.Handles.Label(transform.position + vertPositions[i], i.ToString() + '\n' + vertPositions[i].ToString(), style);
                    else
                        UnityEditor.Handles.Label(transform.position + vertPositions[i], i.ToString(), style);
                }    
            }
        }
    }
}
#endif