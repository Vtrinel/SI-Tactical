using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    PlayerController _player = default;
    PlayerController GetPlayer { get { if (_player == null) _player = GameManager.Instance.GetPlayer; return _player; } }

    private static TurnManager _instance;
    public static TurnManager Instance { get { return _instance; } }

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
    }

    private void Start()
    {
        
    }

    #region Player Turn
    TurnState currentTurnState = TurnState.GameNotStarted;
    public TurnState GetCurrentTurnState => currentTurnState;
    public void StartPlayerTurn()
    {
        if (currentTurnState == TurnState.Won || currentTurnState == TurnState.Lost)
            return;

            currentTurnState = TurnState.PlayerTurn;
        OnStartPlayerTurn?.Invoke();

        if (CameraManager.instance != null)
            CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();

        UIManager.Instance.PlayStartTurnAnimation(currentTurnState, enemiesSpawnedThisTurn, currentlyPendingSpawnPoints.Count > 0);
    }

    public void EndPlayerTurn()
    {
        currentTurnState = TurnState.EnemyTurn;
        OnEndPlayerTurn?.Invoke();

        currentEnemiesTurnCounter = 0;
        StartCoroutine("BetweenTurnsCoroutine");
    }

    public Action OnStartPlayerTurn;
    public Action OnEndPlayerTurn;

    public Action OnStartEnemyTurn;
    public Action OnEndEnemyTurn;
    #endregion

    #region Enemies Turn
    List<EnemyBase> orderedInGameEnemies = new List<EnemyBase>();
    public void RefreshEnemyList(List<EnemyBase> allEnemies)
    {
        orderedInGameEnemies = allEnemies;
    }

    int currentEnemiesTurnCounter = 0;
    EnemyBase currentTurnEnemy = default;
    bool oneEnemyActed= false;
    public void StartEnemiesTurn()
    {
        oneEnemyActed = false;
        if(orderedInGameEnemies.Count == 0)
        {
            currentTurnState = TurnState.ProgressionTurn;
            StartProgressionTurn();
            return;
        }

        currentEnemiesTurnCounter = 0;

        if (orderedInGameEnemies.Count == 0)
        {
            EndEnemiesTurn();
            return;
        }

        currentTurnState = TurnState.EnemyTurn;
        StartEnemyTurn(orderedInGameEnemies[0]);
    }

    public void StartEnemyTurn(EnemyBase enemy)
    {
        currentTurnEnemy = enemy;

        enemy.StartTurn();

        if (enemy.GetPlayerDetected)
        {
            CameraManager.instance.GetPlayerCamera.AttachFollowTransformTo(enemy.transform);
            if (!oneEnemyActed)
            {
                oneEnemyActed = true;
                UIManager.Instance.PlayStartTurnAnimation(currentTurnState, false, false);
            }
        }
    }

    public void EndEnemyTurn(EnemyBase enemy, bool playedItsTurn)
    {
        currentTurnEnemy = null;
        currentEnemiesTurnCounter++;

        if (currentEnemiesTurnCounter == orderedInGameEnemies.Count)
        {
            EndEnemiesTurn();
            return;
        }

        if (playedItsTurn)
            StartCoroutine("BetweenTurnsCoroutine");
        else
            StartEnemyTurn(orderedInGameEnemies[currentEnemiesTurnCounter]);
    }

    public Action OnEnemyTurnInterruption;
    public void InterruptEnemiesTurn()
    {
        OnEnemyTurnInterruption?.Invoke();
        currentEnemiesTurnCounter = orderedInGameEnemies.Count;
        currentTurnEnemy = null;

        EndEnemiesTurn();
    }

    public void EndEnemiesTurn()
    {
        //currentTurnState = TurnState.BetweenEnemiesAndSpawnPoints;
        currentTurnState = TurnState.ProgressionTurn;
        StartCoroutine("BetweenTurnsCoroutine");
    }
    #endregion

    #region Progression Turn
    public void StartProgressionTurn()
    {
        bool progressed = LevelProgressionManager.Instance.CheckForProgressTurn();

        if (!progressed)
        {
            currentTurnState = TurnState.SpawnPointsTurn;
            StartSpawnPointsTurn();
            return;
        }

        //Instant Call, might be called on Level Manager
        //EndProgressionTurn();
    }

    public void EndProgressionTurn()
    {
        if (currentTurnState == TurnState.Won || currentTurnState == TurnState.Lost)
            return;

        currentTurnState = TurnState.SpawnPointsTurn;
        StartCoroutine("BetweenTurnsCoroutine");

        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
    }
    #endregion

    #region Pending Spawn Points Turn
    bool enemiesSpawnedThisTurn = false;
    List<EnemySpawnPoint> currentlyPendingSpawnPoints = new List<EnemySpawnPoint>();
    public void AddPendingSpawnPoint(EnemySpawnPoint newSpawnPoint)
    {
        if (!currentlyPendingSpawnPoints.Contains(newSpawnPoint))
            currentlyPendingSpawnPoints.Add(newSpawnPoint);
    }

    public void StartSpawnPointsTurn()
    {
        enemiesSpawnedThisTurn = false;

        if (currentlyPendingSpawnPoints.Count == 0)
        {
            currentTurnState = TurnState.SpawnPointsTurn;
            StartSwarmZonesTurn();
            return;
        }

        enemiesSpawnedThisTurn = true;

        foreach (EnemySpawnPoint pendingSpawnPoint in currentlyPendingSpawnPoints)
        {
            pendingSpawnPoint.SpawnPendingEnemy();
        }

        currentlyPendingSpawnPoints = new List<EnemySpawnPoint>();

        EndSpawnPointsTurn();
    }

    public void EndSpawnPointsTurn()
    {
        currentTurnState = TurnState.SwarmZonesTurn;
        StartCoroutine("BetweenTurnsCoroutine");
    }
    #endregion

    #region Active Swarm Zone Turn
    List<EnemySwarmZone> currentlyActiveSwarmZones = new List<EnemySwarmZone>();
    public void ActivateSwarmZone(EnemySwarmZone newSwarmZone)
    {
        if (!currentlyActiveSwarmZones.Contains(newSwarmZone))
            currentlyActiveSwarmZones.Add(newSwarmZone);
    }
    public void DeactivateSwarmZone(EnemySwarmZone swarmZoneToRemove)
    {
        if (!currentlyActiveSwarmZones.Contains(swarmZoneToRemove))
            currentlyActiveSwarmZones.Add(swarmZoneToRemove);
    }

    public void StartSwarmZonesTurn()
    {
        if (currentlyActiveSwarmZones.Count == 0)
        {
            currentTurnState = TurnState.PlayerTurn;
            StartPlayerTurn();
            return;
        }

        foreach (EnemySwarmZone swarmZone in currentlyActiveSwarmZones)
        {
            swarmZone.StartWaveTurn();
        }

        // Instantly called
        EndSwarmZonesTurn();
    }

    public void EndSwarmZonesTurn()
    {
        currentTurnState = TurnState.PlayerTurn;
        StartCoroutine("BetweenTurnsCoroutine");
    }
    #endregion

    IEnumerator BetweenTurnsCoroutine()
    {
        float waitDuration = 0;

        switch (currentTurnState)
        {
            case TurnState.PlayerTurn:
                waitDuration = 0.5f;
                break;
            case TurnState.EnemyTurn:
                Debug.Log("Wait enemy turn");
                waitDuration = (currentEnemiesTurnCounter == 0 ? 0.5f : 0.1f);
                break;
            case TurnState.ProgressionTurn:
                waitDuration = 0.5f;
                break;
            case TurnState.SpawnPointsTurn:
                waitDuration = 0.5f;
                break;
            case TurnState.SwarmZonesTurn:
                waitDuration = 0.5f;
                break;
        }

        yield return new WaitForSeconds(waitDuration);

        switch (currentTurnState)
        {
            case TurnState.PlayerTurn:
                StartPlayerTurn();
                break;
            case TurnState.EnemyTurn:
                if (currentEnemiesTurnCounter == 0)
                    StartEnemiesTurn();
                else if (currentEnemiesTurnCounter == orderedInGameEnemies.Count)
                    StartProgressionTurn();
                else
                    StartEnemyTurn(orderedInGameEnemies[currentEnemiesTurnCounter]);
                break;
            case TurnState.ProgressionTurn:
                StartProgressionTurn();
                break;
            case TurnState.SpawnPointsTurn:
                StartSpawnPointsTurn();
                break;
            case TurnState.SwarmZonesTurn:
                StartSwarmZonesTurn();
                break;
        }
    }

    public void WonGame()
    {
        currentTurnState = TurnState.Won;
    }

    public void LostGame()
    {
        currentTurnState = TurnState.Won;
    }
}

public enum TurnState
{
    PlayerTurn, 
    EnemyTurn, 
    ProgressionTurn,
    SpawnPointsTurn, 
    SwarmZonesTurn, 
    GameNotStarted, Won, Lost
}