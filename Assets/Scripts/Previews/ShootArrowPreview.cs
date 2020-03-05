using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrowPreview : MonoBehaviour
{
    public LineRenderer line;

    public Transform startPoint;
    public Transform endPoint;

    public Transform Arrow;

    //Transform player;

    private void Start()
    {
        line.positionCount = 2;

        //player = GameManager.Instance.GetPlayer.transform;
    }

    // Update is called once per frame
    /*void Update()
    {
        RefreshArrow();
        transform.position = player.position;
    }

    void RefreshArrow()
    {
        line.SetPosition(0, startPoint.position);
        line.SetPosition(1, endPoint.position);

        //Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        Arrow.position = GameManager.Instance.GetCurrentWorldMouseResult.mouseWorldPosition;
    }*/

    public void SetPositions(List<Vector3> trajectoryPoints)
    {
        // Here is hardcode for line trajectory, trajectoryPoints[0] is start position and trajectoryPoints[1] is target
        // In final state, each trajectory point should be use to preview the real trajectory

        leftLine.SetPosition(0, startLeft.position);
        leftLine.SetPosition(1, endLeft.position);

        rightLine.SetPosition(0, startRight.position);
        rightLine.SetPosition(1, endRight.position);

        transform.position = trajectoryPoints[0];
        Arrow.position = trajectoryPoints[1];
    }
}
