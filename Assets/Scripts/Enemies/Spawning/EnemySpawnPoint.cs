using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    /// WARNING : The current spawning system will never check if the spawning position is taken by a disc, the player or an enemy. 
    /// This should be improved if possible

    [SerializeField] EnemyType definedEnemyToSpawn = EnemyType.None;
    [SerializeField] bool definedEnemyWillAutoDetectPlayer = true;
    [SerializeField] DiscType definedLootedDisc = DiscType.None;
    [SerializeField] GameObject spawnZoneDebugObject = default;

    public void StartSpawning()
    {
        if (definedEnemyToSpawn == EnemyType.None)
            return;
        StartSpawning(definedEnemyToSpawn, definedEnemyWillAutoDetectPlayer, definedLootedDisc);
    }

    EnemyType spawnPendingEnemyType = EnemyType.None;
    bool spawnPendingWillAutoDetectPlayer;
    DiscType spawnPendingLootedDiscType = DiscType.None;
    public void StartSpawning(EnemyType enemyTypeToSpawn, bool autoDetectPlayer, DiscType lootedDiscType)
    {
        if (enemyTypeToSpawn == EnemyType.None)
            return;
        spawnPendingEnemyType = enemyTypeToSpawn;
        spawnPendingWillAutoDetectPlayer = autoDetectPlayer;
        spawnPendingLootedDiscType = lootedDiscType;

        /// NEXT STEP : add to spawn points to check on next player turn start + generate a preview
        /// For now, just spawn instant
        TurnManager.Instance.AddPendingSpawnPoint(this);
        spawnZoneDebugObject.SetActive(true);
    }

    public void SpawnPendingEnemy()
    {
        SpawnEnemyOnSpawnPoint(spawnPendingEnemyType, spawnPendingWillAutoDetectPlayer, spawnPendingLootedDiscType);
    }

    public void SpawnEnemyOnSpawnPoint(EnemyType enemyTypeToSpawn, bool autoDetectPlayer, DiscType lootedDiscType)
    {
        EnemyBase enemyBase = EnemiesManager.Instance.SpawnEnemyAtPosition(enemyTypeToSpawn, transform.position, lootedDiscType);
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
