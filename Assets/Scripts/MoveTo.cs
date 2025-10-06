using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public Transform goal;
    public NavMeshAgent agent;
    public float defaultMovementSpeed = 5.0f;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = defaultMovementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(ray, out mouseHit))
            {
                GameObject hitObject = mouseHit.transform.gameObject;

                if (hitObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    goal.position = mouseHit.point;
                    agent.destination = goal.position;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}
