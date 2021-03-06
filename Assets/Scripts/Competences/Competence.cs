﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competence : ScriptableObject
{
    [Header("Display")]
    [SerializeField] string competenceName = null;
    [SerializeField, TextArea(4,8)] string competenceDescription = null;
    [SerializeField, TextArea(4,8)] string competenceTooltip = null;

    [SerializeField] Sprite competenceImage = null;
    [SerializeField] Sprite competenceTreeImage = null;

    [Header("Common Parameters")]
    [SerializeField] int actionPointsCost = 1;

    [Header("Competence parameters")]
    [SerializeField] bool unlocked = false;
    [SerializeField] List<Competence> competencesUnlockedNeeded = new List<Competence>();

    public int GetActionPointsCost => actionPointsCost;
    public Sprite GetCompetenceImage=> competenceImage;
    public Sprite GetCompetenceTreeImage=> competenceTreeImage;
    public string GetCompetenceName => competenceName;
    public string GetCompetenceDescription => competenceDescription;
    public string GetCompetenceTooltip => competenceTooltip;
    public bool GetUnlockedState => unlocked;
    public bool CanUnlockCompetence()
    {
        // Need to have parent competence
        if (competencesUnlockedNeeded.Count > 0)
        {
            for (int i = 0; i < competencesUnlockedNeeded.Count; i++)
            {
                if (!competencesUnlockedNeeded[i].GetUnlockedState)
                {
                    Debug.Log("Can't unlock because a parent is still locked");
                    return false;
                }
            }
        }

        unlocked = true;
        return true;
    }
}
