using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrowPreview : MonoBehaviour
{
    public LineRenderer line;

    public Transform startPoint;
    public Transform endPoint;

    public Transform Arrow;

    private void Start()
    {
        line.positionCount = 2;
    }

    public void SetPositions(List<Vector3> trajectoryPoints)
    {
        Vector3 selfPos = trajectoryPoints[0];
        transform.position = selfPos;
        Arrow.position = trajectoryPoints[trajectoryPoints.Count - 1];

        line.positionCount = trajectoryPoints.Count;
        line.SetPositions(trajectoryPoints.ToArray());

        var lookPos = Arrow.position - trajectoryPoints[trajectoryPoints.Count - 2];
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
            Arrow.rotation = Quaternion.LookRotation(lookPos);

        Vector3 closerPoint = Vector3.zero;
        float closerDistance = Mathf.Infinity;

        foreach(Vector3 pos in trajectoryPoints)
        {
            //if(Vector3.Distance(pos, tra))
        }
    }
}
