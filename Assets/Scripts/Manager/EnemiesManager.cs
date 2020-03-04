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

        OnInGameEnemiesChanged?.Invoke(allInGameEnemiesOrdered);
    }

    public void RemoveEnemy(EnemyBase newEnemy)
    {
        allInGameEnemiesOrdered.Remove(newEnemy);

        OnInGameEnemiesChanged?.Invoke(allInGameEnemiesOrdered);
    }

    public void GetAllAlreadyPlacedEnemies()
    {
        allInGameEnemiesOrdered = new List<EnemyBase>();
        EnemyBase[] alreadyPlacedEnemies = FindObjectsOfType<EnemyBase>();

        foreach(EnemyBase enemy in alreadyPlacedEnemies)
        {
            enemy.SetUpInitiative();
            AddEnemy(enemy);
        }
    }
}
