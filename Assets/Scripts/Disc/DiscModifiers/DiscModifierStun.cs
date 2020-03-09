using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun Modifier", menuName = "Disc Modifiers/Stun")]
public class DiscModifierStun : DiscModifier
{
    [SerializeField] int numberOfStunedTurns = 1;
    public int GetNumberOfStunedTurns => numberOfStunedTurns;
}
