using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AgentSignal
{
    ChallengeCompleted,
    ChallengeUncompleted,
    TookDamage,
    Died
}
public class Agent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    public Transform player;
    public SceneController scene;

    public int lives = 3;
    public float speed = 1f;
    public bool isPaused = false;

    public float pushDistance = 1.2f;
    public float pushTime = 1.5f;
    public float pushMoveFactor = 0.8f;
    public float stoppingDistance = 0.05f;
    public float shoveForce = 60f;

    private float contactRadius = 1f;
    private float lastPenaltyTime = -999f;
    private float penaltySeconds = 5f;
    private float penaltyCooldown = 1f;

    StateMachine fsm;
    IdleState idle;
    PushCubeState push;
    AttackState attack;

    public Transform CubeTarget { get; set; }
    public Vector3 TriggerCenter { get; set; }
    public bool pushing { get; set; }
    public Vector3 PushDir { get; set; }
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        fsm = new StateMachine();
        idle = new IdleState(this, agent);
        push = new PushCubeState(this, agent, rb);
        attack = new AttackState(this, agent);

        if (scene == null)
            scene = FindObjectOfType<SceneController>();

        fsm.ChangeState(idle);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Update is called once per frame
    void Update()
    {
        fsm.Tick();
    }
    public bool IsTouchingPlayer()
    {
        if (player == null) 
            return false;
        return Vector3.Distance(player.position, transform.position) <= contactRadius;
    }

    public void StartPush(Transform cube, Vector3 triggerCenter)
    {
        CubeTarget = cube;
        TriggerCenter = triggerCenter;
        fsm.ChangeState(push);
    }
    public void TakeDamage(int damage = 1)
    {
        lives -= damage;
        if (lives <= 0)
        {
            Destroy(gameObject);
            return;
        }
        else if (lives <= 5)
        {
            agent.speed += 0.5f;
            fsm.ChangeState(attack);
        }
        else
        {
            StartCoroutine(PauseAfterHit(1.0f));
        }
    }

    IEnumerator PauseAfterHit(float pauseTime)
    {
        isPaused = true;
        
        if (agent && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(pauseTime);

        if (agent && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        isPaused = false;
    }

    public void OnChallengeUncompleted()
    {
        fsm.ChangeState(idle);
    }
    
    void OnCollisionStay(Collision c)
    {
        if (!pushing) return;

        Rigidbody hitRb = c.rigidbody;
        if (hitRb != null && hitRb.CompareTag("MovableCube"))
        {
            hitRb.AddForce(PushDir * shoveForce, ForceMode.Acceleration);
        }
    }

    public void TryApplyTimePenalty(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && fsm.Current == attack && Time.time - lastPenaltyTime > penaltyCooldown)
        {
            lastPenaltyTime = Time.time;
            scene.AddTimePenalty(5f);
            Debug.Log("Player hit by agent! Time penalty applied.");
        }
    }

    public void AddTimePenalty()
    {
        if (scene == null) 
            return;

        if (Time.time - lastPenaltyTime > penaltyCooldown)
        {
            lastPenaltyTime = Time.time;
            scene.AddTimePenalty(penaltySeconds);
            Debug.Log("Player hit by agent! Time penalty applied.");
        }
    }
}
