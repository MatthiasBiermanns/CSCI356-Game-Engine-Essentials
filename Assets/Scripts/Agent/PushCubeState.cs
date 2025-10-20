using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PushCubeState : IAgentState
{
    Agent agent;
    NavMeshAgent nav;
    Rigidbody rb;

    float t;
    bool isPushing;

    public PushCubeState(Agent agent, NavMeshAgent nav, Rigidbody rb)
    {
        this.agent = agent;
        this.nav = nav;
        this.rb = rb;
    }

    public void OnEnter()
    {
        if (!agent.CubeTarget)
            return;
        
        agent.pushing = false;
        isPushing = false;
        t = 0f;

        Vector3 away = (agent.CubeTarget.position - agent.TriggerCenter); 
        away.y = 0f;
        agent.PushDir = away.sqrMagnitude > 0.0001f ? away.normalized : agent.transform.forward;


        if (nav && nav.isOnNavMesh)
        { 
            nav.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            nav.autoBraking = false;
            nav.stoppingDistance = agent.stoppingDistance;
            nav.SetDestination(agent.CubeTarget.position - agent.PushDir * agent.pushDistance);
        }
    }

    public void OnExit()
    {
        if (nav && nav.isOnNavMesh)
        {
            nav.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            nav.autoBraking = true;
            nav.ResetPath();
        }
        isPushing = false;
        agent.pushing = false;
    }

    public void Tick()
    {
        if (!nav || !nav.enabled) 
            return;
        if (agent.isPaused)
            return;

        if (!isPushing)
        {
            if (nav.isOnNavMesh && !nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
            {
                // Start pushing
                agent.pushing = true;
                isPushing = true;

                if (nav.isOnNavMesh)
                {
                    nav.updatePosition = false;
                    nav.updateRotation = false;
                }
                t = 0f;
            }
            return;
        }
        t += Time.deltaTime;
        if (rb)
        {
            Vector3 next = rb.position + agent.PushDir * (nav.speed * agent.pushMoveFactor) * Time.deltaTime;
            if (NavMesh.SamplePosition(next, out var hit, 0.3f, nav.areaMask)) next = hit.position;
            rb.MovePosition(next);
        }

        Vector3 away = (agent.CubeTarget.position - agent.TriggerCenter); away.y = 0f;
        if (away.sqrMagnitude > 0.0001f) agent.PushDir = away.normalized;

        if (t >= agent.pushTime)
        {
            // Finished pushing
            isPushing = false;
            agent.pushing = false;
            if (nav.isOnNavMesh)
            {
                nav.updatePosition = true;
                nav.updateRotation = true;
                nav.ResetPath();
            }
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
