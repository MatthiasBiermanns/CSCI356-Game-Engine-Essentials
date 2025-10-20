using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : IAgentState
{
    Agent agent;
    NavMeshAgent nav;
    float nextRepath;
    public AttackState(Agent agent, NavMeshAgent nav)
    {
        this.agent = agent;
        this.nav = nav;
    }
    public void OnEnter()
    {
        if (nav && nav.isOnNavMesh)
        { 
            nav.stoppingDistance = agent.stoppingDistance;
        }
    }
    public void OnExit()
    {

    }
    public void Tick()
    {
        if (!nav || !nav.isOnNavMesh || !nav.enabled)
            return;
        if (agent == null)
            return;
        
        if (Time.time >= nextRepath)
        {
            if (agent.player != null)
            {
                nav.SetDestination(agent.player.position);
            }
            nextRepath = Time.time + 0.5f;
        }

        if (agent.IsTouchingPlayer())
        {
            agent.AddTimePenalty();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
