using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competence : ScriptableObject
{
    [Header("Display")]
    [SerializeField] string competenceName = null;
    [SerializeField] string competenceDescription = null;

    [SerializeField] Sprite competenceImage = null;

    [Header("Common Parameters")]
    [SerializeField] int actionPointsCost = 1;


    [SerializeField] bool unlocked = false;

    public int GetActionPointsCost => actionPointsCost;
    public Sprite GetCompetenceImage=> competenceImage;
    public string GetCompetenceDescription => competenceDescription;
    public string GetCompetenceName => competenceName;
    public bool GetUnlockedState => unlocked;
    public bool SetUnlockedState(bool state) => unlocked = state;
}
