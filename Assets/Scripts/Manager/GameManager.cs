using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        playerMovementsManager.SetUp(player);

        OnWorldMouseResultUpdate += playerMovementsManager.UpdateCurrentWorldMouseResult;
        playerMovementsManager.OnPreparationAmountChanged += UpdateActionPointsDebugTextAmount;

        player.OnPlayerReachedMovementDestination += UpdatePlayerActability;

        UpdatePlayerActability();

        ResetActionPointsCount();
    }

    private void Update()
    {
        currentWorldClickResult = GetCurrentWorldClickResult;
        OnWorldMouseResultUpdate?.Invoke(currentWorldClickResult);

        playerMovementsManager.UpdateSystem();
    }

    private void LateUpdate()
    {
        calculatedCurrentWorldClickResult = false;
    }

    [Header("Important References")]
    [SerializeField] PlayerController player = default;
    public PlayerController GetPlayer => player;

    [Header("Action Points")]
    [SerializeField] int maxActionPointsAmount = 10;
    int currentActionPointsAmount;
    public int GetCurrentActionPointsAmount => currentActionPointsAmount;
    
    public void ResetActionPointsCount()
    {
        currentActionPointsAmount = maxActionPointsAmount;
    }

    public bool CanConsumeActionPointsAmount(int amount)
    {
        return amount <= currentActionPointsAmount;
    }

    public void ConsumeActionPoints(int amount)
    {
        currentActionPointsAmount -= amount;

        if (currentActionPointsAmount < 0)
        {
            currentActionPointsAmount = 0;
            Debug.LogWarning("WARNING : Too much action points consumed, count got negative.");
        }
    }

    [SerializeField] UnityEngine.UI.Text actionPointsUseDebugText = default;
    public void SetActionPointsDebugTextVisibility(bool visible)
    {
        if (actionPointsUseDebugText != null)
            actionPointsUseDebugText.enabled = visible;
    }
    public void UpdateActionPointsDebugTextAmount(int amount)
    {
        if (actionPointsUseDebugText != null)
            actionPointsUseDebugText.text = amount + "/" + currentActionPointsAmount;
    }

    [Header("Player Systems")]
    [SerializeField] PlayerMovementsManager playerMovementsManager;

    [Header("Mouse World Position")]
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

        foreach (RaycastHit hit in hits)
        {
            PlayerInputSurface hitInputSurface = hit.collider.GetComponent<PlayerInputSurface>();
            if (hitInputSurface != null)
            {
                result.mouseWorldPosition = hit.point;
            }
        }

        return result;
    }

    System.Action<WorldClickResult> OnWorldMouseResultUpdate = default;

    //[Header("Inputs Events")]
    public void SelectAction(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.None:
                break;

            case ActionType.Move:
                if (!playerMovementsManager.IsWillingToMove)
                {
                    playerMovementsManager.GenerateDistancesPerActionPoints(currentActionPointsAmount);
                    playerMovementsManager.StartMovementPreparation();
                    SetActionPointsDebugTextVisibility(true);
                }
                else
                {
                    playerMovementsManager.InterruptMovementPreparation();
                    SetActionPointsDebugTextVisibility(false);
                }
                break;

            case ActionType.Throw:
                break;

            case ActionType.Recall:
                break;
        }
    }

    public void OnPlayerClickAction()
    {
        if (playerMovementsManager.IsWillingToMove)
        {
            int cost = playerMovementsManager.TryStartMovement(GetCurrentWorldClickResult.mouseWorldPosition);
            if (cost > 0)
            {
                SetActionPointsDebugTextVisibility(false);
                ConsumeActionPoints(cost);
            }
        }

        UpdatePlayerActability();
    }

    public void UpdatePlayerActability()
    {
        player.SetAbleToAct(!playerMovementsManager.IsUsingMoveSystem);
    }
}

public enum ActionType
{
    None, Move, Throw, Recall
}

public enum UsabilityState
{
    None, Preparing, Using
}