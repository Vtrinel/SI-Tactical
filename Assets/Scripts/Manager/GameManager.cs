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
        // Update current mouse world result
        currentWorldMouseResult = GetCurrentWorldMouseResult;
        OnWorldMouseResultUpdate?.Invoke(currentWorldMouseResult);

        playerMovementsManager.UpdateSystem();
        competencesManager.UpdateSystem();
    }

    private void LateUpdate()
    {
        calculatedCurrentWorldMouseResult = false;
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
    [SerializeField] PlayerMovementsManager playerMovementsManager = default;
    [SerializeField] CompetencesManager competencesManager = default;

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

    //[Header("Inputs Events")]
    public void SelectAction(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.None:
                break;

            case ActionType.Move:
                if (competencesManager.IsPreparingCompetence)
                    competencesManager.InterruptPreparation();

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
                if (playerMovementsManager.IsWillingToMove)
                    playerMovementsManager.InterruptMovementPreparation();

                if (competencesManager.IsPreparingCompetence && competencesManager.GetCurrentCompetenceType == actionType)
                    competencesManager.InterruptPreparation();
                else
                {
                    ActionSelectionResult throwSelectionResult = competencesManager.TrySelectAction(currentActionPointsAmount, actionType);
                    if (throwSelectionResult != ActionSelectionResult.EnoughActionPoints)
                        competencesManager.InterruptPreparation();

                    SetActionPointsDebugTextVisibility(throwSelectionResult == ActionSelectionResult.EnoughActionPoints);
                    UpdateActionPointsDebugTextAmount(competencesManager.GetCurrentCompetenceCost());
                }
                break;

            case ActionType.Recall:
                if (playerMovementsManager.IsWillingToMove)
                    playerMovementsManager.InterruptMovementPreparation();

                if (competencesManager.IsPreparingCompetence && competencesManager.GetCurrentCompetenceType == actionType)
                    competencesManager.InterruptPreparation();
                else
                {
                    ActionSelectionResult recallSelectionResult = competencesManager.TrySelectAction(currentActionPointsAmount, actionType);
                    if (recallSelectionResult != ActionSelectionResult.EnoughActionPoints)
                        competencesManager.InterruptPreparation();

                    SetActionPointsDebugTextVisibility(recallSelectionResult == ActionSelectionResult.EnoughActionPoints);
                    UpdateActionPointsDebugTextAmount(competencesManager.GetCurrentCompetenceCost());
                }
                break;
        }
    }

    public void OnPlayerClickAction()
    {
        if (playerMovementsManager.IsWillingToMove)
        {
            int cost = playerMovementsManager.TryStartMovement(GetCurrentWorldMouseResult.mouseWorldPosition);
            if (cost > 0)
            {
                SetActionPointsDebugTextVisibility(false);
                ConsumeActionPoints(cost);
            }
        }
        else if(competencesManager.IsPreparingCompetence)
        {
            int cost = competencesManager.GetCurrentCompetenceCost();
            switch (competencesManager.GetCurrentCompetenceType)
            {
                case ActionType.Throw:
                    competencesManager.LaunchThrowCompetence();
                    break;

                case ActionType.Recall:
                    competencesManager.LaunchRecallCompetence();
                    break;
            }
            ConsumeActionPoints(cost);
        }

        UpdatePlayerActability();
    }

    public void UpdatePlayerActability()
    {
        player.SetAbleToAct(!playerMovementsManager.IsUsingMoveSystem && !competencesManager.IsUsingCompetence);
        SetActionPointsDebugTextVisibility(playerMovementsManager.IsWillingToMove || competencesManager.IsPreparingCompetence);
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