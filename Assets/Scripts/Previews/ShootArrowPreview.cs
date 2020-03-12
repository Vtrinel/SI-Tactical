using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrowPreview : MonoBehaviour
{
    public LineRenderer line;

    public Transform startPoint;
    public Transform endPoint;

    public Transform Arrow;

    public void SetPositions(List<Vector3> trajectoryPoints)
    {
        Vector3 selfPos = trajectoryPoints[0];
        transform.position = selfPos;
        Arrow.position = trajectoryPoints[trajectoryPoints.Count - 1];

        var lookPos = Arrow.position - trajectoryPoints[trajectoryPoints.Count - 2];
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
            Arrow.rotation = Quaternion.LookRotation(lookPos);

        /*List<Vector3> lineCorrector = new List<Vector3>();
        float endPosDistance = Vector3.Distance(trajectoryPoints[0], endPoint.position);

        foreach (Vector3 pos in trajectoryPoints)
        {
            if(Vector3.Distance(Arrow.position, pos) < 1.6f)
            {
                break;
            }
            else
            {
                lineCorrector.Add(pos);
            }
        }*/
        List<Vector3> lineCorrector = trajectoryPoints;
        List<int> positionsIndexToRemove = new List<int>();
        float endPosDistance = Vector3.Distance(trajectoryPoints[trajectoryPoints.Count - 1], endPoint.position);
        float distanceCounter = 0;
        for(int i = trajectoryPoints.Count - 1; i > 0; i--)
        {
            positionsIndexToRemove.Add(i);
            distanceCounter += Vector3.Distance(trajectoryPoints[i], trajectoryPoints[i - 1]);
            if (distanceCounter > endPosDistance)
                break;
        }

        foreach(int index in positionsIndexToRemove)
        {
            lineCorrector.RemoveAt(index);
        }

        lineCorrector.Add(endPoint.position);

        line.positionCount = lineCorrector.Count;
        line.SetPositions(lineCorrector.ToArray());
    }
}
