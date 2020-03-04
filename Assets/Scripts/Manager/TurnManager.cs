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

    TurnState currentTurnState = TurnState.BetweenTurns;
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
        currentTurnState = TurnState.BetweenTurns;
        OnEndPlayerTurn?.Invoke();

        StartCoroutine("BetweenTurnsCoroutine");
    }

    public Action OnStartPlayerTurn;
    public Action OnEndPlayerTurn;

    IEnumerator BetweenTurnsCoroutine()
    {
        Debug.Log("Wait for next turn...");
        yield return new WaitForSeconds(0.5f);
        StartPlayerTurn();
    }
}

public enum TurnState
{
    PlayerTurn, EnemyTurn, EnvironmentTurn, BetweenTurns
}