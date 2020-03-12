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
        else if (IsUsingCompetence)
        {
            if (!teleportationDurationSystem.TimerOver)
            {
                teleportationDurationSystem.UpdateTimer();
            }
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
                List<DiscScript> inRange = DiscManager.Instance.GetInRangeDiscs;
                List<DiscScript> throwed = DiscManager.Instance.GetAllThrowedDiscs;
                List<DiscScript> recallable = DiscListingFactory.GetSortedRecallableDiscs(recallCompetence, throwed, inRange);
                if (recallable.Count == 0)
                {
                    trySelectResult = ActionSelectionResult.NoNearbyDisc;
                }
            }
            else if (compType == ActionType.Special)
            {
                bool usable = GetSpecialCompetenceUsable(totalActionPoints);
                if (!usable)
                {
                    trySelectResult = ActionSelectionResult.NoNearbyDisc;
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
                SoundManager.Instance.PlaySound(Sound.SelectCompetence, Camera.main.transform.position);
                break;

            default:
                SoundManager.Instance.PlaySound(Sound.NotEnoughActionPoint, Camera.main.transform.position);
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
        GameManager.Instance.CheckForCompetencesUsability();
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
    DiscScript aboutToThrowDisc = default;
    public void StartThrowPreparation()
    {
        aboutToThrowDisc = DiscManager.Instance.PeekNextThrowDisc();

        Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition);
        //Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(GameManager.Instance.GetDebugPos);
        DiscTrajectoryParameters trajectoryParameters = 
            DiscTrajectoryFactory.GetTrajectory(throwCompetence, _player.transform.position, trueTargetPosition,
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, aboutToThrowDisc);

        Dictionary<DiscScript, DiscTrajectoryParameters> trajectories = new Dictionary<DiscScript, DiscTrajectoryParameters>();
        trajectories.Add(aboutToThrowDisc, trajectoryParameters);
        PreviewCompetencesManager.Instance.StartThrowPreview(trajectories, _player.transform.position);
        PreviewCompetencesManager.Instance.UpdateThrowPreview(trajectories);
    }

    public void UpdateThrowPreparation()
    {
        Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(currentWorldMouseResult.mouseWorldPosition);
        //Vector3 trueTargetPosition = GetInRangeThrowTargetPosition(GameManager.Instance.GetDebugPos);
        DiscTrajectoryParameters trajectoryParameters = 
            DiscTrajectoryFactory.GetTrajectory(throwCompetence, _player.transform.position, trueTargetPosition,
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, aboutToThrowDisc);

        Dictionary<DiscScript, DiscTrajectoryParameters> trajectories = new Dictionary<DiscScript, DiscTrajectoryParameters>();
        trajectories.Add(aboutToThrowDisc, trajectoryParameters);
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
        Dictionary<DiscScript, DiscTrajectoryParameters> recallTrajectoryParameters = new Dictionary<DiscScript, DiscTrajectoryParameters>();
        List<DiscScript> recallableDiscs = DiscListingFactory.GetSortedRecallableDiscs(recallCompetence, DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);
        foreach(DiscScript discInRange in recallableDiscs)
        {
            DiscTrajectoryParameters newParams = 
                DiscTrajectoryFactory.GetTrajectory(recallCompetence, discInRange.transform.position, playerPos,
                DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, discInRange);
            recallTrajectoryParameters.Add(discInRange, newParams);
        }

        PreviewCompetencesManager.Instance.StartRecallPreview(recallTrajectoryParameters, playerPos);
        PreviewCompetencesManager.Instance.UpdateRecallPreview(recallTrajectoryParameters, playerPos);
    }

    public void UpdateRecallPreparation()
    {
        Vector3 playerPos = _player.transform.position;
        Dictionary<DiscScript, DiscTrajectoryParameters> recallTrajectoryParameters = new Dictionary<DiscScript, DiscTrajectoryParameters>();

        List<DiscScript> recallableDiscs = DiscListingFactory.GetSortedRecallableDiscs(recallCompetence, DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs);
        foreach (DiscScript discInRange in recallableDiscs)
        {
            DiscTrajectoryParameters newParams =
                DiscTrajectoryFactory.GetTrajectory(recallCompetence, discInRange.transform.position, playerPos,
                DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, discInRange);
            recallTrajectoryParameters.Add(discInRange, newParams);
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
    [Header("Teleportation")]
    [SerializeField] float teleportationDuration = 1.5f;
    TimerSystem teleportationDurationSystem = new TimerSystem();
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
    DiscTrajectoryParameters currentThrowTrajectoryParameters = default;
    public void LaunchThrowCompetence(GameObject objLauncher)
    {
        OnDiscThrownAnimEvent?.Invoke(); //Event
        ChangeUsabilityState(UsabilityState.Using, ActionType.Throw);
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
        currentObjLauncher = objLauncher;
        currentThrowPosition = currentWorldMouseResult.mouseWorldPosition;

        Vector3 lookPos = currentThrowPosition;
        List<DiscScript> inRangeThrowedDiscs = new List<DiscScript>();
        List<DiscScript> inRangeDiscs = DiscManager.Instance.GetInRangeDiscs;
        foreach (DiscScript disc in DiscManager.Instance.GetAllThrowedDiscs)
        {
            if (inRangeDiscs.Contains(disc))
                inRangeThrowedDiscs.Add(disc);
        }

        if (inRangeThrowedDiscs.Count > 0)
        {
            foreach (TrajectoryModifier modifier in throwCompetence.GetTrajectoryModifiers)
            {
                TrajectoryModifierLinkedDiscs linkModifier = modifier as TrajectoryModifierLinkedDiscs;
                if (linkModifier != null)
                {
                    switch (linkModifier.GetLinkedDiscTrajectoryType)
                    {
                        case DiscsOrder.FromOldestToNewest:
                            lookPos = inRangeThrowedDiscs[0].transform.position;
                            break;

                        case DiscsOrder.FromNewestToOldest:
                            lookPos = inRangeThrowedDiscs[inRangeThrowedDiscs.Count - 1].transform.position;
                            break;
                    }

                    break;
                }
            }
        }
        _player.StartLookAt(lookPos);
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
        teleportationDurationSystem = new TimerSystem(teleportationDuration, EndTeleportation);
        teleportationDurationSystem.StartTimer();

        if (!canTeleport)
            return false;

        ChangeUsabilityState(UsabilityState.Using, ActionType.Special);

        return true;
    }

    public void EndTeleportation()
    {
        Vector3 initialPlayerPos = _player.transform.position;
        _player.transform.position = currentTeleportationPosition;
        if (teleportationExchangeObject != null)
        {
            teleportationExchangeObject.transform.position = initialPlayerPos;
        }
        CameraManager.instance.GetPlayerCamera.transform.position = initialPlayerPos;
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
        ResetUsabilityState();
        teleportationExchangeObject = null;
        currentTeleportationPosition = Vector3.zero;
    }
    #endregion

    List<DiscScript> currentlyInUseDiscs = new List<DiscScript>();
    public void RemoveDiscFromInUse(DiscScript disc)
    {
        if (currentlyInUseDiscs.Contains(disc))
            currentlyInUseDiscs.Remove(disc);

        if (currentlyInUseDiscs.Count == 0)
            EndCompetenceUsability();
    }

    public void EndCompetenceUsability()
    {
        ResetUsabilityState();
    }

    public List<bool> GetCompetencesUsability(bool movementValue, int currentActionPoints)
    {
        List<bool> values = new List<bool>();
        values.Add(movementValue);
        values.Add(GetThrowCompetenceUsable(currentActionPoints));
        values.Add(GetRecallCompetenceUsable(currentActionPoints));
        values.Add(GetSpecialCompetenceUsable(currentActionPoints));

        return values;
    }

    public bool GetThrowCompetenceUsable(int currentActionPoints)
    {
        bool usable = true;

        if (throwCompetence.GetActionPointsCost > currentActionPoints)
            return false;
        else if(DiscManager.Instance.GetPossessedDiscsCount == 0)
            return false;

        return usable;
    }

    public bool GetRecallCompetenceUsable(int currentActionPoints)
    {
        bool usable = true;

        List<DiscScript> inRange = DiscManager.Instance.GetInRangeDiscs;
        List<DiscScript> throwed = DiscManager.Instance.GetAllThrowedDiscs;
        List<DiscScript> recallable = DiscListingFactory.GetSortedRecallableDiscs(recallCompetence, throwed, inRange);

        if (recallCompetence.GetActionPointsCost > currentActionPoints)
            return false;
        else if (recallable.Count == 0)
            return false;

        return usable;
    }

    public bool GetSpecialCompetenceUsable(int currentActionPoints)
    {
        bool usable = true;

        if (specialCompetence.GetActionPointsCost > currentActionPoints)
            return false;
        else
        {
            CompetenceSpecialTeleportation foundTeleportationCompetence = specialCompetence as CompetenceSpecialTeleportation;
            if (foundTeleportationCompetence != null)
            {
                if (foundTeleportationCompetence.GetTeleportationMode == TeleportationMode.Exchange)
                {
                    if (foundTeleportationCompetence.GetTeleportationTarget == TeleportationTarget.NewestDisc || foundTeleportationCompetence.GetTeleportationTarget == TeleportationTarget.OldestDisc)
                    {
                        if (DiscManager.Instance.GetInRangeDiscsCount == 0)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return usable;
    }
}
