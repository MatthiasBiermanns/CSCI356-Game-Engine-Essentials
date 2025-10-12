using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Freezable : MonoBehaviour
{
    public bool initiallyFreezed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (initiallyFreezed)
        {
            Freeze();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Freeze()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeAll;

        // freeze color change ability
        ChangeColor cc = GetComponent<ChangeColor>();
        if(cc != null )
        {
            cc.isFreezed = true;
        }
    }

    public void Unfreeze()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.None;

        // unfreeze color change
        ChangeColor cc = GetComponent<ChangeColor>();
        if (cc != null)
        {
            cc.isFreezed = false;
        }
    }
}
