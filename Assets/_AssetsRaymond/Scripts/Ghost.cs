using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public float eatDistance = 0.3f;
    public Animator animator;
    public NavMeshAgent agent;
    public float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public GameObject GetClosestOrb()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        List<GameObject> orbs = OrbsSpawner.instance.spawnedOrbs;

        foreach(var item in orbs)
        {
            Vector3 ghostPosition = transform.position;
            ghostPosition.y = 0;
            Vector3 orbPosition = item.transform.position;
            orbPosition.y = 0;

            float d = Vector3.Distance(ghostPosition, orbPosition);

            if (d < minDistance)
            {
                minDistance = d;
                closest = item;
            }
        }

        if (minDistance < eatDistance)
        {
            OrbsSpawner.instance.DestroyOrb(closest);
        }

        return closest;
    }
    // Update is called once per frame
    void Update()
    {
        if (!agent.enabled)
        {
            return;
        }
        GameObject closest = GetClosestOrb();

        if(closest)
        {
            Vector3 targetPosition = closest.transform.position;
            agent.SetDestination(targetPosition);
            agent.speed = speed;
        }
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
