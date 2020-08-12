
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[System.Serializable]
public struct RoomKey_Info
{
	public string name;
	public string key_node;
	public TranslatableString string_translatable;
}

public class SetupLabels : MonoBehaviour 
{

	[Header("Additional infos:")]
	public List<RoomKey_Info> additionalInfoNodes;
	// public List<string> additionalInfoNodes;
	// public List<TranslatableString> strings_translatables;

	void Start()
	{
		Setup();
	}


	public void Setup()
	{
		foreach(RoomKey_Info roomKeyInfo in additionalInfoNodes)
		{
			string node_key = roomKeyInfo.key_node;
            Node node = PathFinder.singleton.GetNodeFromPool(node_key);
			if(node != null)
			{
                AdditionalRoomInfo room_info = node.GetComponent<AdditionalRoomInfo>();
                if (room_info == null)
                {
                    room_info = node.gameObject.AddComponent<AdditionalRoomInfo>();
                }

				room_info.t_string = roomKeyInfo.string_translatable;

				Debug.Log("Added info to <color=yellow>" + roomKeyInfo.key_node + "</color> room");
			}
            else
            {
                Debug.LogError("Node " + node_key + " not found");
            }
		}

		

		Debug.Log("<color=yellow>Labels setup !</color>");

		if(!Application.isPlaying)
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
	}
}
#endif