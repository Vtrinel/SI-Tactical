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
    TurnState currentTurnState = TurnState.PlayerTurn;
    public TurnState GetCurrentTurnState => currentTurnState;
    public void StartPlayerTurn()
    {
        Debug.Log("Start player turn !");
        currentTurnState = TurnState.PlayerTurn;
        OnStartPlayerTurn?.Invoke();
    }

    public void EndPlayerTurn()
    {
        Debug.Log("End player turn !");
        currentTurnState = TurnState.BetweenPlayerAndEnemies;
        OnEndPlayerTurn?.Invoke();

        StartCoroutine("BetweenTurnsCoroutine");
    }

    public Action OnStartPlayerTurn;
    public Action OnEndPlayerTurn;
    #endregion

    #region Enemies Turn
    List<EnemyBase> orderedInGameEnemies = new List<EnemyBase>();
    public void RefreshEnemyList(List<EnemyBase> allEnemies)
    {
        orderedInGameEnemies = allEnemies;
    }

    int currentEnemiesTurnCounter = 0;
    public void StartEnemiesTurn()
    {
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
        enemy.StartDebugTurn();
    }

    public void EndEnemyTurn(EnemyBase enemy)
    {
        currentEnemiesTurnCounter++;

        if (currentEnemiesTurnCounter == orderedInGameEnemies.Count)
        {
            EndEnemiesTurn();
            return;
        }

        StartCoroutine("BetweenTurnsCoroutine");
    }

    public void EndEnemiesTurn()
    {
        currentTurnState = TurnState.BetweenEnemiesAndPlayer;
        StartCoroutine("BetweenTurnsCoroutine");
    }
    #endregion

    IEnumerator BetweenTurnsCoroutine()
    {
        float waitDuration = 0;

        switch (currentTurnState)
        {
            case TurnState.BetweenPlayerAndEnemies:
                waitDuration = 0.5f;
                break;
            case TurnState.EnemyTurn:
                waitDuration = 0.1f;
                break;
            case TurnState.BetweenEnemiesAndPlayer:
                waitDuration = 0.5f;
                break;
        }

        yield return new WaitForSeconds(waitDuration);


        switch (currentTurnState)
        {
            case TurnState.BetweenPlayerAndEnemies:
                StartEnemiesTurn();
                break;
            case TurnState.EnemyTurn:
                StartEnemyTurn(orderedInGameEnemies[currentEnemiesTurnCounter]);
                break;
            case TurnState.BetweenEnemiesAndPlayer:
                StartPlayerTurn();
                break;
        }
    }
}

public enum TurnState
{
    PlayerTurn, BetweenPlayerAndEnemies, EnemyTurn, BetweenEnemiesAndPlayer
}