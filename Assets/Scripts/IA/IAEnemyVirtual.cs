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

    public ShieldManager myShieldManager;

    public bool haveDetectPlayer = false;
    public float detectionPlayerRange = 10;

    public LayerMask detectionMaskRaycast;

    public ShowPathSystem myShowPath;

    public LineRenderer lineTest;

    public virtual void PlayerTurn() { }

    public bool CheckDetectionWithPlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < detectionPlayerRange)
        {
            //Debug.DrawRay(transform.position, (player.transform.position - transform.position) + Vector3.up * 1, Color.magenta, 20);

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

    public Vector3 CalculDestination(Vector3 _targetPos) {

        myNavAgent.isStopped = true;
        NavMeshPath _path = new NavMeshPath();

        NavMesh.CalculatePath(transform.position, _targetPos, NavMesh.AllAreas, _path);

        float currentDistance = 0;

        for (var i = 1; i < _path.corners.Length; i++)
        {
            if (currentDistance + Vector3.Distance(_path.corners[i - 1], _path.corners[i]) < distanceOfDeplacement)
            {
                currentDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
            }
            else
            {
                //Calcule de coupure de la ligne
                float distanceTokeep = distanceOfDeplacement - currentDistance;

                Vector3 dir = _path.corners[i] - _path.corners[i - 1];
                Vector3 targetPosition = _path.corners[i - 1] + dir.normalized * distanceTokeep;

                return targetPosition;
            }
        }

        Debug.LogWarning("ENEMY : PATH NOT FOUND (" + _path.corners.Length + " points calculated)");
        return _targetPos;
    }

    public void LookPosition(Vector3 _pos)
    {
        transform.LookAt(_pos);
    }
}
