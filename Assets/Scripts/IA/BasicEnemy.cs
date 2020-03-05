﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent myNavAgent;

    [SerializeField] float distanceOfDeplacement;
    [SerializeField] float attackRange;
    [SerializeField] int damage = 1;

    [SerializeField] float angleAttack;

    GameObject player;
    PlayerController playerControlleur;

    Vector3 destination;

    public UnityAction OnIsAtDestination;

    [SerializeField] Animator myAnimator;

    bool isPreparing = false;

    private void Start()
    {
        playerControlleur = GameManager.Instance.GetPlayer;
        player = playerControlleur.gameObject;
    }

    public void PlayerTurn()
    {
        if (isPreparing)
        {
            Attack();
            isPreparing = false;
        }

        if (CanAttack())
        {
            PrepareAttack();
            OnIsAtDestination?.Invoke();
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        destination = CalculDestination();


        myNavAgent.SetDestination(destination);
        myNavAgent.isStopped = false;

        StartCoroutine(WaitDeplacement());
    }

    Vector3 CalculDestination()
    {
        Vector3 pos = transform.position;
        Vector3 dir = (this.transform.position - player.transform.position).normalized;

        Debug.DrawLine(pos, pos + -dir * distanceOfDeplacement, Color.red, Mathf.Infinity);

        return pos + -dir * distanceOfDeplacement;
    }

    IEnumerator WaitDeplacement()
    {
        while(Vector3.Distance(transform.position, destination) > 0.1f)
        {
            if (CanAttack())
            {
                PrepareAttack();
                myNavAgent.isStopped = true;
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        OnIsAtDestination?.Invoke();
    }

    void PrepareAttack()
    {
        myAnimator.SetBool("Preparing", true);
        isPreparing = true;
    }

    void Attack()
    {
        myAnimator.SetTrigger("Attack");
        CollisionAttack();
    }

    void CollisionAttack()
    {
        playerControlleur.damageReceiptionSystem.ReceiveDamage(DamageTag.Enemy ,damage);
    }

    bool CanAttack()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceOfDeplacement);


        Gizmos.color = Color.cyan;
        int quality = 15;

        float dist_max = attackRange;

        Vector3 pos = transform.position;
        float angle_lookat = 0;
        float angle_fov = angleAttack;

        float angle_start = angle_lookat - angle_fov;
        float angle_end = angle_lookat + angle_fov;
        float angle_delta = (angle_end - angle_start) / quality;

        
        float angle_curr = transform.rotation.y;
        float angle_next = angle_delta;

        for (int i = 0; i < quality - 1; i++)
        {
            Vector3 sphere_curr = Vector3.zero;
            sphere_curr.x = Mathf.Cos(Mathf.Deg2Rad * angle_curr);
            sphere_curr.z = Mathf.Sin(Mathf.Deg2Rad * angle_curr);

            Vector3 sphere_next = Vector3.zero;
            sphere_next.x = Mathf.Cos(Mathf.Deg2Rad * angle_next);
            sphere_next.z = Mathf.Sin(Mathf.Deg2Rad * angle_next);

            Vector3 pos_curr_max = pos + sphere_curr * dist_max;

            Vector3 pos_next_max = pos + sphere_next * dist_max;

            Gizmos.DrawLine(transform.position, pos_curr_max);
            Gizmos.DrawLine(transform.position, pos_next_max);

            angle_curr += angle_delta;
            angle_next += angle_delta;

        }
    }
}
