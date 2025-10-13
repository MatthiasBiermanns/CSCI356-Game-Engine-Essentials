using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMovement : MonoBehaviour
{
    [SerializeField] private float bobAmplitude = 0.15f;
    [SerializeField] private float bobFrequency = 0.8f;
    [SerializeField] private float rotationSpeed = 35f;

    private Vector3 startPos;
    private float phaseOffset;

    void Start()
    {
        startPos = transform.position;
        phaseOffset = Random.value * Mathf.PI * 2f; // randomPhase = true
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin((Time.time + phaseOffset) * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
