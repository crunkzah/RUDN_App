
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


[CustomEditor(typeof(PathFinder))]
public class PathFinderEditor : Editor {


    public override void OnInspectorGUI()
    {

        //int count = 0;
        base.OnInspectorGUI();
        PathFinder pf = (PathFinder) target;
        // if(GUILayout.Button("Assign graph"))
        // {
        //     pf.AssignGraph();
        // }
        if(GUILayout.Button("Assign Node keys"))
        {
            pf.SetNodeKeysFromNames();
        }
        
        if(GUILayout.Button("Generate graph config"))
        {
            pf.GenerateGraphConfig();
        }



        if(GUILayout.Button("Assign graph from generated config"))
        {
            pf.AssignGeneratedGraph();
        }
        if(GUILayout.Button("Set labels floors"))
        {
            pf.SetNodesToFloors();
        }

        if(GUILayout.Button("Remove renderers from nodes"))
        {
            Node[] nodes = FindObjectsOfType<Node>();
            foreach (Node node in nodes)
            {

                Renderer renderer = node.GetComponent<Renderer>();
                if(renderer != null)
                    DestroyImmediate(renderer);
                MeshFilter mf = node.GetComponent<MeshFilter>();
                if(mf != null)
                    DestroyImmediate(mf);
            }

            Debug.Log("Removed renderers and meshfilters from nodes.");
        }
        if (GUILayout.Button("Add renderers to nodes"))
        {
            Node[] nodes = FindObjectsOfType<Node>();
            foreach (Node node in nodes)
            {

                Renderer renderer = node.GetComponent<Renderer>();
                if(renderer == null)
                {
                    node.gameObject.AddComponent<MeshRenderer>();
                }
            }
        }

        if(GUILayout.Button("Toggle Node Renderers"))
        {
            Node[] nodes = FindObjectsOfType<Node>();
            foreach(Node node in nodes)
            {
                
                Renderer renderer = node.GetComponent<Renderer>();
                renderer.enabled = !renderer.enabled;
            }
        }

        


        if(GUILayout.Button("Remove colliders from nodes"))
        {
            if(EditorApplication.isPlaying)
            {
                Debug.LogWarning("Can't remove colliders from nodes in Play mode");
                return;
            }

            Node[] nodes = FindObjectsOfType<Node>();
            for(int i = 0; i < nodes.Length; i++)
            {
                Collider col = nodes[i].GetComponent<Collider>();
                if(col != null)
                    DestroyImmediate(col);
            }
        }

        if(GUILayout.Button("Add colliders to nodes"))
        {
            if(EditorApplication.isPlaying)
            {
                Debug.LogWarning("Can't add colliders from nodes in Play mode");
                return;
            }

            Node[] nodes = FindObjectsOfType<Node>();
            for(int i = 0; i < nodes.Length; i++)
            {
                Collider col = nodes[i].GetComponent<Collider>();
                if(col == null)
                    nodes[i].gameObject.AddComponent<SphereCollider>();
            }
        }
        if(GUILayout.Button("Calculate floor heights"))
        {
            for(int i = 0; i < 9; i++)
            {
                pf.floorHeights[i] = 3.357649f + i * 4.3414f;
            }
        }
        if(GUILayout.Button("Delete all labels"))
        {
            if(EditorApplication.isPlaying)
            {
                Debug.LogWarning("Deleting all labels allowed only in editor");
                return;
            }
            
            GameObject[] labels = GameObject.FindGameObjectsWithTag("Label3d");
            for(int i = 0; i < labels.Length; i++)
                DestroyImmediate(labels[i]);
        }
        if(GUILayout.Button("Render node debug labels"))
        {
            Node.drawGizmos = !Node.drawGizmos;
        }
        // if(GUILayout.Button("Fix neighbours"))
        // {
        //     //pf.nodes = FindObjectsOfType<Node>();
        //     Node[] nodesArray = FindObjectsOfType<Node>();
        //     pf.nodes = new List<Node>(nodesArray);
        //     List<Node> nodes = pf.nodes;

        //     for (int i = 0; i < nodes.Count; i++)
        //     {
        //         for(int j = 0; j < nodes.Count; j++)
        //         {
        //             if (i == j)
        //                 continue;
        //             bool a = ContainsNode(ref nodes[i].neighbours, ref nodes[j]);
        //             bool b = ContainsNode(ref nodes[j].neighbours, ref nodes[i]);
        //             if (((a || b) == false) || (a & b))
        //                 continue;
        //             else
        //             {
        //                 if(a && !b)
        //                 {
        //                     count++;
        //                     //AddToArray(ref nodes[j].neighbours, nodes[i]);
        //                     nodes[j].neighbours.Add(nodes[i]);
        //                 }
        //                 else
        //                 {
        //                     count++;
        //                     nodes[i].neighbours
        //                     //AddToArray(ref nodes[i].neighbours, ref nodes[j]);
        //                 }
        //             }
                    

        //         }
        //     }
        //     Debug.Log("Fixed " + count + " times");
        // }

        

    }



    
    
    

    
    // void AddToList(List<Node> nodes, ref Node newNode)
    // {
    //     nodes.Add(newNode);
    // }

    // bool ContainsNode(List<Node> nodes, Node target)
    // {

    //     return nodes.Contains(target);
    // }

}
#endif