using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Challenge : MonoBehaviour
{
    public string challengeName;
    public UnityEvent onChallengeCompleted;
    public TMP_Text label;

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
            if (label != null)
            {
                label.color = Color.green;
            }
        }
    }
}
