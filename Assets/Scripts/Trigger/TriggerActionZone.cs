using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerActionZone : MonoBehaviour
{
    public Challenge challenge;

    public UnityEngine.Color zoneColor;

    public bool freezeTriggerItem = false;

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
        TriggerCube triggerCube = other.GetComponent<TriggerCube>();

        if (triggerCube == null) return;

        if(freezeTriggerItem)
        {
            StartCoroutine(FreezeTriggerItem(triggerCube));
        }

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

    IEnumerator FreezeTriggerItem(TriggerCube item)
    {
        yield return new WaitForSeconds(0.2f);
        item.Freeze();
    }
}
