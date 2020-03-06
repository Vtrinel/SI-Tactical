using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CultisteEnemy : IAEnemyVirtual
{
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

    public override void PlayerTurn()
    {
        StartCoroutine(PlayerTurnCouroutine());
    }

    IEnumerator PlayerTurnCouroutine()
    {
        if (isPreparing)
        {
            Attack();
            isPreparing = false;
            yield return new WaitForSeconds(1);
        }

        if (CanAttack())
        {
            PrepareAttack();
            yield return new WaitForSeconds(0.4f);
            OnIsAtDestination?.Invoke();
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        Transform newObjDestination = ResershDisc();
        if (newObjDestination == null)
        {
            print("<color=green> Null </color>");
            OnIsAtDestination?.Invoke();
            return;
        }

        destination = ResershDisc().position;

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

    Transform ResershDisc()
    {
        List<DiscScript> allCurrentDisc = DiscManager.Instance.GetAllDisclUse();

        if(allCurrentDisc.Count == 0) { return null; }

        List<Transform> allObjTransfrom = new List<Transform>();
        foreach(DiscScript disc in allCurrentDisc)
        {
            allObjTransfrom.Add(disc.transform); 
        }

        return EnemyUtilitiesFactory.GetTheCloserObjOfMe(transform ,allObjTransfrom);
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
    }
}
