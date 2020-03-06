using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CultisteEnemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent myNavAgent;

    [SerializeField] float distanceOfDeplacement;
    public float attackRange;

    public float angleAttack;

    GameObject player;
    PlayerController playerControlleur;

    Vector3 destination;

    public UnityAction OnIsAtDestination;

    [SerializeField] Animator myAnimator;

    bool isPreparing = false;

    [SerializeField] float durationTurn = 1;
    public bool haveDisc = false;

    private void OnEnable()
    {
        TurnManager.Instance.OnEnemyTurnInterruption += ForceStopMyTurn;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnEnemyTurnInterruption -= ForceStopMyTurn;
    }

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
        destination = player.transform.position;


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
        float normalizedTime = 0;

        while (normalizedTime < durationTurn)
        {
            normalizedTime += Time.deltaTime;

            print(normalizedTime);
            if (CanAttack())
            {
                normalizedTime = durationTurn + 1;
                PrepareAttack();
                break;
            }
            yield return null;
        }

        myNavAgent.isStopped = true;
        OnIsAtDestination?.Invoke();
    }

    void PrepareAttack()
    {
        myAnimator.SetBool("Preparing", true);
        isPreparing = true;

        transform.LookAt(CalculDestination());
    }

    void Attack()
    {
        myAnimator.SetTrigger("Attack");
        myAnimator.SetBool("Preparing", false);
        LaunchDisc();
    }

    void LaunchDisc()
    {

    }

    bool CanAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange && haveDisc) 
        {
            Vector3 forward = transform.TransformDirection(player.transform.position) * attackRange;
            Debug.DrawRay(transform.position, forward, Color.green);

            RaycastHit hit;
            if(Physics.Raycast(transform.position, forward, out hit ,attackRange))
            {
                if(hit.collider.gameObject == player)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void ForceStopMyTurn()
    {
        StopAllCoroutines();
        OnIsAtDestination?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceOfDeplacement);

        Gizmos.color = Color.cyan;

        float angle = angleAttack;
        float rayRange = attackRange;
        float halfFOV = angle / 2.0f;

        Quaternion upRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion downRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);

        Vector3 rightRayDirection = upRayRotation * transform.forward * rayRange;
        Vector3 leftRayDirection = downRayRotation * transform.forward * rayRange;

        Gizmos.DrawRay(transform.position, rightRayDirection);
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawLine(transform.position + leftRayDirection, transform.position + rightRayDirection);
    }
}
