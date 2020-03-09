using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    /// WARNING : The current spawning system will never check if the spawning position is taken by a disc, the player or an enemy. 
    /// This should be improved if possible

    [SerializeField] EnemyType definedEnemyToSpawn = EnemyType.None;

    public void StartSpawning()
    {
        if (definedEnemyToSpawn == EnemyType.None)
            return;
        StartSpawning(definedEnemyToSpawn);
    }

    EnemyType nextSpawnEnemyType = EnemyType.None;
    public void StartSpawning(EnemyType enemyTypeToSpawn)
    {
        if (enemyTypeToSpawn == EnemyType.None)
            return;
        nextSpawnEnemyType = enemyTypeToSpawn;

        /// NEXT STEP : add to spawn points to check on next player turn start + generate a preview
        /// For now, just spawn instant
        SpawnEnemyOnSpawnPoint(nextSpawnEnemyType);
    }

    public void SpawnEnemyOnSpawnPoint(EnemyType enemyTypeToSpawn)
    {
        EnemiesManager.Instance.SpawnEnemyAtPosition(enemyTypeToSpawn, transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
