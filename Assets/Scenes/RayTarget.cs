using UnityEngine;

public class RayTarget : MonoBehaviour {

    public Camera cam;
    public LayerMask layerMask;
    public Transform target;

    


    private void Update()
    {
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000f, layerMask))
        {
            target.position = hit.point;
        }
        
    }

}
