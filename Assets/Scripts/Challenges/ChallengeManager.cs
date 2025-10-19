using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ChallengeManager : MonoBehaviour
{
    public UIController uiController;
    [SerializeField] Challenge[] challenges = {};

    [SerializeField] private bool enforceOrder = false;

    [SerializeField] int triggerIntermediatePosition;
    [SerializeField] bool isUniqueIntermediateTrigger = true;
    public UnityEvent onIntermediateCompleted;
    public UnityEvent onAllChallengesCompleted;

    private bool intermediateTriggered = false;
    private int remainingChallenges;

    // Start is called before the first frame update
    void Start()
    {
        // if enforceOrder, only activate first challenge
        if (enforceOrder && challenges.Length > 0)
        {
            challenges[0].isActive = true;
        }

        foreach (Challenge challenge in challenges)
        {
            // activate all, if no order necessary
            if (!enforceOrder)
            {
                challenge.isActive = true;
            }

            challenge.onChallengeCompleted.AddListener(OnChallengeCompleted);

            if (challenge.canUncomplete)
            {
                challenge.onChallengeUncompleted.AddListener(OnChallengeUncompleted);
            }
        }

        remainingChallenges = GetRemainingChallenges();

        if (remainingChallenges <= 0)
        {
            // invoke, to not cause issues
            onAllChallengesCompleted.Invoke();
        }

        SetupHelpTexts();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Challenge challenge in challenges)
        {
            challenge.CheckCompleted();
        }
    }

    void OnChallengeCompleted(Challenge completedChallenge)
    {
        // activate next uncompleted challenge
        if (enforceOrder)
        {
            int challengeIndex = Array.IndexOf(challenges, completedChallenge);

            for (int i = challengeIndex; i < challenges.Length-1; i++)
            {
                // activate next challenge
                challenges[i + 1].isActive = true;

                // keep on going until first challenge active and uncompleted
                if (!challenges[i + 1].getIsCompleted())
                {
                    break;
                }
            }
        }

        remainingChallenges = GetRemainingChallenges();
        Debug.Log($"ChallengeManager: Remaining challenges: {remainingChallenges}");

        UpdateProgressUi();
        UpdateHelpTextStati();

        if (!intermediateTriggered)
        {
            if (triggerIntermediatePosition <= challenges.Length - remainingChallenges)
            {
                intermediateTriggered = true;
                onIntermediateCompleted.Invoke();
            }
        }


        if (remainingChallenges <= 0)
        {
            Debug.Log("ChallengeManager: All challenges completed!");
            onAllChallengesCompleted.Invoke();

            // deactivate challenge Manager now
            gameObject.SetActive(false);
        }
    }
    void OnChallengeUncompleted(Challenge completedChallenge)
    {
        // handle uncomplete for following challenges
        if (enforceOrder)
        {
            int challengeIndex = Array.IndexOf(challenges, completedChallenge);

            // deaactivate & if possible uncomplete challenges
            for (int i = challengeIndex+1; i < challenges.Length; i++)
            {
                // if inactive challenges reached, end
                if (!challenges[i].isActive)
                {
                    break;
                }
                //challenges[i].setIsCompleted(false);
                challenges[i].isActive = false;
            }
        }

        if (intermediateTriggered && !isUniqueIntermediateTrigger)
        {
            intermediateTriggered = false;
        }

        remainingChallenges = GetRemainingChallenges();
        Debug.Log($"ChallengeManager: Remaining challenges: {remainingChallenges}");
        UpdateProgressUi();
        UpdateHelpTextStati();
    }

    void UpdateProgressUi()
    {
        uiController.UpdateProgressSmooth((1.0f / challenges.Length) * (challenges.Length - remainingChallenges));
    }

    int GetRemainingChallenges()
    {
        return challenges.Length - challenges.Count((Challenge c) => c.getIsCompleted());
    }

    void SetupHelpTexts()
    {
        Debug.Log("ChallengeManager: " + uiController.helpTexts.Length);
        for (int i = 0; i < uiController.helpTexts.Length; i++)
        { 
            // if less challenges then help text space, disable help text
            if (i >= challenges.Length)
            {
                Debug.Log("ChallengeManager: disable help text: " + i.ToString());
                uiController.SetShowHelpText(i, false);
                continue;
            }

            Debug.Log("ChallengeManager: set help text " + i.ToString() + " to " + challenges[i].helpText);

            // if challenge exists, enable help text and set text
            uiController.SetShowHelpText(i, true);
            uiController.SetHelpText(i, challenges[i].helpText);
        }
        UpdateHelpTextStati();
    }

    void UpdateHelpTextStati()
    {
        for(int i = 0; i < challenges.Length; i++)
        {
            Challenge c = challenges[i]; 

            HelpTextState state;
            if (c.isActive)
            {
                if (c.getIsCompleted())
                {
                    state = HelpTextState.ActiveComplete;
                } else
                {
                    state = HelpTextState.ActiveIncomplete;
                }
            } else
            {
                if (c.getIsCompleted())
                {
                    state = HelpTextState.InactiveComplete;
                }
                else
                {
                    state = HelpTextState.InactiveIncomplete;
                }
            }

            uiController.UpdateHelpTextColor(i, state);
        }
    }
}
