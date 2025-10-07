using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActionZone : MonoBehaviour
{
    public Color zoneColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = zoneColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovableCube"))
        {
            Debug.Log("TriggerActionZone: Cube entered the zone");
            // Perform action here
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
