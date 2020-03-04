﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrowPreview : MonoBehaviour
{
    public LineRenderer leftLine;
    public LineRenderer rightLine;

    public Transform startLeft;
    public Transform startRight;

    public Transform endLeft;
    public Transform endRight;

    public Transform Arrow;

    private void Start()
    {
        leftLine.positionCount = 2;
        rightLine.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        RefreshArrow();
    }

    void RefreshArrow()
    {
        leftLine.SetPosition(0, startLeft.position);
        leftLine.SetPosition(1, endLeft.position);

        rightLine.SetPosition(0, startRight.position);
        rightLine.SetPosition(1, endRight.position);

        //Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        Arrow.position = GameManager.Instance.GetCurrentWorldMouseResult.mouseWorldPosition;
    }
}
