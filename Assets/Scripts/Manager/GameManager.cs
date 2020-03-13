using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header(" Debug")]
    [SerializeField] Transform debugPosition = default;
    public Vector3 GetDebugPos => debugPosition.position;

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
            _instance = this;
        }

        playerMovementsManager.SetUp(player);
        competencesUsabilityManager.SetUp(player);        

        OnWorldMouseResultUpdate += playerMovementsManager.UpdateCurrentWorldMouseResult;
        OnWorldMouseResultUpdate += competencesUsabilityManager.UpdateCurrentWorldMouseResult;

        playerMovementsManager.OnPreparationAmountChanged += UpdateActionPointsDebugTextAmount;
        competencesUsabilityManager.OnCompetenceStateChanged += UpdatePlayerActability;

        player.OnPlayerReachedMovementDestination += UpdatePlayerActability;

        ResetActionPointsCount();

        turnManager.OnStartPlayerTurn += StartPlayerTurn;
        turnManager.OnEndPlayerTurn += EndPlayerTurn;

        enemiesManager.OnInGameEnemiesChanged += turnManager.RefreshEnemyList;
        enemiesManager.GetAllAlreadyPlacedEnemies();

        levelManager.OnGoalReached += WinGame;

        competencesUsabilityManager.OnRecallCompetenceChanged += playerMovementsManager.UpdateCurrentRecallCompetence;

        playerExperienceManager.OnSetChanged += competencesUsabilityManager.UpdateSet;
        playerExperienceManager.OnMenuOpenedOrClosed += UpdatePlayerActability;
        playerExperienceManager.SetUp();        
    }

    private void Start()
    {
        SetUpGame();
    }

    private void Update()
    {
        currentWorldMouseResult = GetCurrentWorldMouseResult;
        OnWorldMouseResultUpdate?.Invoke(currentWorldMouseResult);

        playerMovementsManager.UpdateSystem();
        competencesUsabilityManager.UpdateSystem();

        UpdateGameManagement();

        tooltipsManagementSystem.UpdateSystem(currentWorldMouseResult.currentTooltipable);

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            playerExperienceManager.GainGold(20);
    }

    private void LateUpdate()
    {
        calculatedCurrentWorldMouseResult = false;
    }
    #endregion

    #region Turn Management
    public void StartPlayerTurn()
    {
        ResetActionPointsCount();
        playerMovementsManager.ResetActionPointsUsedThisTurn();
        UpdatePlayerActability();
    }

    public void EndPlayerTurn()
    {
        SelectAction(ActionType.None);
        UpdatePlayerActability();
    }
    #endregion

    [Header("Important References")]
    [SerializeField] PlayerController player = default;
    public PlayerController GetPlayer => player;

    [SerializeField] TurnManager turnManager = default;
    [SerializeField] EnemiesManager enemiesManager = default;
    [SerializeField] LevelProgressionManager levelManager = default;
    [SerializeField] PlayerExperienceManager playerExperienceManager = default;

    public bool OnMouseInUI = false;

    public void SetOnMouseInUI(bool value)
    {
        OnMouseInUI = value;
    }

    #region Action Points
    [Header("Action Points")]
    public int maxActionPointsAmount = 10;
    [SerializeField] int currentActionPointsAmount;
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

        CheckForCompetencesUsability();
    }

    public void GainActionPoints(int amount)
    {
        currentActionPointsAmount += amount;
        if (currentActionPointsAmount > maxActionPointsAmount)
        {
            currentActionPointsAmount = maxActionPointsAmount;
        }

        OnActionPointsAmountChanged?.Invoke(currentActionPointsAmount);
        CheckForCompetencesUsability();
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
    [SerializeField] CompetencesUsabilityManager competencesUsabilityManager = default;

    public CompetencesUsabilityManager GetCompetencesUsabilityManager() => competencesUsabilityManager;

    public Competence GetCurrentlySelectedCompetence => competencesUsabilityManager.GetCurrentCompetence;
    public Action<bool> OnMoveActionSelectionStateChanged;
    public Action<bool> OnThrowCompetenceSelectionStateChanged;
    public Action<bool> OnRecallCompetenceSelectionStateChanged;
    public Action<bool> OnSpecialCompetenceSelectionStateChanged;

    public Action<Vector3> OnPlayerPositionChanged;

    public Action<int> OnPlayerLifeAmountChanged;
    public Action<int> OnPlayerMaxLifeAmountChanged;
    public int maxPlayerLifeAmount = 3;
    [SerializeField] int currentPlayerLifeAmount;
    public int GetCurrentPlayerLifeAmount => currentPlayerLifeAmount;

    public void PlayerLifeChange(int value)
    {
        currentPlayerLifeAmount = value;
        OnPlayerLifeAmountChanged?.Invoke(value);
    }

    public void PlayerMaxLifeChange(int value)
    {
        GetPlayer.damageReceiptionSystem.AddLifeBar(value);
        maxPlayerLifeAmount += value;
        OnPlayerMaxLifeAmountChanged?.Invoke(value);
    }

    public Action<int> OnCancelSelectedAction;

    public void SendRemainingActionPoint()
    {
        OnCancelSelectedAction?.Invoke(currentActionPointsAmount);
    }

    #region Mouse World Result
    [Header("Mouse World Result")]
    [SerializeField] LayerMask worldMouseLayerMask = default;
    [SerializeField] float mouseCheckMaxDistance = 50.0f;
    [SerializeField] UIRaycaster uiRaycaster = default;
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
        ITooltipable foundTooltipable = uiRaycaster.CheckForUITooltipable();
        //OnMouseInUI = (foundTooltipable != null);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.layer == 8)
            {
                result.mouseWorldPosition = hit.point;
            }
            else
            {
                if (!OnMouseInUI)
                {
                    if (foundTooltipable == null && hit.collider.gameObject.layer == 15)
                    {
                        foundTooltipable = hit.collider.GetComponent<ITooltipable>();
                    }
                }
            }
        }

        result.mouseIsOnUI = OnMouseInUI;
        result.currentTooltipable = foundTooltipable;

        return result;
    }

    System.Action<WorldMouseResult> OnWorldMouseResultUpdate = default;
    #endregion

    #region Player Inputs Reception
    public void SelectAction(ActionType actionType)
    {
        if (!GetPlayerCanAct)
            return;

        if (actionType == ActionType.None)
        {
            if (competencesUsabilityManager.IsPreparingCompetence)
            {
                CallUnselectActionEvent(competencesUsabilityManager.GetCurrentCompetenceType);
                competencesUsabilityManager.InterruptPreparation();
            }

            if (playerMovementsManager.IsWillingToMove)
            {
                CallUnselectActionEvent(ActionType.Move);
                playerMovementsManager.InterruptMovementPreparation();
            }
        }

        if (actionType == ActionType.Move)
        {
            if (competencesUsabilityManager.IsPreparingCompetence)
            {
                CallUnselectActionEvent(competencesUsabilityManager.GetCurrentCompetenceType);
                competencesUsabilityManager.InterruptPreparation();
            }

            if (!playerMovementsManager.IsWillingToMove)
            {
                if(currentActionPointsAmount == 0)
                {
                    Debug.Log("Not enough AP to move");
                    SoundManager.Instance.PlaySound(Sound.NotEnoughActionPoint, Camera.main.transform.position);
                    return;
                }

                SoundManager.Instance.PlaySound(Sound.SelectCompetence, Camera.main.transform.position);
                CallSelectActionEvent(ActionType.Move);
                playerMovementsManager.StartMovementPreparation(currentActionPointsAmount);
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
            ActionType previousActionType = competencesUsabilityManager.GetCurrentCompetenceType;

            if (playerMovementsManager.IsWillingToMove)
            {
                CallUnselectActionEvent(ActionType.Move);
                playerMovementsManager.InterruptMovementPreparation();
            }

            if (competencesUsabilityManager.IsPreparingCompetence)
            {
                CallUnselectActionEvent(previousActionType);
                competencesUsabilityManager.InterruptPreparation();
            }

            if (previousActionType != actionType)
            {
                ActionSelectionResult competenceSelectionResult = competencesUsabilityManager.TrySelectAction(currentActionPointsAmount, actionType);
                if (competenceSelectionResult == ActionSelectionResult.EnoughActionPoints)
                {
                    CallSelectActionEvent(actionType);
                }

                SetActionPointsDebugTextVisibility(competenceSelectionResult == ActionSelectionResult.EnoughActionPoints);
                UpdateActionPointsDebugTextAmount(competencesUsabilityManager.GetCurrentCompetenceCost());
            }
        }

        CheckForCompetencesUsability();
    }

    public void OnPlayerClickAction()
    {
        if (OnMouseInUI)
        {
            Debug.Log("Avoided action validation because mouse is on UI");
            return;
        }

        bool validatedAction = true;

        ActionType unselectedActionType = competencesUsabilityManager.GetCurrentCompetenceType;
        if (playerMovementsManager.IsWillingToMove)
        {
            int cost = playerMovementsManager.TryStartMovement(GetCurrentWorldMouseResult.mouseWorldPosition);
            if (cost > 0 && cost <= currentActionPointsAmount)
            {
                CallUnselectActionEvent(ActionType.Move);
                SetActionPointsDebugTextVisibility(false);
                ConsumeActionPoints(cost);
                SoundManager.Instance.PlaySound(Sound.PlayerMovement, player.transform.position);
            }
        }
        else if(competencesUsabilityManager.IsPreparingCompetence)
        {
            int cost = competencesUsabilityManager.GetCurrentCompetenceCost();
            switch (competencesUsabilityManager.GetCurrentCompetenceType)
            {
                case ActionType.Throw:
                    competencesUsabilityManager.LaunchThrowCompetence(player.gameObject);
                    break;

                case ActionType.Recall:
                    competencesUsabilityManager.LaunchRecallCompetence();
                    break;

                case ActionType.Special:
                    validatedAction = competencesUsabilityManager.LaunchSpecialCompetence();

                    break;
            }
            if (validatedAction)
            {
                ConsumeActionPoints(cost);
                CallUnselectActionEvent(unselectedActionType);
            }
        }

        if(validatedAction)
            UpdatePlayerActability();

        CheckForCompetencesUsability();
    }

    public void CallSelectActionEvent(ActionType actionType)
    {
        UIManager.Instance.ChangeCancelButtonVisibility(true);
        UIManager.Instance.ChangeEndTurnButtonVisibility(false);
        UIManager.Instance.RestartEndTurnButton();
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
        UIManager.Instance.GetActionBar.UpdatePreConsommationPointBar(currentActionPointsAmount, 0);
        UIManager.Instance.HideActionPointText();
        UIManager.Instance.ChangeCancelButtonVisibility(false);
        UIManager.Instance.ChangeEndTurnButtonVisibility(GetPlayerCanAct);
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

    public bool GetPlayerCanAct
    {
        get
        {
            return !playerMovementsManager.IsMoving
            &&
            !competencesUsabilityManager.IsUsingCompetence
            &&
            turnManager.GetCurrentTurnState == TurnState.PlayerTurn
            &&
            !playerExperienceManager.IsUsingCompetencesMenu
            &&
            gameStarted && !gameWon && !gameLost;
        }
    }
    public void UpdatePlayerActability()
    {
        bool canAct = GetPlayerCanAct;

        UIManager.Instance.ChangeEndTurnButtonVisibility(canAct);

        player.SetAbleToAct(canAct);
        SetActionPointsDebugTextVisibility(playerMovementsManager.IsWillingToMove || competencesUsabilityManager.IsPreparingCompetence);
        CheckForCompetencesUsability();
    }

    public void LaunchCompetenceForReal()
    {
        competencesUsabilityManager.LaunchCompetenceForReal();
        CheckForCompetencesUsability();
    }
    #endregion

    #region Game Management
    bool gameStarted = false;
    bool gameWon = false;
    bool gameLost = false;
    [Header("Game Management")]
    [SerializeField] KeyCode gameManagementActionKey = KeyCode.Return;
    [SerializeField] bool playIntroCutscene = false;
    [SerializeField] float cutsceneStayOnObjectiveDuration = 2f;
    [SerializeField] float cutsceneObjectiveToPlayerDuration = 4f;
    [SerializeField] AnimationCurve cutsceneCameraMovementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void SetUpGame()
    {
        if (playIntroCutscene)
        {
            CameraManager.instance.GetPlayerCamera.InstantPlaceCameraOnTransform(levelManager.GetGoalZone.transform);
            StartCoroutine(IntroCutscene());
        }
        else
        {
            CameraManager.instance.GetPlayerCamera.InstantPlaceCameraOnTransform(player.transform);
            StartGame();
        }
    }
    IEnumerator IntroCutscene()
    {
        yield return new WaitForSeconds(cutsceneStayOnObjectiveDuration);
        CameraManager.instance.GetPlayerCamera.StartMovementToward(player.transform, cutsceneObjectiveToPlayerDuration, cutsceneCameraMovementCurve);
        CameraManager.instance.GetPlayerCamera.SetUpEndMovementEvent(StartGame);
    }

    public void UpdateGameManagement()
    {
        if (gameWon || gameLost)
        {
            if (Input.GetKeyDown(gameManagementActionKey))
                RestartGame();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        UIManager.Instance.HideStartPanel();
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        yield return new WaitForEndOfFrame();
        turnManager.StartPlayerTurn();
        levelManager.SetUpGoalAnimation();
    }

    bool restarting = false;
    public void RestartGame()
    {
        if (restarting) return;
        restarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void WinGame()
    {
        //Debug.Log("YOU WIN");
        gameWon = true;
        turnManager.WonGame();
        UIManager.Instance.ShowWinPanel();
    }

    public void LoseGame()
    {
        //Debug.Log("YOU LOSE");
        gameLost = true;
        turnManager.InterruptEnemiesTurn();
        turnManager.LostGame();
        UIManager.Instance.ShowLosePanel();
    }
    #endregion

    public Action<List<bool>> OnCompetencesUsableChanged;
    public void CheckForCompetencesUsability()
    {
        List<bool> usabilities = new List<bool>();

        if (currentActionPointsAmount == 0 || !GetPlayerCanAct)
        {
            for (int i = 0; i < 4; i++)
            {
                usabilities.Add(false);
            }
        }
        else
        {
            usabilities = competencesUsabilityManager.GetCompetencesUsability(true, currentActionPointsAmount);
        }

        OnCompetencesUsableChanged?.Invoke(usabilities);
    }

    [Header("Tooltips management")]
    [SerializeField] TooltipsManagementSystem tooltipsManagementSystem = default; 
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
    public bool mouseIsOnUI;
    public ITooltipable currentTooltipable;
}

public enum ActionSelectionResult
{
    EnoughActionPoints, NotEnoughActionPoints, NoCompetenceFound, NotEnoughDiscs, NoNearbyDisc
}