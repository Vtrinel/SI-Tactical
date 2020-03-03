using System;
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
            UpdateMovementPreview();
    }

    PlayerController _player;
    WorldClickResult currentWorldMouseResult;
    public void UpdateCurrentWorldMouseResult(WorldClickResult result)
    {
        currentWorldMouseResult = result;
    }

    [Header("Movement")]
    [SerializeField] float movementDistancePerActionPoint = 1;
    UsabilityState currentUsabilityState = UsabilityState.None;
    public bool IsWillingToMove => currentUsabilityState == UsabilityState.Preparing;
    public bool IsMoving => currentUsabilityState == UsabilityState.Using;

    public bool IsUsingMoveSystem => IsWillingToMove || IsMoving;
    List<float> currentDistancesByUsedActionPoints = new List<float>();
    int availableActionPoints = 0;

    [Header("Movement - PH")]
    [SerializeField] LineRenderer movementLinePreview = default;
    public void UpdateLinePreviewState()
    {
        if (movementLinePreview != null)
            movementLinePreview.enabled = IsWillingToMove;
    }

    public void StartMovementPreparation()
    {
        currentUsabilityState = UsabilityState.Preparing;

        currentPreviewCost = 0;

        UpdateLinePreviewState();
    }

    public Action<int> OnPreparationAmountChanged;
    public void GenerateDistancesPerActionPoints(int actionPointAmount)
    {
        availableActionPoints = actionPointAmount;

        currentDistancesByUsedActionPoints = new List<float>();

        for (int i = 1; i <= availableActionPoints; i++)
        {
            currentDistancesByUsedActionPoints.Add(GetDistanceByUsedActionPoints(i));
        }
    }

    public float GetDistanceByUsedActionPoints(int actionPointsAmount)
    {
        float dist = (float)actionPointsAmount * movementDistancePerActionPoint;

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

    public void InterruptMovementPreparation()
    {
        currentUsabilityState = UsabilityState.None;

        UpdateLinePreviewState();

        currentDistancesByUsedActionPoints = new List<float>();
    }

    int currentPreviewCost = 0;
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
    }

    /// <summary>
    /// Return -1 if error (not enough points,...)
    /// Returns cost if succeeded
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public int TryStartMovement(Vector3 targetPosition)
    {
        bool succeed = true;

        if (IsMoving)
            return -1;

        float movementDistance = Vector3.Distance(_player.transform.position, currentWorldMouseResult.mouseWorldPosition);

        int movementCost = GetActionPointsByDistance(movementDistance);

        if(movementCost > availableActionPoints)
        {
            Debug.Log("NOT ENOUGH POINTS TO MOVE");
            return -1;
        }
               
        _player.MoveTo(targetPosition);
        currentUsabilityState = UsabilityState.Using;
        UpdateLinePreviewState();

        return movementCost;
    }

    public void EndMovement()
    {
        currentUsabilityState = UsabilityState.None;
    }
}
