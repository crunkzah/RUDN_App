using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using TMPro;
using System.Linq;

[System.Serializable]
public struct AlternateKey
{
    public string originKey;
    public List<string> alternateKeys;
}

public class PathFinder : MonoBehaviour {


    #region Singleton
    static PathFinder _instance;
    public static PathFinder singleton{
        get{
            if(_instance == null)
                _instance = FindObjectOfType<PathFinder>();
            return _instance;
        }
    }
    #endregion
    


    [Header("Nodes:")]
    public GameObject nodePrefab;
    public List<Node> nodes;
    public Node startNode, endNode;
    
    [Header("Path:")]
    public List<Vector3> path;

    [Header("UI Elements bindings:")]
    public TMP_InputField from_InputField;
    public TMP_InputField destination_InputField;
    [Header("Labels:")]
    public GameObject Label3dPrefab;

    [Header("Alternate keys:")]
    public List<AlternateKey> altKeys;

    public void Awake()
    {
        Node[] nodesArray = FindObjectsOfType<Node>();
        nodes = new List<Node>(nodesArray);
        if(vlr == null)
            Debug.LogError("VolumetricLineRenderer not set !!!");
            
        Debug.Log("Found " + nodes.Count + " nodes");
        startNode = endNode = null;
        InitNodesDictionary();
    }
    
    public VolumetricLineRenderer vlr;

    public void SetStartNode(Node node)
    {
        if(node == null)
            return;

        Debug.Log("SetStartNode");
        this.startNode = node;
        //from_InputField.text = node.nodeKey;
        
        string node_text = node.label.GetComponent<TextMeshPro>().text;
        if(node_text.Equals("WC"))
            node_text = "WC " + node.nodeKey;


        from_InputField.text = node_text;

        RectangleCreator.singleton.DrawRect1(node.worldPos);
    }

    public void SetEndNode(Node node)
    {
        if(node == null)
            return;

        Debug.Log("SetEndNode");
        this.endNode = node;

        string node_text = node.label.GetComponent<TextMeshPro>().text;

        if (node_text.Equals("WC"))
            node_text = "WC " + node.nodeKey;


        destination_InputField.text = node_text;
        RectangleCreator.singleton.DrawRect2(node.worldPos);
    }

    public string GetOriginKey(string key)
    {
        string result = key;
        
        key = key.ToLower();
        //key.Replace(" ", "");
        key = key.Replace(" ", string.Empty);

        #if UNITY_EDITOR
        foreach(char c in key)
            if(c == ' ')
                Debug.Log("SPACE!!!");
        #endif

        key = key.Replace("\n", string.Empty);
        
        //foreach(AlternateKey altKey in altKeys)
        for(int i = 0; i < altKeys.Count; i++)
        {
            if(altKeys[i].alternateKeys.Contains(key))
            {
                result = altKeys[i].originKey;
                Debug.Log("ALTERNATE: " + key + " ORIGIN: " + result);
                break;
            }
        }
        return result;
    }

    public void FindPathFromInputFields()
    {
        if(nodesDict == null)
        {
            Debug.LogError("nodeDict is null !!!");
            return;
        }

        Node old_start_node = startNode;
        Node old_end_node = endNode;
        
        
        string nodeKey = from_InputField.text;


        nodeKey = GetOriginKey(nodeKey);

        nodeKey = nodeKey.Replace("WC ", string.Empty);

        if(nodesDict.ContainsKey(nodeKey))
            startNode = nodesDict[nodeKey];
        else
            startNode = null;

        
        nodeKey = destination_InputField.text;

        nodeKey = nodeKey.Replace("WC ", string.Empty);

        nodeKey = GetOriginKey(nodeKey);
        if(nodesDict.ContainsKey(nodeKey))
            endNode = nodesDict[nodeKey];
        else
            endNode = null;

        if(startNode != null && endNode != null)
        {
           
            FindPath(startNode, endNode, out path);
           
            vlr.points = path;
            
            vlr.RedrawLines(current_length);
            RectangleCreator.singleton.DrawRects(startNode.worldPos, endNode.worldPos);
            
        }
        else
        {
            FloorRouteLabel.SetFloorRouteText(0, 0);
            vlr.Hide();
            RectangleCreator.singleton.Hide();
        }

        SelectionManager2.singleton.CheckNode_visibility(old_start_node);
        SelectionManager2.singleton.CheckNode_visibility(old_end_node);


        SelectionManager2.singleton.CheckNode_visibility(startNode);
        SelectionManager2.singleton.CheckNode_visibility(endNode);

        SelectionManager2.singleton.SetFloor(SelectionManager2.singleton.currentFloor);

    }

