using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Teleportation Competence", menuName = "Competence/Special/Teleport")]
public class CompetenceSpecialTeleportation : CompetenceSpecial
{
    [Header("Teleportation Parameters")]
    [SerializeField] TeleportationMode teleportationType = TeleportationMode.Exchange;
    public TeleportationMode GetTeleportationMode => teleportationType;
    [SerializeField] TeleportationTarget teleportationTarget = TeleportationTarget.NewestDisc;
    public TeleportationTarget GetTeleportationTarget => teleportationTarget;
    [SerializeField] int teleportationDistance = 0;
    public int GetTeleportationDistance => teleportationDistance;
}

public enum TeleportationMode
{
    None, TowardDirection, Exchange
}

public enum TeleportationTarget
{
    None, OldestDisc, NewestDisc
}

