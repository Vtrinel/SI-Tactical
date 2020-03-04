using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    #region Callbacks
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
        competencesManager.SetUp(player);        

        OnWorldMouseResultUpdate += playerMovementsManager.UpdateCurrentWorldMouseResult;
        OnWorldMouseResultUpdate += competencesManager.UpdateCurrentWorldMouseResult;

        playerMovementsManager.OnPreparationAmountChanged += UpdateActionPointsDebugTextAmount;
        competencesManager.OnCompetenceStateChanged += UpdatePlayerActability;

        player.OnPlayerReachedMovementDestination += UpdatePlayerActability;

        UpdatePlayerActability();

        ResetActionPointsCount();
    }

    private void Update()
    {
        currentWorldMouseResult = GetCurrentWorldMouseResult;
        OnWorldMouseResultUpdate?.Invoke(currentWorldMouseResult);

        playerMovementsManager.UpdateSystem();
        competencesManager.UpdateSystem();
    }

    private void LateUpdate()
    {
        calculatedCurrentWorldMouseResult = false;
    }
    #endregion

    #region Turn Management
    public void OnStartPlayerTurn()
    {
        ResetActionPointsCount();
        playerMovementsManager.ResetActionPointsUsedThisTurn();
    }
    #endregion

    [Header("Important References")]
    [SerializeField] PlayerController player = default;
    public PlayerController GetPlayer => player;

    public bool OnMouseInUI = false;

    public void SetOnMouseInUI(bool value)
    {
        OnMouseInUI = value;
    }

    #region Action Points
    [Header("Action Points")]
    public int maxActionPointsAmount = 10;
    int currentActionPointsAmount;
    public int GetCurrentActionPointsAmount => currentActionPointsAmount;

    public System.Action<int> OnActionPointsAmountChanged;
    
    public void ResetActionPointsCount()
    {
        currentActionPointsAmount = maxActionPointsAmount;
        OnActionPointsAmountChanged?.Invoke(currentActionPointsAmount);
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
        OnActionPointsAmountChanged?.Invoke(currentActionPointsAmount);
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
    #endregion

    [Header("Player Systems")]
    [SerializeField] PlayerMovementsManager playerMovementsManager = default;
    [SerializeField] CompetencesManager competencesManager = default;
    public Competence GetCurrentlySelectedCompetence => competencesManager.GetCurrentCompetence;
    public Action<bool> OnMoveActionSelectionStateChanged;
    public Action<bool> OnThrowCompetenceSelectionStateChanged;
    public Action<bool> OnRecallCompetenceSelectionStateChanged;
    public Action<bool> OnSpecialCompetenceSelectionStateChanged;

    #region Mouse World Result
    [Header("Mouse World Result")]
    [SerializeField] LayerMask worldMouseLayerMask = default;
    [SerializeField] float mouseCheckMaxDistance = 50.0f;
    bool calculatedCurrentWorldMouseResult = false;
    WorldMouseResult currentWorldMouseResult = default;
    public WorldMouseResult GetCurrentWorldMouseResult
    {
        get
        {
            if (!calculatedCurrentWorldMouseResult)
                currentWorldMouseResult = CalculateCurrentWorldMouseResult();

            return currentWorldMouseResult;
        }
    }
    public WorldMouseResult CalculateCurrentWorldMouseResult()
    {
        WorldMouseResult result = default;

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(cameraRay, mouseCheckMaxDistance, worldMouseLayerMask);

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

    System.Action<WorldMouseResult> OnWorldMouseResultUpdate = default;
    #endregion

    #region Player Inputs Reception
    public void SelectAction(ActionType actionType)
    {
        if (actionType == ActionType.Move)
        {
            if (competencesManager.IsPreparingCompetence)
            {
                CallUnselectActionEvent(competencesManager.GetCurrentCompetenceType);
                competencesManager.InterruptPreparation();
            }

            if (!playerMovementsManager.IsWillingToMove)
            {
                CallSelectActionEvent(ActionType.Move);
                playerMovementsManager.GenerateDistancesPerActionPoints(currentActionPointsAmount);
                playerMovementsManager.StartMovementPreparation();
                SetActionPointsDebugTextVisibility(true);
            }
            else
            {
                CallUnselectActionEvent(ActionType.Move);
                playerMovementsManager.InterruptMovementPreparation();
                SetActionPointsDebugTextVisibility(false);
            }

        }
        else
        {
            ActionType previousActionType = competencesManager.GetCurrentCompetenceType;

            if (playerMovementsManager.IsWillingToMove)
            {
                CallUnselectActionEvent(ActionType.Move);
                playerMovementsManager.InterruptMovementPreparation();
            }

            if (competencesManager.IsPreparingCompetence)
            {
                CallUnselectActionEvent(previousActionType);
                competencesManager.InterruptPreparation();
            }

            if (previousActionType != actionType)
            {
                ActionSelectionResult competenceSelectionResult = competencesManager.TrySelectAction(currentActionPointsAmount, actionType);
                if (competenceSelectionResult == ActionSelectionResult.EnoughActionPoints)
                {
                    CallSelectActionEvent(actionType);
                }

                SetActionPointsDebugTextVisibility(competenceSelectionResult == ActionSelectionResult.EnoughActionPoints);
                UpdateActionPointsDebugTextAmount(competencesManager.GetCurrentCompetenceCost());
            }
        }
    }

    public void OnPlayerClickAction()
    {
        if (playerMovementsManager.IsWillingToMove)
        {
            int cost = playerMovementsManager.TryStartMovement(GetCurrentWorldMouseResult.mouseWorldPosition);
            if (cost > 0)
            {
                CallUnselectActionEvent(ActionType.Move);
                SetActionPointsDebugTextVisibility(false);
                ConsumeActionPoints(cost);
            }
        }
        else if(competencesManager.IsPreparingCompetence)
        {
            int cost = competencesManager.GetCurrentCompetenceCost();
            CallUnselectActionEvent(competencesManager.GetCurrentCompetenceType);
            switch (competencesManager.GetCurrentCompetenceType)
            {
                case ActionType.Throw:
                    competencesManager.LaunchThrowCompetence();
                    break;

                case ActionType.Recall:
                    competencesManager.LaunchRecallCompetence();
                    break;

                case ActionType.Special:
                    break;
            }
            ConsumeActionPoints(cost);
        }

        UpdatePlayerActability();
    }

    public void CallSelectActionEvent(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Move:
                OnMoveActionSelectionStateChanged?.Invoke(true);
                break;
            case ActionType.Throw:
                OnThrowCompetenceSelectionStateChanged?.Invoke(true);
                break;
            case ActionType.Recall:
                OnRecallCompetenceSelectionStateChanged?.Invoke(true);
                break;
            case ActionType.Special:
                OnSpecialCompetenceSelectionStateChanged?.Invoke(true);
                break;
        }
    }

    public void CallUnselectActionEvent(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Move:
                OnMoveActionSelectionStateChanged?.Invoke(false);
                break;
            case ActionType.Throw:
                OnThrowCompetenceSelectionStateChanged?.Invoke(false);
                break;
            case ActionType.Recall:
                OnRecallCompetenceSelectionStateChanged?.Invoke(false);
                break;
            case ActionType.Special:
                OnSpecialCompetenceSelectionStateChanged?.Invoke(false);
                break;
        }
    }

    public void UpdatePlayerActability()
    {
        player.SetAbleToAct(!playerMovementsManager.IsUsingMoveSystem && !competencesManager.IsUsingCompetence);
        SetActionPointsDebugTextVisibility(playerMovementsManager.IsWillingToMove || competencesManager.IsPreparingCompetence);
    }
    #endregion
}

public enum ActionType
{
    None, Move, Throw, Recall, Special
}

public enum UsabilityState
{
    None, Preparing, Using
}

public struct WorldMouseResult
{
    public Vector3 mouseWorldPosition;
}

public enum ActionSelectionResult
{
    EnoughActionPoints, NotEnoughActionPoints, NoCompetenceFound
}