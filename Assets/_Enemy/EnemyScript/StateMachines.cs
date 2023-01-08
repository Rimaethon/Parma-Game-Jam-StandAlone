using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachines : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent agent;
    public Animator EnemyAnim;
    int swordCount=0;


    private void OnEnable()
    {
        EnemyAnim = GetComponent<Animator>();
        EnemyAnim.SetBool("isdead", false);
    }
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        


    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            EnemyAnim.SetBool("iswalk", false);
            Debug.Log("Player is in range");
            agent.isStopped = true;
            Attack();
        }
        else
        {
            EnemyAnim.SetBool("iswalk", true);
            EnemyAnim.SetBool("isattack", false);
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
    }

    private void Attack()
    {
        EnemyAnim.SetBool("isattack", true);
    }
    public void Die()
    {

        
        EnemyAnim.SetBool("isdead", true);
        FindObjectOfType<Pool>().Die(this.gameObject);
        EnemyAnim.SetBool("isdead", false);

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Sword"))
        {
            swordCount++;
            if(swordCount==3)
            {
                Die();
            }
        }
    }
}
