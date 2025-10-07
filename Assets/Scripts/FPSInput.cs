using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")] 
public class FPSInput : MonoBehaviour
{
    private CharacterController charController;
    private float yVelocity = 0f;

    public float speed = 6.0f;
    public float gravity = -9.8f;
    public float jumpPower = 5f;
    private bool doubleJumpUsed = false;

    public float pushForce = 2.0f;

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // changes based on WASD keys
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);

        // make diagonal movement consistent
        movement = Vector3.ClampMagnitude(movement, speed);

        if (charController.isGrounded || doubleJumpUsed == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(!charController.isGrounded)
                {
                    doubleJumpUsed = true;
                }
                yVelocity = jumpPower;
            } else
            {
                doubleJumpUsed = false;
            }
        }

        // ensure gravitational impact
        yVelocity += gravity * Time.deltaTime;
        movement.y = yVelocity;

        // ensure movement is independent of the framerate
        movement *= Time.deltaTime;

        // transform from local space to global space
        movement = transform.TransformDirection(movement);


        // pass the movement to the character controller
        charController.Move(movement);

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // check if a non-kinematic rigidbody was hit
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body != null && !body.isKinematic)
        {
            // push the rigidbody in the move direction
            body.velocity = hit.moveDirection * pushForce;
        }
    }
}
