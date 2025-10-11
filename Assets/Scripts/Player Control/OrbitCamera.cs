using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    // the orbit target (e.g., the player GameObject)
    [SerializeField] Transform target;

    // rotation sensitivity
    public float rotSpeed = 1.5f;

    private float rotY;     // horizontal rotation
    private float rotX;     // horizontal rotation
    private Vector3 offset; // offset from the target

    // Start is called before the first frame update
    void Start()
    {
        // get transform component's yaw
        rotY = transform.eulerAngles.y;
        rotY = transform.eulerAngles.x;

        // compute offset of camera from the target
        offset = target.position - transform.position;
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            // yaw based on horizontal mouse movement
            rotY += Input.GetAxis("Mouse X") * rotSpeed * 3;
            rotX += Input.GetAxis("Mouse Y") * 5;

            // convert from Euler angles to quaternions
            Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);

            // set the camera's position based on the offset
            transform.position = target.position - (rotation * offset);

            // camera looking at the target
            transform.LookAt(target);
        }
    }

}
