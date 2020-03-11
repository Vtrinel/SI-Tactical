using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ShowPathSystem : MonoBehaviour
{
    [SerializeField] LineRenderer lineDeplacement;
    [SerializeField] NavMeshAgent myNavAgent;
    float distance;
    float attackRange;

    bool showPreview = false;
    Transform target = default;

    bool playerTurn = false;
    public bool canCheck = true;

    [SerializeField] GameObject EsclamationPoint;

    Transform player;

    private void OnEnable()
    {
        TurnManager.Instance.OnStartPlayerTurn += SetPlayerTurn;
        TurnManager.Instance.OnEndPlayerTurn += UnSetPlayerTurn;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnStartPlayerTurn -= SetPlayerTurn;
        TurnManager.Instance.OnEndPlayerTurn -= UnSetPlayerTurn;
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer.transform;
    }

    void SetPlayerTurn()
    {
        playerTurn = true;
    }

    void UnSetPlayerTurn()
    {
        playerTurn = false;
    }

    public void SetValue(float _distance, float _attackRange)
    {
        distance = _distance;
        attackRange = _attackRange;
    }

    public void ShowOrHide(bool value)
    {
        showPreview = value;
    }

    public void SetTargetPosition(Transform targ)
    {
        target = targ;
    }

    private void Update()
    {
        if (playerTurn && canCheck)
        {
            if (showPreview)
            {
                List<Vector3> _canPos = DrawPath(target.position);

                if (_canPos != null)
                {
                    SetLines(_canPos);
                    EsclamationPoint.SetActive(true);
                }
                else
                {
                    lineDeplacement.positionCount = 0;
                    EsclamationPoint.SetActive(false);
                }
            }
            else
            {
                lineDeplacement.positionCount = 0;

                List<Vector3> _canPos = DrawPath(player.position);
                EsclamationPoint.SetActive(_canPos != null);
            }
        }
        else
        {
            EsclamationPoint.SetActive(false);
        }
    }

    public List<Vector3> DrawPath(Vector3 _target)
    {
        myNavAgent.isStopped = true;
        myNavAgent.SetDestination(_target);

        NavMeshPath _path = myNavAgent.path;

        List<Vector3> canPos = new List<Vector3>();
        canPos.Add(transform.position);


        float currentDistance = 0;
        float currentAttackDistance = 0;

        for (var i = 1; i < _path.corners.Length; i++)
        {
            if (currentDistance > distance)
            {
                if (currentAttackDistance + Vector3.Distance(canPos[canPos.Count - 1], _path.corners[i]) > attackRange)
                {
                    lineDeplacement.positionCount = 0;
                    return null;
                }
                else
                {
                    canPos.Add(_path.corners[i]);
                    currentAttackDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
                }
            }
            else
            {
                if (currentDistance + Vector3.Distance(_path.corners[i - 1], _path.corners[i]) < distance)
                {
                    canPos.Add(_path.corners[i]);
                    currentDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
                }
                else
                {
                    //Calcule de coupure de la ligne
                    float distanceTokeep = distance - currentDistance;

                    Vector3 dir = _path.corners[i] - _path.corners[i - 1];
                    Vector3 targetPosition = _path.corners[i - 1] + dir.normalized * distanceTokeep;

                    canPos.Add(targetPosition);

                    if (Vector3.Distance(targetPosition, _path.corners[i]) > attackRange)
                    {
                        return null;
                    }
                    else
                    {
                        canPos.Add(_path.corners[i]);
                        currentAttackDistance += Vector3.Distance(targetPosition, _path.corners[i]);
                    }

                    currentDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
                }
            }
        }

        return canPos;
    }

    void SetLines(List<Vector3> _canPos)
    {
        lineDeplacement.positionCount = _canPos.Count;
        lineDeplacement.SetPositions(_canPos.ToArray());
    }
}
