using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGate : MonoBehaviour
{
    [SerializeField] private ChallengeManager challengeManager;
    private SlideOpen door;
    private bool unlocked = false;

    void Start()
    {
        door = GetComponent<SlideOpen>();

        // subscribe once when the scene starts
        if (challengeManager != null)
            challengeManager.onAllChallengesCompleted.AddListener(UnlockDoor);
    }

    public void UnlockDoor()
    {
        unlocked = true;
        Debug.Log("Door unlocked!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (unlocked)
            {
                door.Open();
                Debug.Log("Player opened the door.");
            }
            else
            {
                Debug.Log("Door locked — complete all challenges first!");
            }
        }
    }
}
