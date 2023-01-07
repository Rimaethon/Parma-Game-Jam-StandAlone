using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachines : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            Debug.Log("Player is in range");
            agent.isStopped = true;
            Attack();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
    }

    private void Attack()
    {
        //Attack Func
    }
    public void Die()
    {
        //Die Func
    }
}
