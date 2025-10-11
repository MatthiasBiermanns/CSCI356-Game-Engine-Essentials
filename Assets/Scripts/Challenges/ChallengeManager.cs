using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChallengeManager : MonoBehaviour
{
    public List<Challenge> challenges = new List<Challenge>();
    public int remainingChallenges = 0;
    public UnityEvent onAllChallengesCompleted;

    public UIController uiController;

    public bool allChallengesCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var challenge in challenges)
        {
            if (challenge != null && !challenge.isCompleted)
            {
                remainingChallenges++;
                challenge.onChallengeCompleted.AddListener(OnChallengeCompleted);
                allChallengesCompleted = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnChallengeCompleted()
    {
        remainingChallenges--;
        uiController.UpdateProgressSmooth(1.0f / challenges.Count);
        Debug.Log($"ChallengeManager: Remaining challenges: {remainingChallenges}");
        if (remainingChallenges <= 0)
        {
            Debug.Log("ChallengeManager: All challenges completed!");
            allChallengesCompleted = true;
            onAllChallengesCompleted.Invoke();
        }
    }
}
