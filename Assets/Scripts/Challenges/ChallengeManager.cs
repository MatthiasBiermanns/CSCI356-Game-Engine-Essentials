using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ChallengeManager : MonoBehaviour
{
    public Challenge[] challenges = {};
    private List<Challenge> toCheckChallenges = new();
    public int remainingChallenges;
    public UnityEvent onAllChallengesCompleted;
    [SerializeField] private bool enforceOrder = false;

    public UIController uiController;

    private bool allChallengesCompleted = true;
    private bool challengesFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        bool lastCompleted = true;
        foreach (Challenge challenge in challenges)
        {
            if (!challenge.getIsCompleted())
            {
                remainingChallenges++;
                toCheckChallenges.Add(challenge);
                challenge.onChallengeCompleted.AddListener(OnChallengeCompleted);

                if (challenge.canUncomplete)
                {
                    challenge.onChallengeUncompleted.AddListener(OnChallengeUncompleted);
                }

                allChallengesCompleted = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check to avoid unnecessary calculations
        if (challengesFinished)
        { 
            return; 
        }

        foreach (Challenge challenge in toCheckChallenges.ToList())
        {
            challenge.CheckCompleted();
        }

        allChallengesCompleted = toCheckChallenges.All((Challenge c) => c.getIsCompleted());

        if (allChallengesCompleted)
        {
            onAllChallengesCompleted.Invoke();
            challengesFinished = true;
        }
    }

    void OnChallengeCompleted(Challenge completedChallenge)
    {
        remainingChallenges--;
        uiController.UpdateProgressSmooth(1.0f / challenges.Length);
        Debug.Log($"ChallengeManager: Remaining challenges: {remainingChallenges}");
        if (remainingChallenges <= 0)
        {
            Debug.Log("ChallengeManager: All challenges completed!");
            allChallengesCompleted = true;
            onAllChallengesCompleted.Invoke();
        }

        if (!completedChallenge.canUncomplete)
        {
            toCheckChallenges.Remove(completedChallenge);
        }
    }
    void OnChallengeUncompleted(Challenge completedChallenge)
    {
        remainingChallenges++;
        uiController.UpdateProgressSmooth(1.0f / challenges.Length);
        Debug.Log($"ChallengeManager: Remaining challenges: {remainingChallenges}");
    }
}
