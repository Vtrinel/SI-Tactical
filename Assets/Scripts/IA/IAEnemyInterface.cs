using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IAEnemyVirtual : MonoBehaviour
{
    public UnityAction OnIsAtDestination;

    public NavMeshAgent myNavAgent;

    public float distanceOfDeplacement;
    public float attackRange;
    public int damage = 1;

    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerController playerControlleur;

    [HideInInspector] public Vector3 destination;

    public Animator myAnimator;

    public bool isPreparing = false;

    public float durationTurn = 1;

    public virtual void PlayerTurn() { }
}
