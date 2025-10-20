using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    public Transform agentSpawnPoint;
    private GameObject agent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnAgentForChallenge(Challenge challenge)
    {
        // Go through all triggers that belong to this challenge
        foreach (var trigger in challenge.GetComponentsInChildren<Trigger>())
        {
            GameObject cube = trigger.GetCubeInZone(); // get current cube in trigger

            if (cube != null)
            {
                Debug.Log($"AgentSpawner: Spawning agent to push cube {cube.name} from {trigger.name}");
                StartCoroutine(SpawnAndPush(cube, trigger.transform.position));
                return;
            }
        }

        // If no cube found, just aim for the first trigger as fallback
        var fallback = challenge.GetComponentsInChildren<Trigger>()[0];
        StartCoroutine(SpawnAndPush(fallback.gameObject, fallback.transform.position));
    }

    IEnumerator SpawnAndPush(GameObject cube, Vector3 triggerCenter)
    {
        // Spawn the agent
        GameObject agentObj = Instantiate(agentPrefab, agentSpawnPoint.position, Quaternion.identity);
        agent = agentObj;

        var agentLogic = agentObj.GetComponent<Agent>();
        if (!agentLogic) agentLogic = agentObj.AddComponent<Agent>();

        agentLogic.StartPush(cube.transform, triggerCenter);

        yield return null;
    }

    public void DestroyCurrentAgent()
    {
        if (agent != null)
        {
            Destroy(agent);
            agent = null;
        }
    }
}
