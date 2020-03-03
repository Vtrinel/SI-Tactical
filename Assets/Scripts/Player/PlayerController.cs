using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NavMeshAgent navMeshAgent = default;

    [Header("Inputs")]
    [SerializeField] KeyCode clickActionKey = KeyCode.Mouse0;
    [SerializeField] LayerMask worldClickLayerMask = default;
    [SerializeField] float clickCheckMaxDistance = 50.0f;
    bool calculatedCurrentWorldClickResult = false;
    WorldClickResult currentWorldClickResult = default;
    public WorldClickResult GetCurrentWorldClickResult
    {
        get
        {
            if (!calculatedCurrentWorldClickResult)
                currentWorldClickResult = CalculateCurrentWorldClickResult();

            return currentWorldClickResult;
        }
    }
    public WorldClickResult CalculateCurrentWorldClickResult()
    {
        WorldClickResult result = default;

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(cameraRay, clickCheckMaxDistance, worldClickLayerMask);

        foreach(RaycastHit hit in hits)
        {
            PlayerInputSurface hitInputSurface = hit.collider.GetComponent<PlayerInputSurface>();
            if (hitInputSurface != null)
            {
                result.mouseWorldPosition = hit.point;
            }
        }

        return result;
    }

    [SerializeField] KeyCode selectMoveInput = KeyCode.Q;
    [SerializeField] KeyCode throwCompetenceInput = KeyCode.A;
    [SerializeField] KeyCode recallCompetenceInput = KeyCode.Z;

    public void UpdateInputs()
    {
        if (isMoving || competenceSystem.GetCurrentUsabilityState == CompetenceUsabilityState.Using)
            return;

        #region Action Selection
        if (Input.GetKeyDown(selectMoveInput))
        {
            if (competenceSystem.GetCurrentUsabilityState == CompetenceUsabilityState.Preparing)
                competenceSystem.InterruptPreparation();

            StartMovementPreparation();
        }
        else if (Input.GetKeyDown(throwCompetenceInput))
        {
            TrySelectCompetence(CompetenceType.Throw);

        }
        else if (Input.GetKeyDown(recallCompetenceInput))
        {
            TrySelectCompetence(CompetenceType.Recall);

        }
        #endregion

        #region Action Validation
        if (willingToMove)
        {
            if (Input.GetKeyDown(clickActionKey))
                TryStartMovement(GetCurrentWorldClickResult.mouseWorldPosition);
        }
        else if (competenceSystem.GetCurrentUsabilityState == CompetenceUsabilityState.Preparing)
        {
            Debug.Log("Preparing " + competenceSystem.GetCurrentCompetenceType);

            if (Input.GetKeyDown(clickActionKey))
            {
                switch (competenceSystem.GetCurrentCompetenceType)
                {
                    case CompetenceType.None:
                        break;

                    case CompetenceType.Throw:
                        CompetanceRequestInfo newThrow = new CompetanceRequestInfo();
                        newThrow.startPosition = transform.position;
                        newThrow.startTransform = transform;
                        newThrow.targetPosition = GetCurrentWorldClickResult.mouseWorldPosition;

                        Debug.DrawRay(newThrow.startPosition, Vector3.up * 10.0f, Color.red, 5.0f);
                        Debug.DrawRay(newThrow.targetPosition, Vector3.up * 10.0f, Color.blue, 5.0f);

                        competenceSystem.LaunchThrowCompetence(newThrow);
                        break;

                    case CompetenceType.Recall:
                        CompetanceRequestInfo newRecall = new CompetanceRequestInfo();
                        newRecall.targetPosition = transform.position;
                        newRecall.targetTransform = transform;

                        competenceSystem.LaunchRecallCompetence(newRecall);
                        break;
                }
            }
        }
        #endregion
    }

    [Header("Movement")]
    [SerializeField] float movementDistancePerActionPoint = 1;
    bool willingToMove = false;
    bool isMoving = false;
    bool IsUsingMoveSystem => willingToMove || isMoving;
    List<float> currentDistancesByUsedActionPoints = new List<float>();

    [Header("Movement - PH")]
    [SerializeField] LineRenderer movementLinePreview = default;
    public void UpdateLinePreviewState()
    {
        if (movementLinePreview != null)
            movementLinePreview.enabled = willingToMove;
    }

    public void StartMovementPreparation()
    {
        willingToMove = true;

        UpdateLinePreviewState();
        UpdateAPCostPreviewState();

        GenerateDistancesPerActionPoints();
    }

    public void InterruptMovementPreparation()
    {
        willingToMove = false;

        UpdateLinePreviewState();
        UpdateAPCostPreviewState();

        currentDistancesByUsedActionPoints = new List<float>();
    }

    public void UpdateMovementPreview()
    {
        float movementDistance = Vector3.Distance(transform.position, GetCurrentWorldClickResult.mouseWorldPosition);

        int movementCost = GetActionPointsByDistance(movementDistance);

        if (movementLinePreview != null)
        {
            movementLinePreview.SetPositions(new Vector3[] { transform.position + Vector3.up, GetCurrentWorldClickResult.mouseWorldPosition + Vector3.up });
            movementLinePreview.startColor = movementCost <= currentActionPoints ? Color.green : Color.red;
            movementLinePreview.endColor = movementCost <= currentActionPoints ? Color.green : Color.red;
        }

        UpdateActionPointsDebug(movementCost);
        //Debug.Log("Preparing move. Target distance : " + movementDistance + "; Cost : " + movementCost);
    }

    public void GenerateDistancesPerActionPoints()
    {
        currentDistancesByUsedActionPoints = new List<float>();

        for (int i = 1; i <= currentActionPoints; i++)
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

        foreach(float dist in currentDistancesByUsedActionPoints)
        {
            if (targetDistance > dist)
                cost++;
            else
                break;
        }

        return cost;
    }    

    public void TryStartMovement(Vector3 targetPosition)
    {
        if (isMoving)
            return;

        float movementDistance = Vector3.Distance(transform.position, GetCurrentWorldClickResult.mouseWorldPosition);

        int movementCost = GetActionPointsByDistance(movementDistance);

        if(movementCost > currentActionPoints)
        {
            Debug.Log("Not enough AP");
            return;
        }

        ConsumeActionPoints(movementCost);

        navMeshAgent.SetDestination(targetPosition);
        isMoving = true;
        willingToMove = false;

        UpdateLinePreviewState();
        UpdateAPCostPreviewState();
    }

    public void UpdateMovement()
    {

    }

    [Header("Systems")]
    [SerializeField] PlayerCompetenceSystem competenceSystem = default;
    public void UpdateCompetenceSystem()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0)
            isMoving = false;
    }

    public void TrySelectCompetence(CompetenceType compType)
    {
        if (willingToMove)
            InterruptMovementPreparation();

        ActionSelectionResult result = competenceSystem.HasEnoughActionPoints(currentActionPoints, compType);

        switch (result)
        {
            case ActionSelectionResult.EnoughAactionPoints:
                Debug.Log("Selected " + compType);
                competenceSystem.ChangeUsabilityState(CompetenceUsabilityState.Preparing, compType);
                UpdateActionPointsDebug(competenceSystem.GetCompetenceActionPointsCost(compType));
                UpdateAPCostPreviewState();
                break;

            case ActionSelectionResult.NotEnoughActionPoints:
                Debug.Log("Not enough Action Points");
                break;

            case ActionSelectionResult.NoCompetenceFound:
                Debug.LogWarning("NO COMPETENCE FOUND");
                break;
        }
    }

    public void ConsumeCompetenceActionPoints(Competence usedComp)
    {
        ConsumeActionPoints(usedComp.GetActionPointsCost);
        UpdateAPCostPreviewState();
    }

    [Header("Resources - PH")]
    [SerializeField] int maxActionPoints = 10;
    [SerializeField] UnityEngine.UI.Text actionPointsUseDebugText = default;
    int currentActionPoints = default;
    int actionPointsUsedThisTurnToMove = 0;
    public void ConsumeActionPoints(int amount)
    {
        currentActionPoints -= amount;
    }

    public void UpdateActionPointsDebug(int cost)
    {
        if (actionPointsUseDebugText != null)
        {
            actionPointsUseDebugText.text = cost + "AP";

        }
    }
    public void UpdateAPCostPreviewState()
    {
        if (actionPointsUseDebugText != null)
            actionPointsUseDebugText.enabled = willingToMove || competenceSystem.GetCurrentUsabilityState == CompetenceUsabilityState.Preparing;
    }


    private void Start()
    {
        currentActionPoints = maxActionPoints;
        competenceSystem.OnCompetenceUsed += ConsumeCompetenceActionPoints;

        UpdateLinePreviewState();
        UpdateAPCostPreviewState();
    }

    private void Update()
    {
        //Debug.Log("Current action points : " + currentActionPoints);

        if (willingToMove)
            UpdateMovementPreview();
        else if (isMoving)
            UpdateMovement();       

        UpdateInputs();

        UpdateCompetenceSystem();

        if (Input.GetKeyDown(KeyCode.T))
        {
            currentActionPoints = maxActionPoints;
            actionPointsUsedThisTurnToMove = 0;
        }
    }

    private void LateUpdate()
    {
        calculatedCurrentWorldClickResult = false;
    }
}

public struct WorldClickResult
{
    public Vector3 mouseWorldPosition;
}

public enum ActionSelectionResult
{
    EnoughAactionPoints, NotEnoughActionPoints, NoCompetenceFound
}
