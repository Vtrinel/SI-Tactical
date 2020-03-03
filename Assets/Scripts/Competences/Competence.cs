using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competence : ScriptableObject
{
    [Header("Common")]
    [SerializeField] int actionPointsCost = 1;
    public int GetActionPointsCost => actionPointsCost;
}
