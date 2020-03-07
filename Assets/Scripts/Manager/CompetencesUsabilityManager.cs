using System;
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
    public Action<CompetenceRecall> OnRecallCompetenceChanged;

    [SerializeField] CompetenceSpecial specialCompetence = default;
    public CompetenceSpecial GetSpecialCompetence => specialCompetence;

    float maxDiscRange = 0;

    public void UpdateSet(CompetenceThrow throwComp, CompetenceRecall recallComp, CompetenceSpecial specialComp)
    {
        throwCompetence = throwComp;
        recallCompetence = recallComp;
        specialCompetence = specialComp;

        OnRecallCompetenceChanged?.Invoke(recallCompetence);
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
        ActionSelectionResult trySelectResult = HasEnoughActionPoints(totalActionPoints, compType);

        if (trySelectResult == ActionSelectionResult.EnoughActionPoints)
        {
            if (compType == ActionType.Throw)
            {
                if (DiscManager.Instance.GetPossessedDiscsCount == 0)
                {
                    trySelectResult = ActionSelectionResult.NotEnoughDiscs;
                }
            }
            else if (compType == ActionType.Recall)
            {
                if (DiscManager.Instance.GetInRangeDiscsCount == 0)
                {
                    trySelectResult = ActionSelectionResult.NoNearbyDisc;
                }
            }

        }

        switch (trySelectResult)
        {
            case ActionSelectionResult.EnoughActionPoints:
                ChangeUsabilityState(UsabilityState.Preparing, compType);
                break;

            case ActionSelectionResult.NotEnoughActionPoints:
                //Debug.Log("Not enough action points for " + compType);
                break;

            case ActionSelectionResult.NoCompetenceFound:
                //Debug.LogWarning("WARNING : " + compType + " not found.");
                break;

            case ActionSelectionResult.NotEnoughDiscs:
                //Debug.Log("Not enough possessed discs for " + compType);
                break;

            case ActionSelectionResult.NoNearbyDisc:
                //Debug.Log("Not nearby discs for " + compType);
                break;
        }

        return trySelectResult;
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
        DiscTrajectoryParameters trajectoryParameters = 
            DiscTrajectoryFactory.GetThrowTrajectory(throwCompetence, _player.transform.position, trueTargetPosition,
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);
        PreviewCompetencesManager.Instance.StartThrowPreview(new List<DiscTrajectoryParameters> { trajectoryParameters }, _player.transform.position);
    }

    public void UpdateThrowPreparation()
    {
        Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition);
        DiscTrajectoryParameters trajectoryParameters = 
            DiscTrajectoryFactory.GetThrowTrajectory(throwCompetence, _player.transform.position, trueTargetPosition,
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);
        PreviewCompetencesManager.Instance.UpdateThrowPreview(new List<DiscTrajectoryParameters> { trajectoryParameters });
    }

    public void EndThrowPreparation()
    {
        PreviewCompetencesManager.Instance.EndThrowPreview();
    }
    #endregion

    #region Recall Preparation
    public void StartRecallPreparation()
    {
        Vector3 playerPos = _player.transform.position;
        List<DiscTrajectoryParameters> recallTrajectoryParameters = new List<DiscTrajectoryParameters>();

        foreach(DiscScript discInRange in DiscManager.Instance.GetInRangeDiscs)
        {
            DiscTrajectoryParameters newParams = 
                DiscTrajectoryFactory.GetRecallTrajectory(recallCompetence, discInRange.transform.position, playerPos,
                DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);
            recallTrajectoryParameters.Add(newParams);
        }

        PreviewCompetencesManager.Instance.StartRecallPreview(recallTrajectoryParameters, playerPos);
    }

    public void UpdateRecallPreparation()
    {
        Vector3 playerPos = _player.transform.position;
        List<DiscTrajectoryParameters> recallTrajectoryParameters = new List<DiscTrajectoryParameters>();

        foreach (DiscScript discInRange in DiscManager.Instance.GetInRangeDiscs)
        {
            DiscTrajectoryParameters newParams = 
                DiscTrajectoryFactory.GetRecallTrajectory(recallCompetence, discInRange.transform.position, playerPos,
                DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);
            recallTrajectoryParameters.Add(newParams);
        }

        PreviewCompetencesManager.Instance.UpdateRecallPreview(recallTrajectoryParameters, playerPos);
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
        DiscScript newDisc = DiscManager.Instance.TakeFirstDiscFromPossessedDiscs();
        if(newDisc == null)
        {
            Debug.LogWarning("NO DISK TO THROW");
            return;
        }

        DiscTrajectoryParameters trajectoryParameters = 
            DiscTrajectoryFactory.GetThrowTrajectory(throwCompetence,
            _player.transform.position, GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition), 
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);

        newDisc.SetRetreivableByPlayer(false);
        newDisc.StartTrajectory(trajectoryParameters);
        //newDisc.gameObject.SetActive(true);

        ResetUsabilityState();
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
    }
    #endregion

    #region
    public void LaunchRecallCompetence()
    {
        foreach (DiscScript disc in DiscManager.Instance.GetInRangeDiscs)
        {
            DiscTrajectoryParameters trajectoryParameters =
                DiscTrajectoryFactory.GetRecallTrajectory(recallCompetence,
                disc.transform.position, _player.transform.position,
                DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);

            disc.StartTrajectory(trajectoryParameters);
        }

        ResetUsabilityState();
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
    }
    #endregion
}
