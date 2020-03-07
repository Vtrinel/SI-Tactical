using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Discs Link Trajectory", menuName = "Competence/Trajectory Modifiers/Discs Link")]
public class TrajectoryModifierLinkedDiscs : TrajectoryModifier
{
    [SerializeField] DiscsOrder linkedDiscTrajectoryType = DiscsOrder.FromOldestToNewest;
    public DiscsOrder GetLinkedDiscTrajectoryType => linkedDiscTrajectoryType;
}
