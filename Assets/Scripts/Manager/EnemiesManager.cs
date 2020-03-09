using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    private static EnemiesManager _instance;
    public static EnemiesManager Instance { get { return _instance; } }

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

    [Header("Enemies in Game")]
    List<EnemyBase> allInGameEnemiesOrdered = new List<EnemyBase>();
    public Action<List<EnemyBase>> OnInGameEnemiesChanged;

    public void AddEnemy(EnemyBase newEnemy)
    {
        int insertionCounter = 0;
        float newEnemyInitiative = newEnemy.GetEnemyInitiative;

        foreach(EnemyBase enemy in allInGameEnemiesOrdered)
        {
            float init = enemy.GetEnemyInitiative;
            if (newEnemyInitiative > init)
                break;

            insertionCounter++;
        }

        if (insertionCounter < allInGameEnemiesOrdered.Count)
            allInGameEnemiesOrdered.Insert(insertionCounter, newEnemy);
        else
            allInGameEnemiesOrdered.Add(newEnemy);

        newEnemy.OnEnemyDeath += RemoveEnemy;
        OnInGameEnemiesChanged?.Invoke(allInGameEnemiesOrdered);
    }

    public void RemoveEnemy(EnemyBase enemyToRemove)
    {
        enemyToRemove.OnEnemyDeath -= RemoveEnemy;
        allInGameEnemiesOrdered.Remove(enemyToRemove);

        OnInGameEnemiesChanged?.Invoke(allInGameEnemiesOrdered);
    }

    public void GetAllAlreadyPlacedEnemies()
    {
        allInGameEnemiesOrdered = new List<EnemyBase>();
        EnemyBase[] alreadyPlacedEnemies = FindObjectsOfType<EnemyBase>();

        foreach(EnemyBase enemy in alreadyPlacedEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            enemy.SetUpInitiative();
            AddEnemy(enemy);
        }
    }

    /// For now, the spawning system isn't really a pool system, but I created the methods so it will be easy to create a pool system
    [Header("Spawning System")]
    [SerializeField] List<EnemyPoolParameters> allEnemyPoolParameters = new List<EnemyPoolParameters>();

    public EnemyBase GetEnemyFromPool(EnemyType enemyType)
    {
        foreach(EnemyPoolParameters enemyPoolParameters in allEnemyPoolParameters)
        {
            if(enemyPoolParameters.enemyType == enemyType)
            {
                EnemyBase newEnemy = Instantiate(enemyPoolParameters.enemyPrefab, transform);

                return newEnemy;
            }
        }

        return null;
    }

    public void ReturnEnemyInPool(EnemyBase enemy)
    {
        EnemyType returnedEnemyType = enemy.GetEnemyType;

        Destroy(enemy.gameObject);
    }

    public EnemyBase SpawnEnemyAtPosition(EnemyType enemyType, Vector3 position)
    {
        EnemyBase newEnemy = GetEnemyFromPool(enemyType);
        if (newEnemy == null)
            return null;

        newEnemy.SpawnEnemy(position);
        AddEnemy(newEnemy);

        return newEnemy;
    }

    public void DestroyEnemy(EnemyBase enemy)
    {
        RemoveEnemy(enemy);
        ReturnEnemyInPool(enemy);
    }
}

[System.Serializable]
public struct EnemyPoolParameters
{
    public EnemyType enemyType;
    public EnemyBase enemyPrefab;
    public int baseNumberOfElements;
}

public enum EnemyType
{
    None, TouniBase, TouniShield, TouniChaser, TouniChaserShield, CultistBase, CultistChaser
}