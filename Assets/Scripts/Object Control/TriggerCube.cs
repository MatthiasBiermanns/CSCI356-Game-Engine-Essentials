using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class TriggerCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Freeze()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeAll;

        ChangeColor cc = GetComponent<ChangeColor>();

        if(cc != null )
        {
            cc.isFreezed = true;
        }
    }
}
