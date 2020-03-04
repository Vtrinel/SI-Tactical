using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompetencesManager
{
    public void SetUp(PlayerController player)
    {
        _player = player;
    }

    public void UpdateSystem()
    {
        if(IsPreparingCompetence)
        {
            //Debug.Log("Preparing " + currentCompetenceType);
        }
    }

    PlayerController _player = default;
    WorldMouseResult currentWorldMouseResult;
    public void UpdateCurrentWorldMouseResult(WorldMouseResult result)
    {
        currentWorldMouseResult = result;
    }

    public System.Action OnCompetenceStateChanged;

    [Header("Competences")]
    [SerializeField] CompetenceThrow throwCompetence = default;
    public CompetenceThrow GetCompetenceThrow => throwCompetence;

    [SerializeField] CompetenceRecall recallCompetence = default;
    public CompetenceRecall GetRecallCompetence => recallCompetence;

    UsabilityState currentUsabilityState = UsabilityState.None;
    public UsabilityState GetCurrentUsabilityState => currentUsabilityState;
    ActionType currentCompetenceType = ActionType.None;
    public ActionType GetCurrentCompetenceType => currentCompetenceType;
    public Competence GetCurrentCompetence
    {
        get
        {
            switch (currentCompetenceType)
            {                
                case ActionType.Throw:
                    return throwCompetence;
                case ActionType.Recall:
                    return recallCompetence;
                default:
                    return null;
            }
        }
    }

    public ActionSelectionResult TrySelectAction(int totalActionPoints, ActionType compType)
    {
        ActionSelectionResult hasEnoughAPResult = HasEnoughActionPoints(totalActionPoints, compType);

        switch (hasEnoughAPResult)
        {
            case ActionSelectionResult.EnoughActionPoints:
                ChangeUsabilityState(UsabilityState.Preparing, compType);
                break;

            case ActionSelectionResult.NotEnoughActionPoints:
                Debug.Log("Not enough action points for " + compType);
                break;

            case ActionSelectionResult.NoCompetenceFound:
                Debug.LogWarning("WARNING : " + compType + " not found.");
                break;
        }

        return hasEnoughAPResult;
    }

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
    public int GetCurrentCompetenceCost()
    {
        return GetCompetenceActionPointsCost(currentCompetenceType);
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

        OnCompetenceStateChanged?.Invoke();
    }

    public void ResetUsabilityState()
    {
        ChangeUsabilityState(UsabilityState.None, ActionType.None);
    }

    public bool IsUsingCompetenceSystem => currentUsabilityState != UsabilityState.None;
    public bool IsPreparingCompetence => currentUsabilityState == UsabilityState.Preparing;
    public bool IsUsingCompetence => currentUsabilityState == UsabilityState.Using;

    public void InterruptPreparation()
    {
        ResetUsabilityState();
    }

    public void LaunchThrowCompetence()
    {
        CompetanceRequestInfo newCompetenceRequestInfo = new CompetanceRequestInfo();
        newCompetenceRequestInfo.startTransform = _player.transform;
        newCompetenceRequestInfo.startPosition = _player.transform.position + Vector3.up * DiscManager.crystalHeight;
        newCompetenceRequestInfo.targetPosition = currentWorldMouseResult.mouseWorldPosition + Vector3.up * DiscManager.crystalHeight;

        Debug.Log("Throw knife at position " + newCompetenceRequestInfo.targetPosition);

        GameObject newCrystal = DiscManager.Instance.GetCrystal();
        newCrystal.GetComponent<DiscScript>().AttackHere(newCompetenceRequestInfo.startTransform, newCompetenceRequestInfo.targetPosition);
        newCrystal.SetActive(true);

        ResetUsabilityState();
    }

    public void LaunchRecallCompetence()
    {
        CompetanceRequestInfo newCompetenceRequestInfo = new CompetanceRequestInfo();
        newCompetenceRequestInfo.targetTransform = _player.transform;
        newCompetenceRequestInfo.targetPosition = _player.transform.position;

        Debug.Log("Recall knife at position " + newCompetenceRequestInfo.targetPosition);

        foreach (GameObject crystal in DiscManager.Instance.GetAllCrystalUse())
        {
            crystal.GetComponent<DiscScript>().RecallCrystal(newCompetenceRequestInfo.targetTransform);
        }

        ResetUsabilityState();
    }
}

public struct CompetanceRequestInfo
{
    public Transform startTransform;
    public Transform targetTransform;

    public Vector3 startPosition;
    public Vector3 targetPosition;
}
