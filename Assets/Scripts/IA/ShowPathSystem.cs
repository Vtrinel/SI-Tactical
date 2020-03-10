using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShowPathSystem : MonoBehaviour
{
    [SerializeField] LineRenderer lineDeplacement;
    [SerializeField] LineRenderer lineCantDeplacement;
    [SerializeField] NavMeshAgent myNavAgent;
    float distance;
    float attackRange;

    bool showPreview = false;
    Transform target = default;

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
        return;

        if (showPreview)
        {
            DrawPath();
        }
        else
        {
            lineDeplacement.positionCount = 0;
            lineCantDeplacement.positionCount = 0;
        }
    }

    public void DrawPath()
    {
        myNavAgent.isStopped = true;
        myNavAgent.SetDestination(target.position);

        NavMeshPath _path = myNavAgent.path;

        List<Vector3> canPos = new List<Vector3>();
        canPos.Add(transform.position);

        List<Vector3> attackPos = new List<Vector3>();

        float currentDistance = 0;
        float currentAttackDistance = 0;

        for (var i = 1; i < _path.corners.Length; i++)
        {
            if (currentDistance > distance)
            {
                if(currentAttackDistance + Vector3.Distance(attackPos[attackPos.Count - 1], _path.corners[i]) > attackRange)
                {
                    lineDeplacement.positionCount = 0;
                    lineCantDeplacement.positionCount = 0;
                    return;
                }
                else
                {
                    attackPos.Add(_path.corners[i]);
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

                    attackPos.Add(targetPosition);

                    if(Vector3.Distance(targetPosition, _path.corners[i]) > attackRange)
                    {
                        lineDeplacement.positionCount = 0;
                        lineCantDeplacement.positionCount = 0;
                        return;
                    }
                    else
                    {
                        attackPos.Add(_path.corners[i]);
                        currentAttackDistance += Vector3.Distance(targetPosition, _path.corners[i]);
                    }

                    currentDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
                }
            }

            SetLines(canPos, attackPos);
        }
    }

    void SetLines(List<Vector3> _canPos, List<Vector3> _attackPos)
    {
        lineDeplacement.positionCount = _canPos.Count;
        lineDeplacement.SetPositions(_canPos.ToArray());

        lineCantDeplacement.positionCount = _attackPos.Count;
        lineCantDeplacement.SetPositions(_attackPos.ToArray());
    }
}
