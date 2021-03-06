﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TouniEnemy : IAEnemyVirtual
{
    public float angleAttack;

    [SerializeField] LayerMask objCanBeAttacked;

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
        myShieldManager.myObjParent = gameObject;
    }

    public override void PlayerTurn()
    {
        if (!haveDetectPlayer)
        {
            if (!CheckDetectionWithPlayer())
            {
                OnFinishTurn?.Invoke();
                return;
            }
        }

        StartCoroutine(PlayerTurnCouroutine());
    }

    IEnumerator PlayerTurnCouroutine()
    {
        if (CanAttack())
        {
            PrepareAttack();
            while (isPreparing)
            {
                yield return new WaitForEndOfFrame();
            }
            /*yield return new WaitForSeconds(0.4f);
            Attack();
            isPreparing = false;
            yield return new WaitForSeconds(0.4f);
            OnFinishTurn?.Invoke();*/
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        myAnimator.SetBool("Walking", true);
        destination = CalculDestination(player.transform.position);

        LookPosition(destination);

        myNavAgent.isStopped = false;
        myNavAgent.SetDestination(destination);

        StartCoroutine(WaitDeplacement());
    }

    IEnumerator WaitDeplacement()
    {
        do
        {
            if (CanAttack())
            {
                myAnimator.SetBool("Walking", false);
                myNavAgent.isStopped = true;
                PrepareAttack();
                while (isPreparing)
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(0.4f);
                break;
            }
            yield return null;

            SoundManager.Instance.PlaySound(Sound.EnemyMove, gameObject.transform.position);

        } while (myNavAgent.remainingDistance != 0);


        myAnimator.SetBool("Walking", false);
        isPlaying = false;
        myNavAgent.isStopped = true;
        OnFinishTurn?.Invoke();
    }

    void PrepareAttack()
    {
        myAnimator.SetTrigger("Attack");
        isPreparing = true;

        transform.LookAt(player.transform);

        animationEventContainer.SetEvent(PlayAttackSound);
    }

    public void PlayAttackSound()
    {
        SoundManager.Instance.PlaySound(Sound.TouniATK, GameManager.Instance.GetPlayer.transform.position);
        animationEventContainer.SetEvent(Attack);
    }

    void Attack()
    {
        CollisionAttack();
        isPreparing = false;

        GameManager.Instance.GetPlayer.damageReceiptionSystem.ReceiveDamage(DamageTag.Enemy, new DamagesParameters(damage));
        animationEventContainer.SetEvent(CheckForIdleBreak);
        SoundManager.Instance.PlaySound(Sound.PlayerGetHit, gameObject.transform.position);
    }

    void CollisionAttack()
    {
        List<GameObject> _objsTouched = GetListOfObjsTouched();

        foreach(GameObject _obj in _objsTouched)
        {
            DamageableEntity hitDamageableEntity = _obj.GetComponent<DamageableEntity>();
            if (hitDamageableEntity != null && hitDamageableEntity.gameObject != player)
                hitDamageableEntity.ReceiveDamage(DamageTag.Enemy, new DamagesParameters(damage));
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
        hits = Physics.RaycastAll(transform.position + Vector3.up * 1, transform.forward, attackRange, objCanBeAttacked);
        foreach(RaycastHit hit in hits)
        {
            if (!objTouched.Contains(hit.collider.gameObject))
            {
                objTouched.Add(hit.collider.gameObject);
            }
        }

        hits = Physics.RaycastAll(transform.position + Vector3.up * 1, rightRayDirection, attackRange, objCanBeAttacked);
        foreach (RaycastHit hit in hits)
        {
            if (!objTouched.Contains(hit.collider.gameObject))
            {
                objTouched.Add(hit.collider.gameObject);
            }
        }

        hits = Physics.RaycastAll(transform.position + Vector3.up * 1, transform.forward, attackRange, objCanBeAttacked);
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

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionPlayerRange);
    }
}
