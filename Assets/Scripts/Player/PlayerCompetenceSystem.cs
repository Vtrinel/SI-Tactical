using System.Collections;
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

    UsabilityState currentUsabilityState = UsabilityState.None;
    public UsabilityState GetCurrentUsabilityState => currentUsabilityState;
    ActionType currentCompetenceType = ActionType.None;
    public ActionType GetCurrentCompetenceType => currentCompetenceType;

    public int GetCompetenceActionPointsCost(ActionType compType)
    {
        switch (compType)
        {
            case ActionType.None:
                return 0;
            case ActionType.Throw:
                return throwCompetence.GetActionPointsCost;
            case ActionType.Recall:
                return recallCompetence.GetActionPointsCost;
        }
        return 0;
    }
    public ActionSelectionResult HasEnoughActionPoints(int totalActionPoints, ActionType compType)
    {
        Competence competenceToCheck = null;

        switch (compType)
        {
            case ActionType.Throw:
                competenceToCheck = throwCompetence;
                break;
            case ActionType.Recall:
                competenceToCheck = recallCompetence;
                break;
        }

        if (competenceToCheck == null)
            return ActionSelectionResult.NoCompetenceFound;

        return totalActionPoints >= competenceToCheck.GetActionPointsCost ? ActionSelectionResult.EnoughActionPoints : ActionSelectionResult.NotEnoughActionPoints;
    }

    public void ChangeUsabilityState(UsabilityState usabilityState, ActionType compType)
    {
        currentUsabilityState = usabilityState;
        currentCompetenceType = compType;
    }

    public void ResetUsabilityState()
    {
        ChangeUsabilityState(UsabilityState.None, ActionType.None);
    }

    public bool IsUsingCompetenceSystem => currentUsabilityState != UsabilityState.None;

    public void InterruptPreparation()
    {
        ResetUsabilityState();
    }

    public void LaunchThrowCompetence(CompetanceRequestInfo newCompetanceRequestInfo)
    {
        Debug.Log("Throw knife at position " + newCompetanceRequestInfo.targetPosition);

        GameObject newCrystal = DiscManager.Instance.GetCrystal();
        newCrystal.GetComponent<DiscScript>().AttackHere(newCompetanceRequestInfo.startTransform, newCompetanceRequestInfo.targetPosition);
        newCrystal.SetActive(true);

        OnCompetenceUsed(throwCompetence);

        ResetUsabilityState();
    }

    public void LaunchRecallCompetence(CompetanceRequestInfo newCompetanceRequestInfo)
    {
        Debug.Log("Recall knife at position " + newCompetanceRequestInfo.targetPosition);

        foreach (GameObject crystal in DiscManager.Instance.GetAllCrystalUse())
        {
            crystal.GetComponent<DiscScript>().RecallCrystal(newCompetanceRequestInfo.targetTransform);
        }

        OnCompetenceUsed(recallCompetence);

        ResetUsabilityState();
    }
}

/*public struct CompetanceRequestInfo
{
    public Transform startTransform;
    public Transform targetTransform;

    public Vector3 startPosition;
    public Vector3 targetPosition;
}*/