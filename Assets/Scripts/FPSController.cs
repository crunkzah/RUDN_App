using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSController : MonoBehaviour {
    public Vector2 upDownRange = new Vector2(-70, 70);
    public float mouseSensX = 2f;
    public float mouseSensY = 2f;
    float verticalRotation;

    public float moveSpeed = 5f;
    public bool isControlling = false;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
            isControlling = !isControlling;
        if(isControlling)
        {
            Movement();
            HandleRotation();
        }
    }

    float GetAxisY()
    {
        if(Input.GetKey(KeyCode.Space))
            return 1f;
        if(Input.GetKey(KeyCode.LeftShift))
            return -1f;

        return 0f;
    }

    void Movement()
    {
        Vector2 inputXZ = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(inputXZ != Vector2.zero)
            inputXZ.Normalize();
        Vector3 input = new Vector3(inputXZ.x, GetAxisY(), inputXZ.y);
        

        Vector3 velocity = input * moveSpeed;
        transform.Translate(velocity * Time.deltaTime, Space.Self);
    }

    void HandleRotation()
    {
        float horizontalRotation = Input.GetAxisRaw("Mouse X") * mouseSensX;
        verticalRotation -= Input.GetAxisRaw("Mouse Y") * mouseSensY;
        verticalRotation = Mathf.Clamp(verticalRotation, upDownRange.x, upDownRange.y);
        float desiredHorizontal = transform.rotation.eulerAngles.y + horizontalRotation;

        transform.rotation = Quaternion.Euler(verticalRotation, desiredHorizontal, 0f);
    }
}
