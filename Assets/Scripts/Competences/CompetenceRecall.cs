using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recall Competence", menuName = "Competence/Recall/Base")]
public class CompetenceRecall : CompetenceDisc
{
    [Header("Recall Parameters")]
    [Tooltip("0 will recall all in-range discs")] [SerializeField] int numberOfRecalledDiscs = 0;
    public int GetNumberOfRecalledDiscs => numberOfRecalledDiscs;

    [SerializeField] DiscsOrder recallingOrder = DiscsOrder.FromOldestToNewest;
    public DiscsOrder GetRecallingOrder => recallingOrder;

    [SerializeField] bool canRecallUnthrowedDiscs = true;
    public bool GetCanRecallUnthrowedDiscs => canRecallUnthrowedDiscs;
}

