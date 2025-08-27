using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    public float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.enabled)
        {
            return;
        }
        Vector3 targetPosition = Camera.main.transform.position;

        agent.SetDestination(targetPosition);
        agent.speed = speed;
    }

    public void Kill()
    {
        agent.enabled = false;
        animator.SetTrigger("Death");
    }

    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}
