using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovementsManager
{
    public void SetUp(PlayerController player)
    {
        _player = player;
        _player.OnPlayerReachedMovementDestination += EndMovement;
    }

    public void UpdateSystem()
    {
        if (IsWillingToMove)
            UpdateMovementPreparation();
    }

    PlayerController _player;
    WorldMouseResult currentWorldMouseResult;
    public void UpdateCurrentWorldMouseResult(WorldMouseResult result)
    {
        currentWorldMouseResult = result;
    }

    [Header("Movement")]
    [SerializeField] float baseMovementDistancePerActionPoint = 2f;
    [SerializeField] float distanceReductionCoeffPerActionPoint = 1.2f;
    [SerializeField] float distanceReductionModificationCoeffPerActionPoint = 0.5f;
    [SerializeField] float minimumDistance = 1f;
    UsabilityState currentUsabilityState = UsabilityState.None;
    public bool IsWillingToMove => currentUsabilityState == UsabilityState.Preparing;
    public bool IsMoving => currentUsabilityState == UsabilityState.Using;

    public bool IsUsingMoveSystem => IsWillingToMove || IsMoving;
    List<float> currentDistancesByUsedActionPoints = new List<float>();
    int availableActionPoints = 0;
    int actionPointsUsedThisTurnToMove = 0;

    public void ResetActionPointsUsedThisTurn()
    {
        actionPointsUsedThisTurnToMove = 0;
    }

    CompetenceRecall currentRecallCompetence = default;
    public void UpdateCurrentRecallCompetence(CompetenceRecall competenceRecall)
    {
        currentRecallCompetence = competenceRecall;
    }

    #region Preparation
    int currentPreviewCost = 0;
    int currentlyAvailableActionPoints = 0;
    public void StartMovementPreparation(int availableActionPoints)
    {
        currentlyAvailableActionPoints = availableActionPoints;

        bool maxRangeMovement = false;

        GenerateDistancesPerActionPoints(currentlyAvailableActionPoints);

        currentUsabilityState = UsabilityState.Preparing;

        currentPreviewCost = 0;

        Vector3 targetPosition = currentWorldMouseResult.mouseWorldPosition;
        float movementDistance = Vector3.Distance(_player.transform.position, targetPosition);

        int movementCost = GetActionPointsByDistance(movementDistance);
        if (movementCost > currentlyAvailableActionPoints)
        {
            maxRangeMovement = true;
            targetPosition = GetClampedTargetPosition(targetPosition);
            movementCost = currentlyAvailableActionPoints;
        }

        if (movementCost != currentPreviewCost)
        {
            currentPreviewCost = movementCost;
            OnPreparationAmountChanged?.Invoke(currentPreviewCost);
        }

        PreviewCompetencesManager.Instance.StartMovementPreview(currentDistancesByUsedActionPoints, _player.transform.position, targetPosition,
            currentRecallCompetence, maxRangeMovement ? currentPreviewCost : currentPreviewCost - 1);
        PreviewCompetencesManager.Instance.UpdateMovementPreview(_player.transform.position, targetPosition,
            currentRecallCompetence, maxRangeMovement ? currentPreviewCost : currentPreviewCost - 1);

        UIManager.Instance.ShowActionPointsCostText();
        UIManager.Instance.UpdateActionPointCostText(movementCost, currentlyAvailableActionPoints);
    }

    public void UpdateMovementPreparation()
    {
        bool maxRangeMovement = false;

        Vector3 targetPosition = currentWorldMouseResult.mouseWorldPosition;
        float movementDistance = Vector3.Distance(_player.transform.position, targetPosition);

        int movementCost = GetActionPointsByDistance(movementDistance);
        if (movementCost > availableActionPoints)
        {
            maxRangeMovement = true;
            targetPosition = GetClampedTargetPosition(targetPosition);
            movementCost = availableActionPoints;
        }

        if (movementCost != currentPreviewCost)
        {
            currentPreviewCost = movementCost;
            OnPreparationAmountChanged?.Invoke(currentPreviewCost);
        }

        PreviewCompetencesManager.Instance.UpdateMovementPreview(_player.transform.position, targetPosition, 
            currentRecallCompetence, maxRangeMovement ? currentPreviewCost : currentPreviewCost - 1);

        UIManager.Instance.UpdateActionPointCostText(movementCost, currentlyAvailableActionPoints);
    }

    public void InterruptMovementPreparation()
    {
        currentUsabilityState = UsabilityState.None;

        EndMovementPreparation();

        currentDistancesByUsedActionPoints = new List<float>();
    }

    public void EndMovementPreparation()
    {
        PreviewCompetencesManager.Instance.EndMovementPreview();
        UIManager.Instance.HideActionPointText();
    }
    #endregion

    #region Distance - AP Relation
    public System.Action<int> OnPreparationAmountChanged;
    public void GenerateDistancesPerActionPoints(int actionPointAmount)
    {
        availableActionPoints = actionPointAmount;

        currentDistancesByUsedActionPoints = new List<float>();

        float totalDistance = 0;
        for (int i = 1; i <= availableActionPoints; i++)
        {
            totalDistance += GetDistanceByUsedActionPoints(i);
            currentDistancesByUsedActionPoints.Add(totalDistance);
        }
    }

    public float GetDistanceByUsedActionPoints(int actionPointsAmount)
    {
        float floatedActionPoints = (float)actionPointsAmount;

        float dist = baseMovementDistancePerActionPoint;
        float power = (floatedActionPoints + actionPointsUsedThisTurnToMove - 1) * distanceReductionModificationCoeffPerActionPoint;
        float reductionCoeff = (Mathf.Pow(distanceReductionCoeffPerActionPoint, power));

        if (reductionCoeff > 0)
            dist /= reductionCoeff;

        if (dist < minimumDistance)
            dist = minimumDistance;

        return dist;
    }

    public int GetActionPointsByDistance(float targetDistance)
    {
        int cost = 1;

        foreach (float dist in currentDistancesByUsedActionPoints)
        {
            if (targetDistance > dist)
                cost++;
            else
                break;
        }

        return cost;
    }

    public float GetMaxDistance()
    {
        return currentDistancesByUsedActionPoints[availableActionPoints - 1];
    }

    public Vector3 GetClampedTargetPosition(Vector3 baseTargetPos)
    {
        float maxDistance = GetMaxDistance();

        Vector3 playerPos = _player.transform.position;
        Vector3 trueTargetPos = baseTargetPos;
        trueTargetPos.y = playerPos.y;

        Vector3 moveDirection = (trueTargetPos - playerPos).normalized;
        trueTargetPos = playerPos + moveDirection.normalized * maxDistance;
        trueTargetPos.y = baseTargetPos.y;

        return trueTargetPos;
    }
    #endregion
       
    /// <summary>
    /// Return -1 if error (not enough points,...)
    /// Returns cost if succeeded
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public int TryStartMovement(Vector3 targetPosition)
    {
        if (IsMoving)
            return -1;

        float movementDistance = Vector3.Distance(_player.transform.position, currentWorldMouseResult.mouseWorldPosition);

        int movementCost = GetActionPointsByDistance(movementDistance);

        if(movementCost > availableActionPoints)
        {
            targetPosition = GetClampedTargetPosition(targetPosition);
            movementCost = availableActionPoints;
        }

        _player.MoveTo(targetPosition);
        currentUsabilityState = UsabilityState.Using;

        EndMovementPreparation();

        actionPointsUsedThisTurnToMove += movementCost;

        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();

        return movementCost;
    }

    public void EndMovement()
    {
        currentUsabilityState = UsabilityState.None;
    }
}
