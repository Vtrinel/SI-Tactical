using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLinePreview : MonoBehaviour
{
    [SerializeField] LineRenderer line = default;
    [SerializeField] float lineHeight = 0.05f;
    [SerializeField] GameObject linePointPrefab = default;
    [SerializeField] int baseNumberOfLinePoints = 5;
    List<GameObject> linePoints = new List<GameObject>();

    private void Awake()
    {
        linePoints = new List<GameObject>();

        for (int i = 0; i < baseNumberOfLinePoints; i++)
        {
            GameObject newLinePoint = Instantiate(linePointPrefab, transform);
            newLinePoint.SetActive(false);
            linePoints.Add(newLinePoint);
        }
    }

    public void UpdateLine(List<Vector3> trajectory, List<float> distances, int pointsToShow, bool reachedMax)
    {
        List<Vector3> rightYTrajectory = new List<Vector3>();

        float currentDistance = 0;
        int distanceCounter = 0;
        Vector3 lastPlacedPointPosition = Vector3.zero;
        float remainingDistanceBeforeNextPoint = distances[distanceCounter];


        Vector3 newCorrectPosition = new Vector3(trajectory[0].x, lineHeight, trajectory[0].z);
        rightYTrajectory.Add(newCorrectPosition);
        for (int i = 1; i < trajectory.Count; i++)
        {
            Vector3 pos = trajectory[i];

            newCorrectPosition = new Vector3(pos.x, lineHeight, pos.z);
            lastPlacedPointPosition = rightYTrajectory[i - 1];

            rightYTrajectory.Add(newCorrectPosition);

            Vector3 movement = newCorrectPosition - rightYTrajectory[i - 1];
            float distance = movement.magnitude;

            int aled = 0;
            while(distance > remainingDistanceBeforeNextPoint)
            {
                distance -= remainingDistanceBeforeNextPoint;

                Vector3 newPos = lastPlacedPointPosition + movement.normalized * remainingDistanceBeforeNextPoint;
                linePoints[distanceCounter].SetActive(true);
                linePoints[distanceCounter].transform.position = newPos;
                lastPlacedPointPosition = newPos;

                distanceCounter++;
                if (distanceCounter < distances.Count)
                {
                    remainingDistanceBeforeNextPoint = distances[distanceCounter] - distances[distanceCounter - 1];
                }
                else
                {
                    break;
                }

            }
            remainingDistanceBeforeNextPoint -= distance;
        }

        if(reachedMax && (distanceCounter != pointsToShow))
        {
            linePoints[distanceCounter].SetActive(true);
            linePoints[distanceCounter].transform.position = rightYTrajectory[rightYTrajectory.Count - 1];
            distanceCounter++;
        }

        for (int i = distanceCounter; i < linePoints.Count; i++)
        {
            linePoints[i].SetActive(false);
        }

       Vector3 startPosition = rightYTrajectory[0];
        Vector3 targetPos = rightYTrajectory[rightYTrajectory.Count - 1];

        Vector3 direction = (targetPos - startPosition).normalized;

        line.positionCount = rightYTrajectory.Count;
        line.SetPositions(rightYTrajectory.ToArray());
    }

    public void ShowPreview()
    {
        gameObject.SetActive(true);
    }

    public void HidePreview()
    {
        gameObject.SetActive(false);
    }
}