    [Header("Floors for labels:")]
    public Transform[] labelsHoldersByFloors;
    public float[] floorHeights;

    public void SetNodesToFloors()
    {
        if(labelsHoldersByFloors == null)
        {
            Debug.LogWarning("LabelsHoldersByFloor is null");
            return;
        }

        if(floorHeights == null)
        {
            Debug.LogWarning("floorHeights is null");
            return;
        }

        if(labelsHoldersByFloors.Length != floorHeights.Length)
        {
            Debug.LogError("Lengths of floorHeights and labelsHoldersByFloors are not matched!!!");
            return;
        }

        nodes = new List<Node>(FindObjectsOfType<Node>());
        for(int i = 0; i < nodes.Count; i++)
        {
            if(nodes[i].isRoom == false)
                continue;
                
            int floorIndex = GetNodeFloor(nodes[i]);
            nodes[i].label.transform.SetParent(labelsHoldersByFloors[floorIndex]);
        }
    }

    int GetNodeFloor(Node node)
    {
        if(node.isRoom == false)
            return -1;
        if(node.label == null)
            Debug.LogWarning("Node " + node.nodeKey + " has no label");
            
        float minHeightDiff = float.MaxValue;
        int floorIndex = 0;
        for(int j = 0; j < floorHeights.Length; j++)
        {
            float heightDiff = Mathf.Abs(node.worldPos.y - floorHeights[j]);
            if(heightDiff < minHeightDiff)
            {
                minHeightDiff = heightDiff;
                floorIndex = j;
            }
        }

        return floorIndex;
    }

    int GetFloorFromNode(Node node)
    {
        int Result = node.nodeKey[0] - '0';
        return Result;
    }

    public void FindPath(Node startNode, Node endNode, out List<Vector3> path)
    {
        Node.ClearAllParents();
        FindNodePath(startNode, endNode);
        path = RetracePath(endNode);
        if(path.Count > 1)
        {
            
            int floorStart = GetFloorFromNode(startNode);
            // Debug.Log(string.Format("<color=green>FloorStart: {0}-{1}</color>", startNode.nodeKey, floorStart));
            int floorEnd = GetFloorFromNode(endNode);
            // Debug.Log(string.Format("<color=blue>FloorStart: {0}-{1}</color>", endNode.nodeKey, floorEnd));
            FloorRouteLabel.SetFloorRouteText(floorStart, floorEnd);
            CameraController2.singleton.SetMiddlePoint(path[0], path[path.Count - 1]);
        }
        else
        {
            FloorRouteLabel.SetFloorRouteText(0, 0);
        }
    }

    public float current_length;

    public List<Vector3> RetracePath(Node node)
    {
        List<Vector3> path = new List<Vector3>();
       // float distance = 0f;
        int iterations = 0;

        while(node != null && iterations < 2000)
        {
            iterations++;
            path.Add(node.worldPos);
            
            node = node.parent;
     
        }

        current_length = 0f;
        for(int i = 0; i < path.Count-1; i++)
        {
            current_length += Vector3.Distance(path[i], path[i+1]);
        }

        Debug.Log("<color=yellow>Path length</color> is " + current_length.ToString());



        if(iterations >= 4999)
            Debug.LogWarning("RetracePath overshoot iterations limit ! Iterations: " + iterations.ToString());
        
        return path;
    }



