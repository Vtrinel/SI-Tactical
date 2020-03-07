using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TouniEnemy : IAEnemyVirtual
{
    public float angleAttack;

    [SerializeField] LayerMask objCanBeAttecked;

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
            OnFinishTurn?.Invoke();
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
        isPlaying = true;
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

        isPlaying = false;
        myNavAgent.isStopped = true;
        OnFinishTurn?.Invoke();
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
        CollisionAttack();
    }

    void CollisionAttack()
    {
        List<GameObject> _objsTouched = GetListOfObjsTouched();

        foreach(GameObject _obj in _objsTouched)
        {
            _obj.GetComponent<DamageableEntity>().ReceiveDamage(DamageTag.Enemy, damage);
        }
    }

    List<GameObject> GetListOfObjsTouched()
    {
        float angle = angleAttack;
        float rayRange = attackRange;
        float halfFOV = angle / 2.0f;

        Quaternion upRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion downRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);

        Vector3 rightRayDirection = upRayRotation * transform.forward * rayRange;
        Vector3 leftRayDirection = downRayRotation * transform.forward * rayRange;

        List<GameObject> objTouched = new List<GameObject>();

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position + Vector3.up * 1, transform.forward, attackRange, objCanBeAttecked);
        foreach(RaycastHit hit in hits)
        {
            if (!objTouched.Contains(hit.collider.gameObject))
            {
                objTouched.Add(hit.collider.gameObject);
            }
        }

        hits = Physics.RaycastAll(transform.position + Vector3.up * 1, rightRayDirection, attackRange, objCanBeAttecked);
        foreach (RaycastHit hit in hits)
        {
            if (!objTouched.Contains(hit.collider.gameObject))
            {
                objTouched.Add(hit.collider.gameObject);
            }
        }

        hits = Physics.RaycastAll(transform.position + Vector3.up * 1, transform.forward, attackRange, objCanBeAttecked);
        foreach (RaycastHit hit in hits)
        {
            if (!objTouched.Contains(hit.collider.gameObject))
            {
                objTouched.Add(hit.collider.gameObject);
            }
        }

        return objTouched;
    }

    bool CanAttack()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            return true;
        }
        return false;
    }

    void ForceStopMyTurn()
    {
        StopAllCoroutines();
        OnFinishTurn?.Invoke();
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
