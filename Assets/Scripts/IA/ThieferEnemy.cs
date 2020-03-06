using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class ThieferEnemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent myNavAgent;

    [SerializeField] float distanceOfDeplacement;
    [SerializeField] float attackRange;
    [SerializeField] int damage = 1;

    GameObject player;
    PlayerController playerControlleur;

    Vector3 destination;

    public UnityAction OnIsAtDestination;

    [SerializeField] Animator myAnimator;

    bool haveDisc = false;

    private void Start()
    {
        playerControlleur = GameManager.Instance.GetPlayer;
        player = playerControlleur.gameObject;
    }

    public void PlayerTurn()
    {
        if (CanAttack())
        {
            Attack();
            print("attack");
            OnIsAtDestination?.Invoke();
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
                Attack();
                myNavAgent.isStopped = true;
                print("sdfhhjf");
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        OnIsAtDestination?.Invoke();
    }

    void Attack()
    {
        myAnimator.SetTrigger("Attack");
        playerControlleur.damageReceiptionSystem.LoseLife(damage);
    }

    bool CanAttack()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < attackRange && haveDisc)
        {
            return true;
        }
        return false;
    }

    void FindDisc()
    {


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceOfDeplacement);
    }
}
