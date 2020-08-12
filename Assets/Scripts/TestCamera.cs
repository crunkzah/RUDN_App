using UnityEngine;

public class TestCamera : MonoBehaviour {

	public Camera cam;
	void Awake()
	{
		cam = FindObjectOfType<Camera>();
	}

	void Start()
	{
        Vector3[] frustumCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        for (int i = 0; i < 4; i++)
        {
            var worldSpaceCorner = cam.transform.TransformVector(frustumCorners[i]);
			
			
            Debug.DrawRay(cam.transform.position, worldSpaceCorner, Color.blue, 10f);
        }

       
	}
}
