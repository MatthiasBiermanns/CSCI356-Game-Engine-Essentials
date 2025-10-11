using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpOnCubeTrigger : MonoBehaviour
{
    public Challenge challenge;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("JumpOnCubeTrigger: Player jumped on the cube");
            challenge.CompleteChallenge();
        }
    }
}
