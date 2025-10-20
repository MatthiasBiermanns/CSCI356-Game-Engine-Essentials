using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform target;
    private Rigidbody rb;

    public float pushDistance = 1.2f;
    public float pushTime = 1.5f;
    public float pushMoveFactor = 0.8f;
    public float stoppingDistance = 0.05f;
    public float shoveForce = 60f;

    float originalRadius;
    bool originalAutoBraking;
    ObstacleAvoidanceType originalAvoidanceType;
    bool pushing;
    Vector3 lastMoveDir = Vector3.forward;
    Vector3 dir;

    private bool active = true;
    private bool attacking = false;
    private float attackingSpeed = 3f;
    public int lives = 10;
    public int attackBelowLives = 5;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        originalRadius = agent.radius;
        originalAutoBraking = agent.autoBraking;
        originalAvoidanceType = agent.obstacleAvoidanceType;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.velocity.sqrMagnitude > 0.0001f)
            lastMoveDir = agent.velocity.normalized;
    }

    public void TakeDamage(int damage = 1)
    {
        lives -= damage;
        if (lives <= 0)
        {
            Destroy(gameObject);
            return;
        }
        else if (lives < 5 && !attacking)
        {
            StartCoroutine(AttackPlayer());
        }
        StartCoroutine(PauseMovement(2.0f));
    }

    IEnumerator AttackPlayer()
    {
        attacking = true;
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        while (player != null && lives > 0)
        {
            agent.SetDestination(player.position);
            agent.speed = attackingSpeed;
            yield return new WaitForSeconds(0.2f);
        }

        attacking = false;
    }

    IEnumerator PauseMovement(float duration)
    {
        active = false;
        agent.isStopped = true;
        yield return new WaitForSeconds(duration);
        agent.isStopped = false;
        active = true;
    }

    public void PushCube(Transform cube, Vector3 triggerCenter)
    {
        StopAllCoroutines();
        StartCoroutine(PushRoutine(cube, triggerCenter));
    }

    IEnumerator PushRoutine(Transform cube, Vector3 triggerCenter)
    {
        dir = (triggerCenter - cube.position).normalized;
        Vector3 target = cube.position;// + dir * pushDistance;

        // prepare to make physical contact
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.autoBraking = false;
        agent.stoppingDistance = stoppingDistance;
        
        agent.SetDestination(target);

        // move until we're right there (and colliding)
        while (agent != null && agent.enabled && agent.isOnNavMesh && 
            (agent == null || agent.pathPending || agent.remainingDistance > agent.stoppingDistance))
            yield return null;

        // hold pressure for a moment so the cube clears the trigger
        agent.updatePosition = false;
        agent.updateRotation = false;
        pushing = true;

        float t = 0f;
        while (t < pushTime)
        {
            t += Time.deltaTime;
            transform.position += dir * (agent.speed * pushMoveFactor) * Time.deltaTime;
            yield return null;
        }
        pushing = false;

        // restore defaults and stop
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.obstacleAvoidanceType = originalAvoidanceType;
        agent.autoBraking = originalAutoBraking;
        agent.radius = originalRadius;
        //agent.ResetPath();
    }

    void OnCollisionStay(Collision c)
    {
        if (!pushing) return;

        Rigidbody hitRb = c.rigidbody;
        if (hitRb != null && hitRb.CompareTag("MovableCube"))
        {
            hitRb.AddForce(dir * shoveForce, ForceMode.Acceleration);
        }
    }
}
