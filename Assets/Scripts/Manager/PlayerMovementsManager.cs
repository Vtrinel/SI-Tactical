﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovementsManager
{
    public void SetUp(PlayerController player)
    {
        _player = player;
        _player.OnPlayerReachedMovementDestination += EndMovement;

        movementPlayerPreview = Object.Instantiate(movementPlayerPreviewPrefab);
        movementPlayerPreview.SetActive(false);
    }

    public void UpdateSystem()
    {
        if (IsWillingToMove)
            UpdateMovementPreview(currentWorldMouseResult.mouseWorldPosition);
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

    #region Preview
    [Header("Movement Preview")]
    [SerializeField] GameObject movementPlayerPreviewPrefab = default;
    GameObject movementPlayerPreview = default;
    CompetenceRecall currentRecallCompetence = default;
    public void UpdateCurrentRecallCompetence(CompetenceRecall competenceRecall)
    {
        currentRecallCompetence = competenceRecall;
    }

    bool justStartedMovementPreview = false;
    public void StartMovementPreview(Vector3 targetPosition)
    {
        if (movementLinePreview != null)
            movementLinePreview.enabled = true;

        GenerateDebugCircles();
        movementPlayerPreview.SetActive(true);

        List<DiscTrajectoryParameters> discsInNewPositionRangeParameters = GetDiscInRangeTrajectory(targetPosition);
        PreviewCompetencesManager.Instance.StartRecallPreview(discsInNewPositionRangeParameters, targetPosition);

        justStartedMovementPreview = true;
        UpdateMovementPreview(targetPosition);
    }

    int currentPreviewCost = 0;
    public void UpdateMovementPreview(Vector3 targetPosition)
    {
        float movementDistance = Vector3.Distance(_player.transform.position, targetPosition);

        int movementCost = GetActionPointsByDistance(movementDistance);

        if (movementLinePreview != null)
        {
            movementLinePreview.SetPositions(new Vector3[] { _player.transform.position + Vector3.up, targetPosition + Vector3.up });
            movementLinePreview.startColor = movementCost <= availableActionPoints ? Color.green : Color.red;
            movementLinePreview.endColor = movementCost <= availableActionPoints ? Color.green : Color.red;
        }

        if (movementCost != currentPreviewCost)
        {
            currentPreviewCost = movementCost;
            OnPreparationAmountChanged?.Invoke(currentPreviewCost);
        }

        movementPlayerPreview.transform.position = targetPosition;

        if (!justStartedMovementPreview)
        {
            List<DiscTrajectoryParameters> discsInNewPositionRangeParameters = GetDiscInRangeTrajectory(targetPosition);
            PreviewCompetencesManager.Instance.UpdateRecallPreview(discsInNewPositionRangeParameters, targetPosition);
        }
        else
            justStartedMovementPreview = false;
    }

    public void EndMovementPreview()
    {
        justStartedMovementPreview = false;

        if (movementLinePreview != null)
            movementLinePreview.enabled = false;

        ClearInstantiatedDebugCircles();
        movementPlayerPreview.SetActive(false);
        PreviewCompetencesManager.Instance.EndRecallPreview();
    }

    public List<DiscTrajectoryParameters> GetDiscInRangeTrajectory(Vector3 targetPosition)
    {
        List<DiscTrajectoryParameters> allTrajParams = new List<DiscTrajectoryParameters>();
        foreach (DiscScript disc in DiscManager.Instance.GetAllInRangeDiscsFromPosition(targetPosition))
        {
            DiscTrajectoryParameters newTrajParams = DiscTrajectoryFactory.GetRecallTrajectory(currentRecallCompetence, disc.transform.position, targetPosition);
            allTrajParams.Add(newTrajParams);
        }
        return allTrajParams;
    }

    [Header("Movement - PH")]
    [SerializeField] LineRenderer movementLinePreview = default;
    [SerializeField] GameObject debugCirclePrefab = default;
    List<GameObject> instanciatedDebugCircles = new List<GameObject>();
    public void GenerateDebugCircles()
    {
        ClearInstantiatedDebugCircles();

        foreach (float dist in currentDistancesByUsedActionPoints)
        {
            GameObject debugCircle = GameObject.Instantiate(debugCirclePrefab, _player.transform.position + Vector3.up * 0.01f, Quaternion.identity);
            debugCircle.transform.localScale = Vector3.one * dist;
            instanciatedDebugCircles.Add(debugCircle);
        }
    }
    public void ClearInstantiatedDebugCircles()
    {
        if(instanciatedDebugCircles != null)
        {
            foreach (GameObject debugCircle in instanciatedDebugCircles)
                GameObject.Destroy(debugCircle);
        }
        instanciatedDebugCircles = new List<GameObject>();
    }
    #endregion

    public void StartMovementPreparation()
    {
        currentUsabilityState = UsabilityState.Preparing;

        currentPreviewCost = 0;

        StartMovementPreview(currentWorldMouseResult.mouseWorldPosition);
    }

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

        GenerateDebugCircles();
    }

    #region Distance - AP Relation
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
    #endregion

    public void InterruptMovementPreparation()
    {
        currentUsabilityState = UsabilityState.None;

        EndMovementPreview();

        currentDistancesByUsedActionPoints = new List<float>();
    }

    /*int currentPreviewCost = 0;
    public void UpdateMovementPreview()
    {
        float movementDistance = Vector3.Distance(_player.transform.position, currentWorldMouseResult.mouseWorldPosition);

        int movementCost = GetActionPointsByDistance(movementDistance);

        if (movementLinePreview != null)
        {
            movementLinePreview.SetPositions(new Vector3[] { _player.transform.position + Vector3.up, currentWorldMouseResult.mouseWorldPosition + Vector3.up });
            movementLinePreview.startColor = movementCost <= availableActionPoints ? Color.green : Color.red;
            movementLinePreview.endColor = movementCost <= availableActionPoints ? Color.green : Color.red;
        }

        if(movementCost != currentPreviewCost)
        {
            currentPreviewCost = movementCost;
            OnPreparationAmountChanged?.Invoke(currentPreviewCost);
        }
    }*/

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
            Debug.Log("NOT ENOUGH POINTS TO MOVE");
            return -1;
        }

        ClearInstantiatedDebugCircles();

        _player.MoveTo(targetPosition);
        currentUsabilityState = UsabilityState.Using;
        EndMovementPreview();

        actionPointsUsedThisTurnToMove += movementCost;

        return movementCost;
    }

    public void EndMovement()
    {
        currentUsabilityState = UsabilityState.None;
    }
}
