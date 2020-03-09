using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IAEnemyVirtual : MonoBehaviour
{
    public UnityAction OnFinishTurn;

    public NavMeshAgent myNavAgent;

    public float distanceOfDeplacement;
    public float attackRange;
    public int damage = 1;

    public GameObject player;
    [HideInInspector] public PlayerController playerControlleur;

    [HideInInspector] public Vector3 destination;

    public Animator myAnimator;

    public bool isPlaying = false;
    public bool isPreparing = false;

    public float durationTurn = 1;

    public ShieldManager myShieldManager;

    public bool haveDetectPlayer = false;
    public float detectionPlayerRange = 10;

    public LayerMask detectionMaskRaycast;

    public virtual void PlayerTurn() { }

    public bool CheckDetectionWithPlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < detectionPlayerRange)
        {
            Debug.DrawRay(transform.position, (player.transform.position - transform.position) + Vector3.up * 1, Color.green, 20);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.transform.position - transform.position) + Vector3.up * 1, out hit, detectionPlayerRange, detectionMaskRaycast))
            {
                if (hit.transform.gameObject == player)
                {
                    haveDetectPlayer = true;
                    return true;
                }
            }
        }
        return false;
    }
}
