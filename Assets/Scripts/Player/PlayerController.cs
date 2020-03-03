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

        #region Competence Selection
        if (Input.GetKeyDown(selectMoveInput))
        {
            if (competenceSystem.GetCurrentUsabilityState == CompetenceUsabilityState.Preparing)
                competenceSystem.InterruptPreparation();

            StartMovementPreparation();
        }
        else if (Input.GetKeyDown(throwCompetenceInput))
        {
            if (willingToMove)
                InterruptMovementPreparation();

            competenceSystem.ChangeUsabilityState(CompetenceUsabilityState.Preparing, CompetenceType.Throw);
        }
        else if (Input.GetKeyDown(recallCompetenceInput))
        {
            if (willingToMove)
                InterruptMovementPreparation();

            competenceSystem.ChangeUsabilityState(CompetenceUsabilityState.Preparing, CompetenceType.Recall);
        }
        #endregion

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
    }

    [Header("Movement")]
    bool willingToMove = false;
    bool isMoving = false;
    bool IsUsingMoveSystem => willingToMove || isMoving;

    public void StartMovementPreparation()
    {
        willingToMove = true;
    }

    public void InterruptMovementPreparation()
    {
        willingToMove = false;
    }

    public void UpdateMovementPreview()
    {
        float movementDistance = Vector3.Distance(transform.position, GetCurrentWorldClickResult.mouseWorldPosition);

        Debug.Log("Preparing move");
        //Debug.Log(movementDistance);
    }

    public void TryStartMovement(Vector3 targetPosition)
    {
        if (isMoving)
            return;

        navMeshAgent.SetDestination(targetPosition);
        isMoving = true;
        willingToMove = false;
    }

    [Header("Systems")]
    [SerializeField] PlayerCompetenceSystem competenceSystem = default;
    public void UpdateCompetenceSystem()
    {

    }

    [Header("Resources - PH")]
    [SerializeField] int maxAP = 3;
    int currentAP = default;

    private void Start()
    {
        currentAP = maxAP;
    }

    private void Update()
    {
        UpdateCompetenceSystem();

        if (isMoving)
        {
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0)
                isMoving = false;
        }

        if (willingToMove)
            UpdateMovementPreview();

        UpdateInputs();

        if (Input.GetKeyDown(KeyCode.T))
        {
            currentAP = maxAP;
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
