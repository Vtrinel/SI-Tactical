using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competence : ScriptableObject
{
    [Header("Common")]
    [SerializeField] string description = null;

    [SerializeField] Sprite image = null;

    [SerializeField] int actionPointsCost = 1;


    [SerializeField] bool unlocked = false;

    public int GetActionPointsCost => actionPointsCost;
    public Sprite GetImage=> image;
    public string Getdescription => description;
    public bool GetUnlockedState => unlocked;
    public bool SetUnlockedState(bool state) => unlocked = state;
}
