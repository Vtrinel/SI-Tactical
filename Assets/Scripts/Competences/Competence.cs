using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competence : ScriptableObject
{
    [Header("Common")]
    [SerializeField] int apCost = 1;
    public int GetAPCost => apCost;
}
