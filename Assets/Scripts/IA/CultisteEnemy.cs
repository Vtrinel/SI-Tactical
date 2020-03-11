using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CultisteEnemy : IAEnemyVirtual
{
    public bool haveDisc = false;
    Vector3 shootPos;

    [SerializeField] LineRenderer myLine;
    [SerializeField] GameObject myArrow;
    [SerializeField] GameObject posEndLine;
    [SerializeField] GameObject projectileObj;
    [SerializeField] GameObject projectilePrefab;

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

        myNavAgent.isStopped = true;
        myNavAgent.SetDestination(player.transform.position);
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
        projectileObj.SetActive(haveDisc);
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
        if (!haveDisc)
        {
            Transform newObjDestination = ResershDisc();
            if (newObjDestination == null)
            {
                OnFinishTurn?.Invoke();
                return;
            }
            destination = CalculDestination(newObjDestination.position);
        }
        else
        {
            destination = CalculDestination(player.transform.position);
        }

        LookPosition(destination);

        myNavAgent.SetDestination(destination);
        myNavAgent.isStopped = false;


        StartCoroutine(WaitDeplacement());
    }

    IEnumerator WaitDeplacement()
    {
        isPlaying = true;

        do
        {
            if (CanAttack())
            {
                myNavAgent.isStopped = true;

                PrepareAttack();
                break;
            }
            yield return null;

            SoundManager.Instance.PlaySound(Sound.EnemyMove, gameObject.transform.position);

        } while (myNavAgent.remainingDistance != 0);

        yield return new WaitForSeconds(0.4f);

        //si il a chopé un disc sur la route
        if (CanAttack() && !isPreparing)
        {
            PrepareAttack();
            yield return new WaitForSeconds(0.4f);
        }

        isPlaying = false;
        myNavAgent.isStopped = true;
        OnFinishTurn?.Invoke();
    }

    void PrepareAttack()
    {
        myNavAgent.isStopped = true;
        myAnimator.SetBool("Preparing", true);
        transform.LookAt(player.transform);

        shootPos = player.transform.position;
        isPreparing = true;
    }

    void SetPreview()
    {
        myArrow.transform.position = shootPos;

        //direction de l'arrow
        var lookPos = myArrow.transform.position - transform.position;
        lookPos.y = 0;
        myArrow.transform.rotation = Quaternion.LookRotation(lookPos);

        myLine.SetPosition(0, transform.position);
        myLine.SetPosition(1, posEndLine.transform.position);
        //myLine.gameObject.SetActive(true);
    }

    void Attack()
    {
        myAnimator.SetTrigger("Attack");
        myAnimator.SetBool("Preparing", false);
        LaunchObj();
        SoundManager.Instance.PlaySound(Sound.CultistATK, gameObject.transform.position);
    }

    void LaunchObj()
    {
        GameObject newProjectile = Instantiate(projectilePrefab);
        newProjectile.transform.position = transform.position;

        newProjectile.GetComponent<ProjectileScript>().SetDestination(myArrow.transform.position, gameObject);
    }

    Transform ResershDisc()
    {
        List<DiscScript> allCurrentDisc = DiscManager.Instance.GetAllInGameDiscs;

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
            Debug.DrawRay(transform.position, (player.transform.position - transform.position) + Vector3.up * 1, Color.green, 20);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.transform.position - transform.position) + Vector3.up * 1, out hit, attackRange))
            {
                if (hit.transform.gameObject == player)
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
        OnFinishTurn?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceOfDeplacement);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionPlayerRange);
    }

    private void Update()
    {
        if (isPreparing)
        {
            SetPreview();
        }

        myShowPath.canCheck = haveDisc;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11 && !haveDisc && isPlaying)
        {
            DiscScript touchedDisc = other.GetComponent<DiscScript>();
            if (touchedDisc != null)
            {
                if (!touchedDisc.isAttacking)
                {
                    DiscManager.Instance.DestroyDisc(touchedDisc);
                    haveDisc = true;
                }
            }
        }
    }
}