    public void FindNodePath(Node start, Node end)
    {
        Debug.Log("Finding path");
        if (start == null || end == null)
            Debug.LogWarning("Couldn't find path with no nodes selected");
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        start.gCost = 0f;
        start.hCost = GetDistance(start, end);
        open.Add(start);
        int iterations = 0;

        while(open.Count > 0 && iterations < 4000)
        {
            Node current = GetNodeFromOpen(open);
            open.Remove(current);
            closed.Add(current);

            if (current == end)
            {
                return;
            }

            foreach(Node neighbour in current.neighbours)
            {
                if (closed.Contains(neighbour))
                    continue;

                float newCost = current.gCost + GetDistance(current, neighbour);

                if(newCost < neighbour.gCost || open.Contains(neighbour) == false)
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, end);
                    //neighbour.parent = current;
                    neighbour.SetParent(current);
                    if (open.Contains(neighbour) == false)
                        open.Add(neighbour);
                }
                
                
            }
        }
        if (iterations < 4000)
            Debug.Log("Path not found");
        else
            Debug.Log("Iterations overload");
    }

    Node GetNodeFromOpen(List<Node> q)
    {
        float cost = Mathf.Infinity;
        Node resultNode = null;

        foreach(Node node in q)
        {
            if(node.fCost < cost)
            {
                cost = node.fCost;
                resultNode = node;
            }
        }

        return resultNode;
    }
    
    float GetDistance(Node n1, Node n2)
    {
        return Vector3.Distance(n1.transform.position, n2.transform.position);
    }

    [Header("Graph reading settings:")]
    public string GraphDirectory = "directory";
    public string GeneratedConfigDirectory = "generated_directory";
    public Dictionary<string, Node> nodesDict;

    
    public bool centerLabels = false;
    public float aboveGroundOffset = 0.5f;
    [Header("Labels settings:")]
    public GameObject labelHolderToParent;

    public void SetNodeKeysFromNames()
    {
        
        Node[] nodes = FindObjectsOfType<Node>();
        List<Node> sortedNodes = new List<Node>(nodes);
        sortedNodes.OrderBy(n => n.name);
     

        int labelsCount = 0;
        foreach(Node node in sortedNodes)
        {
            if(node.label != null)
                return;
            string[] words = node.name.Split('_');
            
            node.nodeKey = words[1];
            node.isRoom = ((
                words[1][0] == '0') || (words[1][0] == '1')
            || (words[1][0] == '2') || (words[1][0] == '3') 
            || (words[1][0] == '4') || (words[1][0] == '5') || (words[1][0] == '6')
            || (words[1][0] == '7') || (words[1][0] == '8'));

           
            if(Label3dPrefab != null && node.isRoom)
            {
                GameObject label3d = Instantiate(Label3dPrefab, Vector3.zero, Quaternion.Euler(30f, 0f, 0f));
                labelsCount++;
                label3d.name = node.nodeKey.ToString() + " Label3D";

                node.label = label3d;
                if(!centerLabels || !node.centerLabelWhenPlaced)
                    label3d.transform.position = node.transform.position + Vector3.up * aboveGroundOffset;
                else
                    label3d.transform.position = FindObjectOfType<RectangleCreator>().CalculateCenterOfRectangle(node.transform.position, aboveGroundOffset);
                   
                
                if(labelHolderToParent != null)
                    label3d.transform.SetParent(labelHolderToParent.transform);

                if(node.isRoom)
                {
                    label3d.AddComponent<Label_node>();
                    label3d.GetComponent<Label_node>().node = node;
                }

                if(node.isWC)
                    label3d.GetComponent<TextMeshPro>().text = "WC";
                else
                    label3d.GetComponent<TextMeshPro>().text = node.nodeKey;
                
            }
            
        

        }
        #if UNITY_EDITOR
        Debug.Log("Instaintaited " + labelsCount.ToString() + " labels");
        #endif
    }
    [Header("Node parent !!!")]
    public Transform NodeParent;

