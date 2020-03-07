using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceDisc : Competence
{
    [Header("Disc Competence Parameters")]
    [SerializeField] List<TrajectoryModifier> trajectoryModifiers = new List<TrajectoryModifier>();
    public List<TrajectoryModifier> GetTrajectoryModifiers => trajectoryModifiers;
}

public enum DiscsOrder
{
    FromOldestToNewest, FromNewestToOldest, None
}