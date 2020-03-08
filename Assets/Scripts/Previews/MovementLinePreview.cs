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

    public void UpdateLine(Vector3 startPosition, Vector3 targetPos, List<float> distances, int pointsToShow)
    {
        Vector3 trueStartPos = new Vector3(startPosition.x, lineHeight, startPosition.z);
        Vector3 trueTargetPos = new Vector3(targetPos.x, lineHeight, targetPos.z);
        Vector3 direction = (trueTargetPos - trueStartPos).normalized;

        line.SetPosition(0, trueStartPos);
        line.SetPosition(1, trueTargetPos);

        for(int i = 0; i < pointsToShow; i++)
        {
            linePoints[i].SetActive(true);
            linePoints[i].transform.position = startPosition + direction * distances[i];
        }
        for (int i = pointsToShow; i < linePoints.Count; i++)
        {
            linePoints[i].SetActive(false);
        }
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
