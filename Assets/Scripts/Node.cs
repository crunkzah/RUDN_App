using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public Vector3 worldPos
    {
        get
        {
            return transform.position;
        }
    }

    public Vector3 labelPos
    {
        get
        {
            if(label == null)
                return transform.position;
            return label.transform.position;
        }
    }

    public List<Node> neighbours;

    static List<Node> parentedNodes = new List<Node>();
    public static void ClearAllParents()
    {
        if(parentedNodes == null)
        {
            Debug.LogError("ParentedNodes is null !!!");
            return;
        }

        int k = 0;
        for(int i = 0; i < parentedNodes.Count; i++)
        {
            parentedNodes[i].parent = null;
            k++;
        }
        parentedNodes.Clear();
        //Debug.Log("Cleared " + k + " nodes!");
    }

    public Node parent;
    public void SetParent(Node parent)
    {
        parentedNodes.Add(this);
        this.parent = parent;
    }

    public GameObject label;

    [HideInInspector] public float gCost = -1;
    [HideInInspector] public float hCost = -1;

    public float fCost
    {
        get{
            return gCost + hCost;
        }
    }

    public static bool drawGizmos = true;
    public bool isRoom = true;
    public bool centerLabelWhenPlaced = true;

    public bool isWC = false;

    public string nodeKey = "stringKey";

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(drawGizmos == false)
            return;
        // foreach(Node node in neighbours)
        // { 
        if(neighbours != null)
        {
            for(int i = 0; i < neighbours.Count; i++)
            {
                if(neighbours[i] != null) 
                Debug.DrawLine(this.worldPos, neighbours[i].worldPos, Color.yellow, 0, true);
            }
        }
        if(isRoom)
            UnityEditor.Handles.Label(transform.position, this.nodeKey);
    }
    #endif


}
