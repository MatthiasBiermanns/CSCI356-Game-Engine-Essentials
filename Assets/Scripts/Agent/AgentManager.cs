using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentManager : MonoBehaviour
{
    public NavMeshAgent[] agents;
    
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var agent in agents)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateAgents()
    {
        foreach (var agent in agents)
        {
            agent.isStopped = false;
        }
    }

    public IEnumerator ActivateAgentsWithDelay(float delay = 2f)
    {
        foreach (var agent in agents)
        {
            agent.isStopped = false;
            yield return new WaitForSeconds(delay);
        }
    }

    public void SetTarget(Transform target)
    {
        foreach (var agent in agents)
        {
            agent.SetDestination(target.position);
        }
    }
}
