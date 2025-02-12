using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;
    private NavMeshAgent navAgent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (navAgent.velocity.magnitude > 0.1)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            int randomValue = Random.Range(0, 2);
            if (randomValue == 0)
            {
                animator.SetTrigger("Die1");
            }
            else
            {
                animator.SetTrigger("Die2");
            }
        }
        else
        {
            animator.SetTrigger("Damage");
        }
    }
}
