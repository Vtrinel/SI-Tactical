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

    public void SetValue(float _distance, float _attackRange)
    {
        distance = _distance;
        attackRange = _attackRange;
    }

    public void ShowOrHide(bool value)
    {
        showPreview = value;
    }

    private void Update()
    {
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
        myNavAgent.SetDestination(GameManager.Instance.GetCurrentWorldMouseResult.mouseWorldPosition);

        NavMeshPath _path = myNavAgent.path;

        List<Vector3> canPos = new List<Vector3>();
        canPos.Add(transform.position);

        List<Vector3> cantPos = new List<Vector3>();

        float currentDistance = 0;
        for (var i = 1; i < _path.corners.Length; i++)
        {
            if (currentDistance > distance)
            {
                cantPos.Add(_path.corners[i]);
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

                    cantPos.Add(targetPosition);
                    cantPos.Add(_path.corners[i]);

                    currentDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
                }
            }
        }

        lineDeplacement.positionCount = canPos.Count;
        lineDeplacement.SetPositions(canPos.ToArray());

        lineCantDeplacement.positionCount = cantPos.Count;
        lineCantDeplacement.SetPositions(cantPos.ToArray());
    }
}
