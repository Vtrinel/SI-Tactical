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

    public void UpdateInputs()
    {
        if (Input.GetKeyDown(clickActionKey))
        {
            if(willingToMove)
                TryStartMovement(GetCurrentWorldClickResult.mouseWorldPosition);
        }
    }

    [Header("Movement")]
    bool willingToMove = false;
    bool isMoving = false;

    public void UpdateMovementPreview()
    {
        float movementDistance = Vector3.Distance(transform.position, GetCurrentWorldClickResult.mouseWorldPosition);

        Debug.Log(movementDistance);
    }

    public void TryStartMovement(Vector3 targetPosition)
    {
        if (isMoving)
            return;

        navMeshAgent.SetDestination(targetPosition);
        isMoving = true;
        willingToMove = false;
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
        if (isMoving)
        {
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0)
                isMoving = false;
        }

        if (willingToMove)
            UpdateMovementPreview();

        UpdateInputs();

        if (Input.GetKeyDown(KeyCode.M))
        {
            willingToMove = true;
        }

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
