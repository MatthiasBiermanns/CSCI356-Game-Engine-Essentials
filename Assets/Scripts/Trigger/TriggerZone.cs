using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerZone : Trigger
{
    public Color zoneColor;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = Enums.ColorToUnityColor(zoneColor);
    }

    // Update is called once per frames
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // no checks necessary, if already triggered
        if (base.getIsTriggered())
        {
            return;
        }

        // check if restricted tags
        if (base.triggeringTags.Length != 0)
        {
            // if no tag matches, return
            if (!base.triggeringTags.Any((string tag) => other.CompareTag(tag)))
            {
                return; 
            }
        }
        
        // check further requirements
        if (other.CompareTag("MovableCube"))
        {
            // check for same color
            if (other.GetComponent<Renderer>().material.color != Enums.ColorToUnityColor(zoneColor))
            {
                return;
            }
        }

        // set triggered, if no special case identified
        base.triggerObjectCollider = other;
        base.setIsTriggered(true);
    }

    public void NotifiedColorChange(GameObject gObj) 
    {
        // has no impact, if not a movable cube
        if (!gObj.CompareTag("MovableCube"))
        {
            return;
        }

        if (base.getIsTriggered())
        {
            // check if same collider, that triggers at the moment
            if (gObj.GetComponent<Collider>() != base.triggerObjectCollider)
            {
                // check for same color
                if (gObj.GetComponent<Renderer>().material.color != Enums.ColorToUnityColor(zoneColor))
                {
                    base.setIsTriggered(false);
                }
            }
        } else
        {
            // check for same color
            if (gObj.GetComponent<Renderer>().material.color == Enums.ColorToUnityColor(zoneColor))
            {
                base.triggerObjectCollider = gObj.GetComponent<Collider>();
                base.setIsTriggered(true);
            }
        }
    }
}
