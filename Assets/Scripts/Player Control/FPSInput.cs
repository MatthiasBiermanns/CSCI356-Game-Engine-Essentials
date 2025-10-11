using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
[DefaultExecutionOrder(1200)] // after grappling hook
public class FPSInput : MonoBehaviour
{
    private CharacterController charController;
    private float yVelocity = 0f;

    public float speed = 6.0f;
    public float gravity = -9.8f;
    public float jumpPower = 5f;
    private bool doubleJumpUsed = false;

    public float pushForce = 2.0f;

    // input + gravity
    private Vector3 baseVelocity = Vector3.zero;
    // for external scripts (grappling hook)
    public Vector3 externalVelocity = Vector3.zero;
    [HideInInspector] public bool suspendGravity = false;

    private Vector3 standUpScale;
    private Vector3 sneakingScale;

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

        if (charController.isGrounded && yVelocity < 0f)
            yVelocity = -2f;

        // ensure gravitational impact
        if (!suspendGravity)
            yVelocity += gravity * Time.deltaTime;
        movement.y = yVelocity;

        // transform from local space to global space
        movement = transform.TransformDirection(movement);

        baseVelocity = movement;
    }

    void LateUpdate()
    {
        // Summe aus Basis + externer (Grapple) Velocity
        Vector3 movement = baseVelocity + externalVelocity;

        // ensure movement is independent of the framerate
        movement *= Time.deltaTime;
        charController.Move(movement);

        // externen Anteil nach Verbrauch zurücksetzen
        externalVelocity = Vector3.zero;
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
