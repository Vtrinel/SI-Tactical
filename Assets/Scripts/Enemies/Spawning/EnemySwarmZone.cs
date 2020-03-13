using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarmZone : MonoBehaviour
{
    private void OnEnable()
    {
        if (selfActivates)
            GameManager.Instance.OnPlayerPositionChanged += CheckIfPlayerIsInZone;
    }

    private void OnDisable()
    {
        if (selfActivates)
            GameManager.Instance.OnPlayerPositionChanged -= CheckIfPlayerIsInZone;
    }

    private void OnDrawGizmos()
    {
        if (!debugZone)
            return;

        Gizmos.color = new Color(1, 0, 1, 0.4f);
        Gizmos.DrawSphere(transform.position, radius);
    }

    [SerializeField] bool debugZone = true;

    #region Zone
    [Header("Zone Parameters")]
    [SerializeField] float radius = 5f;
    [SerializeField] bool selfActivates = false;
    [SerializeField] bool alwaysActivatedWhenActivatedOnce = true;

    SwarmZoneState currentState = SwarmZoneState.Inactive;

    public void CheckIfPlayerIsInZone(Vector3 playerPos)
    {
        switch (currentState)
        {
            case SwarmZoneState.Inactive:
                if (GetDistanceWithPlayer(playerPos) <= radius)
                    OnPlayerEntersZone();
                break;

            case SwarmZoneState.PlayerInZone:
                if (GetDistanceWithPlayer(playerPos) > radius)
                    OnPlayerExitsZone();
                break;

            case SwarmZoneState.PlayerOutZoneButEnteredOnce:
                if (GetDistanceWithPlayer(playerPos) <= radius)
                    OnPlayerEntersZone();
                break;
        }
    }

    public float GetDistanceWithPlayer(Vector3 playerPos)
    {
        Vector3 selfPos = transform.position;
        playerPos.y = transform.position.y;

        return Vector3.Distance(playerPos, selfPos);
    }

    public void OnPlayerEntersZone()
    {
        if (!alwaysActivatedWhenActivatedOnce)
            TurnManager.Instance.ActivateSwarmZone(this);

        if (currentState == SwarmZoneState.Inactive)
        {
            OnPlayerEntersZoneFirstTime();
        }
        currentState = SwarmZoneState.PlayerInZone;
    }

    public void OnPlayerEntersZoneFirstTime()
    {
        if (alwaysActivatedWhenActivatedOnce)
            TurnManager.Instance.ActivateSwarmZone(this);
    }

    public void OnPlayerExitsZone()
    {
        if (!alwaysActivatedWhenActivatedOnce)
            TurnManager.Instance.DeactivateSwarmZone(this);
        currentState = SwarmZoneState.PlayerOutZoneButEnteredOnce;
    }
    #endregion

    #region Wave Management
    [Header("Wave Parameters")]
    [SerializeField] List<WaveParameters> allWavesParameters = new List<WaveParameters>();
    [SerializeField] bool infiniteSpawn = true;
    [SerializeField] bool spawnedEnemiesAutoDetectPlayer = true;
    int currentWaveCounter = 0;
    int remainingNumberOfTurnsToWait = 0;

    public void StartWaveTurn()
    {
        if (allWavesParameters.Count == 0)
            return;

        if (remainingNumberOfTurnsToWait > 0)
        {
            remainingNumberOfTurnsToWait--;
        }
        else
        {
            StartWave(allWavesParameters[currentWaveCounter]);

            currentWaveCounter++;
            if (currentWaveCounter >= allWavesParameters.Count)
                currentWaveCounter = 0;
        }
    }

    public void StartWave(WaveParameters waveToStart)
    {
        List<SpawnPointEnemyCouple> enemiesToSpawn = waveToStart.enemiesToSpawn;
        remainingNumberOfTurnsToWait = waveToStart.numberOfWaitTurnAfterSpawned;

        foreach (SpawnPointEnemyCouple couple in enemiesToSpawn)
        {
            if (couple.spawnPoint != null)
                couple.spawnPoint.StartSpawning(couple.enemyType, spawnedEnemiesAutoDetectPlayer, couple.attachedDiscType);
        }

        EndWaveTurn();
    }

    public void EndWaveTurn()
    {
    }
    #endregion
}

public enum SwarmZoneState
{
    Inactive, PlayerInZone, PlayerOutZoneButEnteredOnce
}

[System.Serializable]
public struct WaveParameters
{
    public List<SpawnPointEnemyCouple> enemiesToSpawn;
    public int numberOfWaitTurnAfterSpawned;
}

[System.Serializable]
public struct SpawnPointEnemyCouple
{
    public EnemyType enemyType;
    public EnemySpawnPoint spawnPoint;
    public DiscType attachedDiscType;
}