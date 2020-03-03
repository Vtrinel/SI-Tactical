﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCompetenceSystem
{
    public System.Action<Competence> OnCompetenceUsed = default;

    [SerializeField] CompetenceThrow throwCompetence = default;
    public CompetenceThrow GetCompetenceThrow => throwCompetence;

    [SerializeField] CompetenceRecall recallCompetence = default;
    public CompetenceRecall GetRecallCompetence => recallCompetence;

    CompetenceUsabilityState currentUsabilityState = CompetenceUsabilityState.None;
    public CompetenceUsabilityState GetCurrentUsabilityState => currentUsabilityState;
    CompetenceType currentCompetenceType = CompetenceType.None;
    public CompetenceType GetCurrentCompetenceType => currentCompetenceType;

    public int GetCompetenceActionPointsCost(CompetenceType compType)
    {
        switch (compType)
        {
            case CompetenceType.None:
                return 0;
            case CompetenceType.Throw:
                return throwCompetence.GetActionPointsCost;
            case CompetenceType.Recall:
                return recallCompetence.GetActionPointsCost;
        }
        return 0;
    }
    public ActionSelectionResult HasEnoughActionPoints(int totalActionPoints, CompetenceType compType)
    {
        Competence competenceToCheck = null;

        switch (compType)
        {
            case CompetenceType.Throw:
                competenceToCheck = throwCompetence;
                break;
            case CompetenceType.Recall:
                competenceToCheck = recallCompetence;
                break;
        }

        if (competenceToCheck == null)
            return ActionSelectionResult.NoCompetenceFound;

        return totalActionPoints >= competenceToCheck.GetActionPointsCost ? ActionSelectionResult.EnoughAactionPoints : ActionSelectionResult.NotEnoughActionPoints;
    }

    public void ChangeUsabilityState(CompetenceUsabilityState usabilityState, CompetenceType compType)
    {
        currentUsabilityState = usabilityState;
        currentCompetenceType = compType;
    }

    public void ResetUsabilityState()
    {
        ChangeUsabilityState(CompetenceUsabilityState.None, CompetenceType.None);
    }

    public bool IsUsingCompetenceSystem => currentUsabilityState != CompetenceUsabilityState.None;

    public void InterruptPreparation()
    {
        ResetUsabilityState();
    }

    public void LaunchThrowCompetence(CompetanceRequestInfo newCompetanceRequestInfo)
    {
        Debug.Log("Throw knife at position " + newCompetanceRequestInfo.targetPosition);

        GameObject newCrystal = CrystalManager.Instance.GetCrystal();
        newCrystal.GetComponent<CrystalScript>().AttackHere(newCompetanceRequestInfo.startTransform, newCompetanceRequestInfo.targetPosition);
        newCrystal.SetActive(true);

        OnCompetenceUsed(throwCompetence);

        ResetUsabilityState();
    }

    public void LaunchRecallCompetence(CompetanceRequestInfo newCompetanceRequestInfo)
    {
        Debug.Log("Recall knife at position " + newCompetanceRequestInfo.targetPosition);

        foreach (GameObject crystal in CrystalManager.Instance.GetAllCrystalUse())
        {
            crystal.GetComponent<CrystalScript>().RecallCrystal(newCompetanceRequestInfo.targetTransform);
        }

        OnCompetenceUsed(recallCompetence);

        ResetUsabilityState();
    }
}

public enum CompetenceUsabilityState
{
    None, Preparing, Using
}

public enum CompetenceType
{
    None, Throw, Recall
}

public struct CompetanceRequestInfo
{
    public Transform startTransform;
    public Transform targetTransform;

    public Vector3 startPosition;
    public Vector3 targetPosition;
}