#if UNITY_EDITOR
    public void AssignGeneratedGraph()
    {
        nodesDict = new Dictionary<string, Node>();


        FileStream fs = new FileStream(GeneratedConfigDirectory, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
       
        
        int nodeCount = int.Parse(sr.ReadLine());
        string line;
        for(int i = 0; i < nodeCount; i++)
        {
            line = sr.ReadLine();
            string[] words = line.Split(' ');
            if(words != null && words.Length > 0)
            {
                string currentNodeKey = words[0];
                Vector3 pos = new Vector3(float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]));
                GameObject node_new = Instantiate(nodePrefab, pos, Quaternion.identity);
                if(NodeParent != null)
                    node_new.transform.SetParent(NodeParent);
                node_new.transform.name = "Point_" + currentNodeKey;
                node_new.GetComponent<Node>().nodeKey = currentNodeKey;
                
                node_new.GetComponent<Node>().isRoom = bool.Parse(words[4]);
                node_new.GetComponent<Node>().isWC = words[0][0].Equals('0');

                if(node_new.GetComponent<Node>().isWC)
                {
                    print(node_new.name + " is WC.");
                }

                if(nodesDict.ContainsKey(currentNodeKey))
                    Debug.LogWarning("Key collision for nodesDict! Key is " + currentNodeKey);
                else
                    nodesDict.Add(currentNodeKey, node_new.GetComponent<Node>());
            }
        }
        while((line = sr.ReadLine()) != null)
        {
            string[] words = line.Split(' ');
            if(words != null && words.Length > 0)
            {
                string currentNodeKey = words[0];
                if(nodesDict.ContainsKey(currentNodeKey) == false)
                    Debug.LogError("Key " + currentNodeKey + " not found!!!");
                Node currentNode = nodesDict[currentNodeKey];
                
                for(int i = 1; i < words.Length; i++)
                {
                    if(nodesDict.ContainsKey(words[i]) == false)
                        Debug.LogError("Point " + currentNode + " has problem with key " + words[i]);
                    Node neighbour = nodesDict[words[i]];
                    AddRelation(currentNode, neighbour);
                }
            }
            else
            {
                Debug.LogError("Can't split the line into words, line is:");
                Debug.LogError(line);
            }
        }

        sr.Close();
        fs.Close();
        Debug.Log("----- POINTS CREATED -----");
    }

    

    public void GenerateGraphConfig()
    {
        if(UnityEditor.EditorApplication.isPlaying == false)
        {
            Debug.LogWarning("Can't generate config not in play mode!!! Enter play mode and try again");
            return;
        }
        if(!File.Exists(GeneratedConfigDirectory))
        {
            Debug.LogError("GeneratedConfigDirectory " + GeneratedConfigDirectory + " does not exist");
            return;
        }
        


        File.WriteAllText(GeneratedConfigDirectory, string.Empty);

        FileStream fs = new FileStream(GeneratedConfigDirectory, FileMode.Open, FileAccess.Write); 
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

        Node[] _nodes = FindObjectsOfType<Node>();
        sw.WriteLine(_nodes.Length);
        
        foreach(Node node in _nodes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(node.nodeKey);
            sb.Append(" ");
            sb.Append(node.worldPos.x);
            sb.Append(" ");
            sb.Append(node.worldPos.y);
            sb.Append(" ");
            sb.Append(node.worldPos.z);

            sb.Append(" ");
            sb.Append(node.isRoom.ToString());
            
            sw.WriteLine(sb.ToString());
        }

        foreach(Node node in _nodes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(node.nodeKey);
            if(node.nodeKey.Contains(" "))
                Debug.LogError("Node " + node.name + " has problems!!!");
            if(node.nodeKey == " " || node.nodeKey == string.Empty)
                Debug.LogError("Node " + node.name + " has no key");

            foreach(Node neighbour in node.neighbours)
            {
                sb.Append(" ");
                if(neighbour == null)
                    Debug.LogWarning(node.name + " is corrupted");
                    
                
                
                if(neighbour.nodeKey == " " || neighbour.nodeKey == string.Empty)
                    Debug.LogError("Node " + node.name + " has neighbour with no key. Neighbour is " + neighbour.name);
                sb.Append(neighbour.nodeKey);
                
            }

            sw.WriteLine(sb.ToString());
        }
        sw.Close();
        fs.Close();
        Debug.Log("----- CONFIG GENERATED -----");
    }
#endif
    public void InitNodesDictionary()
    {
        Debug.Log("InitNodesDictionary...");
        nodesDict = new Dictionary<string, Node>();
        for(int i = 0; i < nodes.Count; i++)
        {
            if(nodesDict.ContainsKey(nodes[i].nodeKey))
                Debug.LogWarning("NodesDict already contains key " + nodes[i].nodeKey);
            nodesDict.Add(nodes[i].nodeKey, nodes[i]);
        }
    }

    public void AddRelation(Node node1, Node node2)
    {
        if(node1 == node2)
        {
            Debug.LogWarning("You are trying to connect node with itself");
            return;
        }

        if(node1.neighbours != null && node2.neighbours != null)
        {
            if(!node1.neighbours.Contains(node2))
                node1.neighbours.Add(node2);
            if(!node2.neighbours.Contains(node1))
                node2.neighbours.Add(node1);

            #if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPlaying)
                Debug.Log(node1.name + " and " + node2.name + " are now neighbours !!!");
            #endif
        }


        if(node1.neighbours == null)
            Debug.LogError("Node neighbours are null !!! Node is " + node1.name);
        
        if(node2.neighbours == null)
            Debug.LogError("Node neighbours are null !!! Node is " + node2.name);


    }

    public void DeleteRelation(Node node1, Node node2)
    {
        if(node1 == node2)
            return;

        if(node1.neighbours != null && node1.neighbours.Contains(node2))
            node1.neighbours.Remove(node2);
        else
            return;

        if(node2.neighbours != null && node2.neighbours.Contains(node1))
            node2.neighbours.Remove(node1);
        else
            return;
        
        Debug.Log(node1.name + " and " + node2.name + " are no longer neighbours.");
    }

    public Node GetNodeFromPool(string key)
    {
        if(key[0] == 'б')
            key = "1" + key;
        key = GetOriginKey(key);
        if(nodesDict.ContainsKey(key) == false){
            Debug.LogError("NodesDisct does not containt key " + key);
            return null;
        }
        
        return nodesDict[key];

    }

}
