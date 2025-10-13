using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentHealth : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool active = true;
    public int lives = 3;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage = 1)
    {
        lives -= damage;
        if (lives <= 0)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(PauseMovement(2.0f));
    }

    IEnumerator PauseMovement(float duration)
    {
        active = false;
        agent.isStopped = true;
        yield return new WaitForSeconds(duration);
        agent.isStopped = false;
        active = true;
    }

}
