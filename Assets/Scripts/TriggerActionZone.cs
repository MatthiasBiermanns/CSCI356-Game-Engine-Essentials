using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerActionZone : MonoBehaviour
{
    public Challenge challenge;

    public Color zoneColor;

    // Start is called before the first frame update
    void Start()
    {
        zoneColor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frames
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovableCube"))
        {
            Debug.Log("TriggerActionZone: Cube entered the zone");
            if (other.GetComponent<Renderer>().material.color == zoneColor)
            {
                challenge.CompleteChallenge();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MovableCube"))
        {
            Debug.Log("TriggerActionZone: Cube exited the zone");
            // Perform action here
        }
    }
}
