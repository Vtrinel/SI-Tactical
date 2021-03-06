﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawnPoint : MonoBehaviour
{
    /// WARNING : The current spawning system will never check if the spawning position is taken by a disc, the player or an enemy. 
    /// This should be improved if possible

    [SerializeField] EnemyType definedEnemyToSpawn = EnemyType.None;
    [SerializeField] bool definedEnemyWillAutoDetectPlayer = true;
    [SerializeField] DiscType definedLootedDisc = DiscType.None;
    //[SerializeField] GameObject spawnZoneDebugObject = default;
    [SerializeField] TooltipCollider tooltipCollider = default;
    private GameObject fxGameObject;

    private void Start()
    {
        //spawnZoneDebugObject?.SetActive(false);
        tooltipCollider.gameObject.SetActive(false);
    }

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

        TurnManager.Instance.AddPendingSpawnPoint(this);
        //spawnZoneDebugObject.SetActive(true);
        fxGameObject = FxManager.Instance.SendFx(FxType.enemySpawnPreparation, gameObject.transform.position + Vector3.up * 0.01f);
        tooltipCollider.gameObject.SetActive(true);
        fxGameObject.SetActive(true);
    }

    public void SpawnPendingEnemy()
    {
        SpawnEnemyOnSpawnPoint(spawnPendingEnemyType, spawnPendingWillAutoDetectPlayer, spawnPendingLootedDiscType);
        Destroy(fxGameObject);
    }

    public void SpawnEnemyOnSpawnPoint(EnemyType enemyTypeToSpawn, bool autoDetectPlayer, DiscType lootedDiscType)
    {
        EnemyBase enemyBase = EnemiesManager.Instance.SpawnEnemyAtPosition(enemyTypeToSpawn, transform.position + Vector3.up * 0.05f, lootedDiscType);
        if(enemyBase != null)
        {
            enemyBase.SetPlayerDetected(autoDetectPlayer);
            enemyBase.myIA.myNavAgent.Warp(enemyBase.transform.position);
            //enemyBase.myIA.myNavAgent.enabled = false;
        }

        FxManager.Instance.CreateFx(FxType.enemySpawn, gameObject.transform.position);
        //spawnZoneDebugObject.SetActive(false);
        tooltipCollider.gameObject.SetActive(false);
    }

    IEnumerator DebugCoroutine(EnemyBase enemy)
    {
        yield return new WaitForSeconds(0.01f);
        enemy.myIA.myNavAgent.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
