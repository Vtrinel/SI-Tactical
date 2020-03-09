using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    /// WARNING : The current spawning system will never check if the spawning position is taken by a disc, the player or an enemy. 
    /// This should be improved if possible

    [SerializeField] EnemyType definedEnemyToSpawn = EnemyType.None;
    [SerializeField] bool definedEnemyWillAutoDetectPlayer = true;
    [SerializeField] GameObject spawnZoneDebugObject = default;

    public void StartSpawning()
    {
        if (definedEnemyToSpawn == EnemyType.None)
            return;
        StartSpawning(definedEnemyToSpawn, definedEnemyWillAutoDetectPlayer);
    }

    EnemyType spawnPendingEnemyType = EnemyType.None;
    bool spawnPendingWillAutoDetectPlayer;
    public void StartSpawning(EnemyType enemyTypeToSpawn, bool autoDetectPlayer)
    {
        if (enemyTypeToSpawn == EnemyType.None)
            return;
        spawnPendingEnemyType = enemyTypeToSpawn;
        spawnPendingWillAutoDetectPlayer = autoDetectPlayer;

        /// NEXT STEP : add to spawn points to check on next player turn start + generate a preview
        /// For now, just spawn instant
        TurnManager.Instance.AddPendingSpawnPoint(this);
        spawnZoneDebugObject.SetActive(true);
    }

    public void SpawnPendingEnemy()
    {
        SpawnEnemyOnSpawnPoint(spawnPendingEnemyType, spawnPendingWillAutoDetectPlayer);
    }

    public void SpawnEnemyOnSpawnPoint(EnemyType enemyTypeToSpawn, bool autoDetectPlayer)
    {
        EnemyBase enemyBase = EnemiesManager.Instance.SpawnEnemyAtPosition(enemyTypeToSpawn, transform.position);
        if(enemyBase != null)
        {
            enemyBase.SetPlayerDetected(autoDetectPlayer);
        }

        spawnZoneDebugObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
