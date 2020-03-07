using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Discs Link Trajectory", menuName = "Competence/Trajectory Modifiers/Discs Link")]
public class TrajectoryModifierLinkedDiscs : TrajectoryModifier
{
    [SerializeField] LinkedDiscTrajectoryType linkedDiscTrajectoryType = LinkedDiscTrajectoryType.FromOldestToNewest;
    public LinkedDiscTrajectoryType GetLinkedDiscTrajectoryType => linkedDiscTrajectoryType;
}

public enum LinkedDiscTrajectoryType
{
    FromOldestToNewest, FromNewestToOldest, None
}
