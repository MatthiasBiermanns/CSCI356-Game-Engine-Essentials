using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Challenge : MonoBehaviour
{
    public string challengeName;
    public UnityEvent onChallengeCompleted;

    public bool isCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompleteChallenge()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            Debug.Log($"Challenge '{challengeName}' completed!");
            onChallengeCompleted.Invoke();
        }
    }
}
