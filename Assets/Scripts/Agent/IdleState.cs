using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : IAgentState
{
    Agent agent;
    NavMeshAgent nav;
    public IdleState(Agent agent, NavMeshAgent nav)
    {
        this.agent = agent;
        this.nav = nav;
    }
    public void OnEnter()
    {
        if (nav && nav.isOnNavMesh)
        { 
            nav.ResetPath(); 
            nav.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
        agent.pushing = false;
    }
    public void OnExit()
    {
        nav.isStopped = false;
    }
    public void Tick()
    {
        
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
