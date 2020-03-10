using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PlayerMovementsManager
{
    public void SetUp(PlayerController player)
    {
        _player = player;
        _player.OnPlayerReachedMovementDestination += EndMovement;

        _playerNavMeshAgent = player.GetNavMeshAgent;
    }

    public void UpdateSystem()
    {
        if (IsWillingToMove)
            UpdateMovementPreparation();
    }

    PlayerController _player = default;
    NavMeshAgent _playerNavMeshAgent = default;
    WorldMouseResult currentWorldMouseResult = default;
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

        GenerateDistancesPerActionPoints(currentlyAvailableActionPoints);

        currentUsabilityState = UsabilityState.Preparing;

        currentPreviewCost = 0;

        List<Vector3> trueMovementTrajectory = GetClampedTrajectory(currentWorldMouseResult.mouseWorldPosition);
        float movementDistance = 0;
        for (int i = 1; i < trueMovementTrajectory.Count; i++)
        {
            movementDistance += Vector3.Distance(trueMovementTrajectory[i - 1], trueMovementTrajectory[i]);
        }

        bool maxRangeMovement = Mathf.Abs(movementDistance - GetMaxDistance()) < 0.01f;

        Vector3 targetPosition = trueMovementTrajectory[trueMovementTrajectory.Count - 1];

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

        PreviewCompetencesManager.Instance.StartMovementPreview(currentDistancesByUsedActionPoints, trueMovementTrajectory,
            currentRecallCompetence, maxRangeMovement ? currentPreviewCost : currentPreviewCost - 1, maxRangeMovement);

        UIManager.Instance.ShowActionPointsCostText();
        UIManager.Instance.UpdateActionPointCostText(movementCost, currentlyAvailableActionPoints);
    }

    public void UpdateMovementPreparation()
    {
        List<Vector3> trueMovementTrajectory = GetClampedTrajectory(currentWorldMouseResult.mouseWorldPosition);
        float movementDistance = 0;
        for(int i = 1; i < trueMovementTrajectory.Count; i++)
        {
            movementDistance += Vector3.Distance(trueMovementTrajectory[i-1], trueMovementTrajectory[i]);
        }

        bool maxRangeMovement = Mathf.Abs(movementDistance - GetMaxDistance()) < 0.01f;

        Vector3 targetPosition = trueMovementTrajectory[trueMovementTrajectory.Count - 1];

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

        PreviewCompetencesManager.Instance.UpdateMovementPreview(trueMovementTrajectory, 
            currentRecallCompetence, maxRangeMovement ? currentPreviewCost : currentPreviewCost - 1, maxRangeMovement);

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

    public List<Vector3> GetClampedTrajectory(Vector3 baseTargetPos)
    {
        _playerNavMeshAgent.SetDestination(baseTargetPos);

        List<Vector3> trajectory = new List<Vector3>();
        float maxDist = GetMaxDistance();
        float currentDistance = 0;
        float previousDistance = 0;

        trajectory.Add(_player.transform.position);

        NavMeshPath _path = _playerNavMeshAgent.path;
        Vector3 previousPos = trajectory[0];
        foreach (Vector3 step in _path.corners)
        {
            Vector3 totalMovement = step - previousPos;

            currentDistance += totalMovement.magnitude;
            if(currentDistance >= maxDist)
            {
                currentDistance = maxDist;
                trajectory.Add(previousPos + totalMovement.normalized * (currentDistance - previousDistance));
                break;
            }
            else
            {
                trajectory.Add(step);
                previousDistance = currentDistance;
            }

            previousPos = step;
        }

        return trajectory;
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

        List<Vector3> trajectory = GetClampedTrajectory(targetPosition);
        targetPosition = trajectory[trajectory.Count - 1];

        float movementDistance = 0;
        for (int i = 1; i < trajectory.Count; i++)
        {
            movementDistance += Vector3.Distance(trajectory[i - 1], trajectory[i]);
        }

        int movementCost = GetActionPointsByDistance(movementDistance);
        

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
