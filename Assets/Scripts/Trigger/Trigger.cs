using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    private bool isTriggered = false;
    public UnityEvent onTrigger;
    public UnityEvent onUntrigger;

    // empty = all tags
    public string[] triggeringTags = {};

    public bool freezeTriggeringObject = false;
    public Collider triggerObjectCollider;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == triggerObjectCollider) 
        {
            setIsTriggered(false);
        }
    }

    public bool getIsTriggered()
    {
        return isTriggered;
    }

    protected void setIsTriggered(bool value)
    {
        if (value == isTriggered) {
            return;
        }
        
        isTriggered = value;

        if(isTriggered)
        {
            onTrigger.Invoke();
            if (freezeTriggeringObject)
            {
                StartCoroutine(FreezeTriggerObject());
            }
        } else
        {
            triggerObjectCollider = null;
            onUntrigger.Invoke();
        }
    }

    private IEnumerator FreezeTriggerObject()
    {
        yield return new WaitForSeconds(0.2f);
        
        Freezable triggerCube = triggerObjectCollider.GetComponent<Freezable>();

        if (triggerCube != null)
        {
            triggerCube.Freeze();
        }
    }

    public GameObject GetCubeInZone()
    {
        if (triggerObjectCollider != null)
        {
            return triggerObjectCollider.gameObject;
        }
        return null;
    }
}
