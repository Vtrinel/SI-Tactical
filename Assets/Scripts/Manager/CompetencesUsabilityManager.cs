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

    int currentActionPoints = 0;
    public void UpdateSystem()
    {
        if(IsPreparingCompetence)
        {
            UpdatePreparation();
            UIManager.Instance.UpdateActionPointCostText(GetCurrentCompetenceCost(), currentActionPoints);
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
    [SerializeField] float minThrowDistance = 1f;

    [SerializeField] CompetenceRecall recallCompetence = default;
    public CompetenceRecall GetRecallCompetence => recallCompetence;
    public Action<CompetenceRecall> OnRecallCompetenceChanged;

    [SerializeField] CompetenceSpecial specialCompetence = default;
    public CompetenceSpecial GetSpecialCompetence => specialCompetence;

    float maxRecallRange = 0;
    float maxThrowRange = 0;

    //Action Event
    public Action OnDiscThrownAnimEvent;
    public Action OnDiscRecallAnimEvent;
    public Action OnSpecialLaunch;

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
                case ActionType.Special:
                    return specialCompetence;
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
            else if (compType == ActionType.Special)
            {
                CompetenceSpecialTeleportation foundTeleportationCompetence = specialCompetence as CompetenceSpecialTeleportation;
                if(foundTeleportationCompetence != null)
                {
                    if(foundTeleportationCompetence.GetTeleportationMode == TeleportationMode.Exchange)
                    {
                        if(foundTeleportationCompetence.GetTeleportationTarget == TeleportationTarget.NewestDisc || foundTeleportationCompetence.GetTeleportationTarget == TeleportationTarget.OldestDisc)
                        {
                            if (DiscManager.Instance.GetInRangeDiscsCount == 0)
                            {
                                trySelectResult = ActionSelectionResult.NoNearbyDisc;
                            }
                        }
                    }
                }
            }
            currentActionPoints = totalActionPoints;
        }

        switch (trySelectResult)
        {
            case ActionSelectionResult.EnoughActionPoints:
                ChangeUsabilityState(UsabilityState.Preparing, compType);
                UIManager.Instance.GetActionBar.UpdatePreConsommationPointBar(totalActionPoints, GetCurrentCompetenceCost());
                UIManager.Instance.ShowActionPointsCostText();
                UIManager.Instance.UpdateActionPointCostText(GetCurrentCompetenceCost(), totalActionPoints);
                break;

            case ActionSelectionResult.NotEnoughActionPoints:
                Debug.Log("Not enough action points for " + compType);
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
            case ActionType.Special:
                return specialCompetence.GetActionPointsCost;
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
            case ActionType.Special:
                competenceToCheck = specialCompetence;
                break;
        }

        if (competenceToCheck == null)
            return ActionSelectionResult.NoCompetenceFound;

        return totalActionPoints >= competenceToCheck.GetActionPointsCost ? ActionSelectionResult.EnoughActionPoints : ActionSelectionResult.NotEnoughActionPoints;
    }

    public void ChangeUsabilityState(UsabilityState usabilityState, ActionType compType)
    {
        if (usabilityState == UsabilityState.Preparing)
        {
            maxRecallRange = DiscManager.Instance.recallRange;
            maxThrowRange = DiscManager.Instance.throwRange;
        }

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
                StartSpecialPreparation();
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
                UpdateSpecialPreparation();
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
                EndSpecialPreparation();
                break;
        }
    }

    #region Throw Preparation
    public void StartThrowPreparation()
    {
        Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition);
        DiscTrajectoryParameters trajectoryParameters = 
            DiscTrajectoryFactory.GetTrajectory(throwCompetence, _player.transform.position, trueTargetPosition,
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, null);

        List<DiscTrajectoryParameters> trajectories = new List<DiscTrajectoryParameters> { trajectoryParameters };
        PreviewCompetencesManager.Instance.StartThrowPreview(trajectories, _player.transform.position);
        PreviewCompetencesManager.Instance.UpdateThrowPreview(new List<DiscTrajectoryParameters> { trajectoryParameters });
    }

    public void UpdateThrowPreparation()
    {
        Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition);
        DiscTrajectoryParameters trajectoryParameters = 
            DiscTrajectoryFactory.GetTrajectory(throwCompetence, _player.transform.position, trueTargetPosition,
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, null);

        List<DiscTrajectoryParameters> trajectories = new List<DiscTrajectoryParameters> { trajectoryParameters };
        PreviewCompetencesManager.Instance.UpdateThrowPreview(trajectories);
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

        List<DiscScript> recallableDiscs = DiscListingFactory.GetSortedRecallableDiscs(recallCompetence, DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);
        foreach(DiscScript discInRange in recallableDiscs)
        {
            DiscTrajectoryParameters newParams = 
                DiscTrajectoryFactory.GetTrajectory(recallCompetence, discInRange.transform.position, playerPos,
                DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, discInRange);
            recallTrajectoryParameters.Add(newParams);
        }

        PreviewCompetencesManager.Instance.StartRecallPreview(recallTrajectoryParameters, playerPos);
        PreviewCompetencesManager.Instance.UpdateRecallPreview(recallTrajectoryParameters, playerPos);
    }

    public void UpdateRecallPreparation()
    {
        Vector3 playerPos = _player.transform.position;
        List<DiscTrajectoryParameters> recallTrajectoryParameters = new List<DiscTrajectoryParameters>();

        List<DiscScript> recallableDiscs = DiscListingFactory.GetSortedRecallableDiscs(recallCompetence, DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);
        foreach (DiscScript discInRange in recallableDiscs)
        {
            DiscTrajectoryParameters newParams =
                DiscTrajectoryFactory.GetTrajectory(recallCompetence, discInRange.transform.position, playerPos,
                DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, discInRange);
            recallTrajectoryParameters.Add(newParams);
        }

        PreviewCompetencesManager.Instance.UpdateRecallPreview(recallTrajectoryParameters, playerPos);
    }

    public void EndRecallPreparation()
    {
        PreviewCompetencesManager.Instance.EndRecallPreview();
    }
    #endregion

    #region Special Preparation
    CompetenceSpecialTeleportation specialCompetenceTeleportTyped = default;

    public void StartSpecialPreparation()
    {
        specialCompetenceTeleportTyped = specialCompetence as CompetenceSpecialTeleportation;
        if (specialCompetenceTeleportTyped != null)
            StartTeleportationPreparation();
    }

    public void UpdateSpecialPreparation()
    {
        if (specialCompetenceTeleportTyped != null)
            UpdateTeleportationPreparation();
    }

    public void EndSpecialPreparation()
    {
        if (specialCompetenceTeleportTyped != null)
            EndTeleportationPreparation();
    }

    #region Teleportation Utilities
    Vector3 currentTeleportationPosition = default;
    GameObject teleportationExchangeObject = default;
    bool canTeleport = false;

    public void StartTeleportationPreparation()
    {
        canTeleport = false;
        switch (specialCompetenceTeleportTyped.GetTeleportationMode)
        {
            case TeleportationMode.Exchange:
                teleportationExchangeObject = GetTeleportationExchangeObject(specialCompetenceTeleportTyped.GetTeleportationTarget);
                currentTeleportationPosition = teleportationExchangeObject.transform.position;
                canTeleport = true;
                break;
            case TeleportationMode.TowardDirection:
                currentTeleportationPosition = GetTowardDirectionTeleportationPosition();
                break;
        }

        PreviewCompetencesManager.Instance.StartTeleportationPreview(_player.transform.position, currentTeleportationPosition, canTeleport, recallCompetence);
    }

    public void UpdateTeleportationPreparation()
    {
        switch (specialCompetenceTeleportTyped.GetTeleportationMode)
        {
            case TeleportationMode.TowardDirection:
                currentTeleportationPosition = GetTowardDirectionTeleportationPosition();
                break;
        }

        PreviewCompetencesManager.Instance.UpdateTeleportationPreview(currentTeleportationPosition, canTeleport, recallCompetence);
    }

    public void EndTeleportationPreparation()
    {
        PreviewCompetencesManager.Instance.EndTeleportationPreview();
        teleportationExchangeObject = null;
        currentTeleportationPosition = Vector3.zero;
    }

    public GameObject GetTeleportationExchangeObject(TeleportationTarget teleportationTarget)
    {
        DiscsOrder order = (teleportationTarget == TeleportationTarget.NewestDisc ? DiscsOrder.FromNewestToOldest :
            teleportationTarget == TeleportationTarget.OldestDisc ? DiscsOrder.FromOldestToNewest : DiscsOrder.FromNewestToOldest);
        List<DiscScript> discs = DiscListingFactory.GetSortedInRangeDiscs(1, order, true, DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);

        if (discs.Count == 0)
            return null;

        return discs[0].gameObject;
    }

    public Vector3 GetTowardDirectionTeleportationPosition()
    {
        Vector3 movement = currentWorldMouseResult.mouseWorldPosition - _player.transform.position;

        if(movement.magnitude > specialCompetenceTeleportTyped.GetTeleportationDistance)
        {
            movement = movement.normalized * specialCompetenceTeleportTyped.GetTeleportationDistance;
        }
        Vector3 targetPos = _player.transform.position + movement;

        LayerMask checkMask = 1 << 10 | 1 << 11 | 1 << 12 | 1 << 14;
        canTeleport = Physics.OverlapSphere(targetPos, 1f, checkMask).Length == 0;

        return targetPos;
    }
    #endregion
    #endregion

    #endregion

    public void LaunchCompetenceForReal()
    {
        switch (currentCompetenceType)
        {
            case ActionType.Throw:
                LaunchThrowCompetenceForReal();
                break;
            case ActionType.Recall:
                LaunchRecallCompetenceForReal();
                break;
            case ActionType.Special:
                break;
        }
    }

    #region Throw
    public Vector3 GetInRangeThrowTargetPosition(Vector3 baseTargetPos)
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 trueTargetPos = baseTargetPos;
        trueTargetPos.y = playerPos.y;

        float distance = Vector3.Distance(trueTargetPos, playerPos);
        if(distance > maxThrowRange)
        {
            Vector3 throwDirection = (trueTargetPos - playerPos).normalized;
            trueTargetPos = playerPos + throwDirection * maxThrowRange;
        }
        else if(distance < minThrowDistance)
        {
            Vector3 throwDirection = (trueTargetPos - playerPos).normalized;
            trueTargetPos = playerPos + throwDirection * minThrowDistance;
        }

        return trueTargetPos;
    }

    GameObject currentObjLauncher = default;
    Vector3 currentThrowPosition = default;
    public void LaunchThrowCompetence(GameObject objLauncher)
    {
        OnDiscThrownAnimEvent?.Invoke(); //Event
        ChangeUsabilityState(UsabilityState.Using, ActionType.Throw);
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
        currentObjLauncher = objLauncher;
        currentThrowPosition = currentWorldMouseResult.mouseWorldPosition;
    }

    public void LaunchThrowCompetenceForReal()
    {
        currentlyInUseDiscs = new List<DiscScript>();

        DiscScript newDisc = DiscManager.Instance.TakeFirstDiscFromPossessedDiscs();
        if (newDisc == null)
        {
            //Debug.LogWarning("NO DISK TO THROW");
            return;
        }

        DiscTrajectoryParameters trajectoryParameters =
            DiscTrajectoryFactory.GetTrajectory(throwCompetence,
            _player.transform.position, GetInRangeThrowTargetPosition(currentThrowPosition),
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, newDisc);

        newDisc.SetIsBeingRecalled(false);
        newDisc.SetRetreivableByPlayer(false);
        newDisc.StartTrajectory(trajectoryParameters, currentObjLauncher);
        currentlyInUseDiscs.Add(newDisc);
        newDisc.OnTrajectoryStopped += RemoveDiscFromInUse;
        newDisc.OnReachedTrajectoryEnd += RemoveDiscFromInUse;
    }
    #endregion

    #region Recall
    public void LaunchRecallCompetence()
    {
        OnDiscRecallAnimEvent?.Invoke(); //Event
        ChangeUsabilityState(UsabilityState.Using, ActionType.Recall);
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();

        /*List<DiscScript> discsToRecall = DiscListingFactory.GetSortedRecallableDiscs(recallCompetence, DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);

        foreach(DiscScript discToRecall in discsToRecall)
        {
            StartRecallDisc(discToRecall);
        }        

        if (discsToRecall.Count > 0)
            ChangeUsabilityState(UsabilityState.Using, ActionType.Recall);
        else
            ResetUsabilityState();
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();*/

    }
    public void LaunchRecallCompetenceForReal()
    {
        currentlyInUseDiscs = new List<DiscScript>();
        List<DiscScript> discsToRecall = DiscListingFactory.GetSortedRecallableDiscs(recallCompetence, DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);

        foreach (DiscScript discToRecall in discsToRecall)
        {
            StartRecallDisc(discToRecall);
        }

        if (discsToRecall.Count > 0)
            ChangeUsabilityState(UsabilityState.Using, ActionType.Recall);
        else
            ResetUsabilityState();
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
    }

    public void StartRecallDisc(DiscScript disc)
    {
        DiscTrajectoryParameters trajectoryParameters =
            DiscTrajectoryFactory.GetTrajectory(recallCompetence,
            disc.transform.position, _player.transform.position,
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, disc);

        disc.SetIsBeingRecalled(true);
        disc.StartTrajectory(trajectoryParameters, null);
        currentlyInUseDiscs.Add(disc);
        disc.OnTrajectoryStopped += RemoveDiscFromInUse;

        OnSpecialLaunch?.Invoke();
    }
    #endregion

    #region Special
    public bool LaunchSpecialCompetence()
    {
        if(specialCompetenceTeleportTyped != null)
        {
            return LaunchTeleportation();
        }
        return false;
    }

    public bool LaunchTeleportation()
    {
        if (!canTeleport)
            return false;

        Vector3 initialPlayerPos = _player.transform.position;

        _player.transform.position = currentTeleportationPosition;
        if(teleportationExchangeObject != null)
        {
            teleportationExchangeObject.transform.position = initialPlayerPos;
        }

        ResetUsabilityState();
        CameraManager.instance.GetPlayerCamera.transform.position = initialPlayerPos;
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();

        return true;
    }
    #endregion

    List<DiscScript> currentlyInUseDiscs = new List<DiscScript>();
    public void RemoveDiscFromInUse(DiscScript disc)
    {
        currentlyInUseDiscs.Remove(disc);

        if (currentlyInUseDiscs.Count == 0)
            EndCompetenceUsability();
    }

    public void EndCompetenceUsability()
    {
        ResetUsabilityState();
    }
}
