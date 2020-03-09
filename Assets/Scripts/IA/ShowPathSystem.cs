using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShowPathSystem : MonoBehaviour
{
    public LineRenderer lineDeplacement;
    public LineRenderer lineCantDeplacement;

    public void DrawPath(NavMeshAgent myNavAgent, float _distance)
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
            if (currentDistance > _distance)
            {
                cantPos.Add(_path.corners[i]);
            }
            else
            {
                if (currentDistance + Vector3.Distance(_path.corners[i - 1], _path.corners[i]) < _distance)
                {
                    canPos.Add(_path.corners[i]);
                    currentDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
                }
                else
                {
                    //Calcule de coupure de la ligne
                    float distanceTokeep = _distance - currentDistance;
                    print(distanceTokeep);

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
