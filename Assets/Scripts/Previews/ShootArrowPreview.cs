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
        transform.position = trajectoryPoints[0];
        Arrow.position = trajectoryPoints[1];
        line.SetPosition(0, startPoint.position);
        line.SetPosition(1, endPoint.position);

        var lookPos = Arrow.position - startPoint.position;
        lookPos.y = 0;
        Arrow.rotation = Quaternion.LookRotation(lookPos);
    }
}
