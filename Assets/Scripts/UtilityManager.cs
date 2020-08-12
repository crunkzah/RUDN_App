
using UnityEngine;

public class UtilityManager : MonoBehaviour {

	void Start()
	{
	#if UNITY_EDITOR || UNITY_STANDALONE
		 Application.targetFrameRate = 120;
	#endif
	#if UNITY_ANDROID || UNITY_IOS
		Application.targetFrameRate = 60;
	#endif
	}
}
