using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Challenge : MonoBehaviour
{
    public string challengeName;
    public bool canUncomplete = false;

    [System.Serializable]
    public class ChallengeEvent : UnityEvent<Challenge> { };

    public ChallengeEvent onChallengeCompleted;
    public ChallengeEvent onChallengeUncompleted;

    [SerializeField] private Trigger[] triggers;
    private bool isCompleted = false;
    public bool isActive = false;

    public string helpText = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckCompleted()
    {
        if (!isActive)
        {
            return;
        }

        if (isCompleted && !canUncomplete)
        {
            return;
        }
        
        bool newCompleted = triggers.All((Trigger t) => t.getIsTriggered() == true);

        if (isCompleted != newCompleted)
        {
            isCompleted = newCompleted;
            if(isCompleted == true)
            {
                CompleteChallenge();
                Debug.Log("Challenge completed: " + challengeName);
                return;
            }

            if (canUncomplete == true)
            {
                UncompleteChallenge();
            }

        }
    }

    void CompleteChallenge()
    {
        if (onChallengeCompleted != null)
        {
            onChallengeCompleted.Invoke(this);
        }
    }

    void UncompleteChallenge()
    {
        if (!canUncomplete)
        {
            return;
        }

        if( onChallengeUncompleted != null )
        { 
            onChallengeUncompleted.Invoke(this); 
        }
    }

    public bool getIsCompleted()
    {
        if (!isActive)
        {
            return false;
        }
        return isCompleted;
        
    }

    internal void setIsCompleted(bool value)
    {
        if (isCompleted == value)
        {
            return;
        }
        isCompleted = value;

        if (isCompleted)
        {
            CompleteChallenge();
        } else if (canUncomplete)
        {
            UncompleteChallenge();
        }
    }
}
