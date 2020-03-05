using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent myNavAgent;

    [SerializeField] float distanceOfDeplacement;
    public float attackRange;
    [SerializeField] int damage = 1;

    public float angleAttack;

    GameObject player;
    PlayerController playerControlleur;

    Vector3 destination;

    public UnityAction OnIsAtDestination;

    [SerializeField] Animator myAnimator;

    bool isPreparing = false;

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

        transform.LookAt(CalculDestination());
    }

    void Attack()
    {
        myAnimator.SetTrigger("Attack");
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
