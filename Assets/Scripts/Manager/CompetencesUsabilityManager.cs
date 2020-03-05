using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompetencesUsabilityManager
{
    public void SetUp(PlayerController player)
    {
        _player = player;
    }

    public void UpdateSystem()
    {
        if(IsPreparingCompetence)
        {
            UpdatePreparation();
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

    [SerializeField] CompetenceSpecial specialCompetence = default;
    public CompetenceSpecial GetSpecialCompetence => specialCompetence;

    float maxDiscRange = 0;

    public void UpdateSet(CompetenceThrow throwComp, CompetenceRecall recallComp, CompetenceSpecial specialComp)
    {
        throwCompetence = throwComp;
        recallCompetence = recallComp;
        specialCompetence = specialComp;
    }

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
        if (usabilityState == UsabilityState.Preparing)
            maxDiscRange = DiscManager.Instance.rangeOfPlayer;

        if (currentUsabilityState == UsabilityState.Preparing)
            EndPreparation();

        currentUsabilityState = usabilityState;
        currentCompetenceType = compType;

        if (currentUsabilityState == UsabilityState.Preparing)
            StartPreparation();

        /*if (usabilityState == UsabilityState.Preparing && compType == ActionType.Recall)
            PreviewCompetencesManager.Instance.StartRecallPreview(_player.transform.position);
        else
            PreviewCompetencesManager.Instance.EndRecallPreview();*/

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

    #region Preparation
    public void StartPreparation()
    {
        switch (currentCompetenceType)
        {
            case ActionType.Throw:
                StartThrowPreparation();
                break;
            case ActionType.Recall:
                StartRecallPreparation();
                break;
            case ActionType.Special:
                break;
        }
    }

    public void UpdatePreparation()
    {
        switch (currentCompetenceType)
        {
            case ActionType.Throw:
                UpdateThrowPreparation();
                break;
            case ActionType.Recall:
                UpdateRecallPreparation();
                break;
            case ActionType.Special:
                break;
        }
    }

    public void EndPreparation()
    {
        switch (currentCompetenceType)
        {
            case ActionType.Throw:
                EndThrowPreparation();
                break;
            case ActionType.Recall:
                EndRecallPreparation();
                break;
            case ActionType.Special:
                break;
        }
    }

    #region Throw Preparation
    public void StartThrowPreparation()
    {
        Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition);
        PreviewCompetencesManager.Instance.StartThrowPreview(_player.transform.position, trueTargetPosition);
    }

    public void UpdateThrowPreparation()
    {
        Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition);
        PreviewCompetencesManager.Instance.UpdateThrowPreview(_player.transform.position, trueTargetPosition);
    }

    public void EndThrowPreparation()
    {
        PreviewCompetencesManager.Instance.EndThrowPreview();
    }
    #endregion

    #region Recall Preparation
    public void StartRecallPreparation()
    {
        PreviewCompetencesManager.Instance.StartRecallPreview(_player.transform.position);
    }

    public void UpdateRecallPreparation()
    {
    }

    public void EndRecallPreparation()
    {
        PreviewCompetencesManager.Instance.EndRecallPreview();
    }
    #endregion

    #endregion

    #region Throw
    public Vector3 GetInRangeThrowTargetPosition(Vector3 baseTargetPos)
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 trueTargetPos = baseTargetPos;
        trueTargetPos.y = playerPos.y;

        float distance = Vector3.Distance(trueTargetPos, playerPos);
        if(distance > maxDiscRange)
        {
            Vector3 throwDirection = (trueTargetPos - playerPos).normalized;
            trueTargetPos = playerPos + throwDirection * maxDiscRange;
        }

        return trueTargetPos;
    }

    public void LaunchThrowCompetence()
    {
        CompetanceRequestInfo newCompetenceRequestInfo = new CompetanceRequestInfo();
        newCompetenceRequestInfo.startTransform = _player.transform;
        newCompetenceRequestInfo.startPosition = _player.transform.position + Vector3.up * DiscManager.crystalHeight;
        newCompetenceRequestInfo.targetPosition = GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition) + Vector3.up * DiscManager.crystalHeight;

        //Debug.Log("Throw knife at position " + newCompetenceRequestInfo.targetPosition);
        Debug.Log("Using throw competence : " + throwCompetence.GetCompetenceName);

        GameObject newCrystal = DiscManager.Instance.GetCrystal();
        newCrystal.GetComponent<DiscScript>().AttackHere(newCompetenceRequestInfo.startTransform, newCompetenceRequestInfo.targetPosition);
        newCrystal.SetActive(true);

        ResetUsabilityState();
    }
    #endregion

    public void LaunchRecallCompetence()
    {
        CompetanceRequestInfo newCompetenceRequestInfo = new CompetanceRequestInfo();
        newCompetenceRequestInfo.targetTransform = _player.transform;
        newCompetenceRequestInfo.targetPosition = _player.transform.position;

        //Debug.Log("Recall knife at position " + newCompetenceRequestInfo.targetPosition);
        Debug.Log("Using recall competence : " + recallCompetence.GetCompetenceName);

        foreach (DiscScript disc in DiscManager.Instance.GetAllCrystalUse())
        {
            disc.RecallCrystal(newCompetenceRequestInfo.targetTransform);
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
