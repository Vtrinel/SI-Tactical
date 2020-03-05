using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent myNavAgent;

    [SerializeField] float distanceOfDeplacement;
    [SerializeField] float attackRange;

    GameObject player;

    Vector3 destination;

    public UnityAction OnIsAtDestination;

    private void Start()
    {
        player = GameManager.Instance.GetPlayer.gameObject;
    }

    public void PlayerTurn()
    {
        if (CanAttack())
        {
            Attack();
            print("attack");
        }
        else
        {
            Move();
            print("cant attack");
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
        Vector3 pos = player.transform.position;
        Vector3 dir = (this.transform.position - player.transform.position).normalized;

        Debug.DrawLine(pos, pos + dir * 10, Color.red, Mathf.Infinity);

        return dir * distanceOfDeplacement;
    }

    IEnumerator WaitDeplacement()
    {
        while(Vector3.Distance(transform.position, destination) > 0.1f)
        {
            if (CanAttack())
            {
                Attack();
                myNavAgent.isStopped = true;
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        OnIsAtDestination?.Invoke();
    }

    void Attack()
    {

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
    }
}
