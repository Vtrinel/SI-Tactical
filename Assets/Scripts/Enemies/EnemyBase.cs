﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] EnemyType enemyType = EnemyType.TouniBase;
    public EnemyType GetEnemyType => enemyType;
    public int goldGain = 10;

    private void Start()
    {
        SpawnEnemy(transform.position, _lootedDiscType);
        InitLifeBar(damageReceiptionSystem.GetCurrentLifeAmount);
    }

    [Header("References")]
    [SerializeField] DamageableEntity damageReceiptionSystem = default;
    [SerializeField] KnockbackableEntity knockbackReceiptionSystem = default;
    [SerializeField] Transform lifeBarParent;
    [SerializeField] GameObject lifeBarEnemyPrefab;
    List<Image> lifeBarList = new List<Image>();

    void InitLifeBar(int lifeNumber)
    {
        for(int i=0; i < lifeNumber; i++)
        {
            lifeBarList.Add(Instantiate(lifeBarEnemyPrefab, lifeBarParent).GetComponent<Image>());
        }
    }

    public void UpdateLifeBarFill(int currentAmount, int damageDelta)
    {
        int i = 1;
        foreach(Image bar in lifeBarList)
        {
            bar.enabled = (currentAmount <= i);
            i++;
        }
    }

    public Action<EnemyBase> OnEnemyDeath;
    public void Die()
    {
        CheckForLootedDisc();

        Debug.Log(name + " (Enemy) is dead");
        spawned = false;
        setedUpInitiative = false;
        
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);
        PlayerExperienceManager.Instance.GainGold(goldGain);
    }   

    [Header("Common Values")]
    [SerializeField] int baseInitiative = 1;
    float enemyInstanceInitiative = 1;
    public float GetEnemyInitiative => enemyInstanceInitiative;
    bool setedUpInitiative = false;

    [Header("Loot")]
    [SerializeField] DiscType _lootedDiscType = DiscType.None;
    [SerializeField] GameObject lootDiscIndicator = default;

    public void CheckForLootedDisc()
    {
        if(_lootedDiscType != DiscType.None)
        {
            DiscScript newDisc = DiscManager.Instance.GetDiscFromPool(_lootedDiscType);
            if(newDisc != null)
            {
                newDisc.transform.position = transform.position;
            }
        }
    }

    bool spawned = false;
    public void SpawnEnemy(Vector3 position, DiscType lootedDiscType)
    {
        if (spawned)
            return;

        spawned = true;

        damageReceiptionSystem.SetUpSystem(false);
        transform.position = position;
        gameObject.SetActive(true);
        SetUpInitiative();
        _lootedDiscType = lootedDiscType;

        if (_lootedDiscType != DiscType.None)
            Debug.Log(name + " will loot " + _lootedDiscType + " disc");

        if (lootDiscIndicator != null)
        {
            lootDiscIndicator.SetActive(_lootedDiscType != DiscType.None);
        }
    }

    public void SetUpInitiative()
    {
        if (setedUpInitiative)
            return;

        setedUpInitiative = true;
        enemyInstanceInitiative = baseInitiative + UnityEngine.Random.Range(0f, 1f);

        name = name + " - " +  GetEnemyInitiative.ToString();
    }

    #region Turn management

    public void StartTurn()
    {
        myIA.myNavAgent.avoidancePriority = 10;
        myIA.isPlaying = true;
        PlayMyTurn();
    }

    public void EndTurn()
    {
        myIA.myNavAgent.avoidancePriority = 50;
        myIA.isPlaying = false;
        if(TurnManager.Instance.GetCurrentTurnState != TurnState.EnemyTurn)
        {
            return;
        }

        TurnManager.Instance.EndEnemyTurn(this, GetPlayerDetected);
    }

    public void InterruptAllAction()
    {
        //Debug.Log("Interrupt " + name + "'s actions");
        // TO DO : interrupt action of the linked AI, without calling EndTurn 
    }
    #endregion

    #region IA
    [Header("IA")]

    public IAEnemyVirtual myIA = default;
    public void SetPlayerDetected(bool detected)
    {
        myIA.haveDetectPlayer = detected;
    }
    public bool GetPlayerDetected => myIA.haveDetectPlayer;

    void PlayMyTurn()
    {
        if (myIA == null)
            return;

        myIA.PlayerTurn();
    }
    #endregion

    private void OnEnable()
    {
        damageReceiptionSystem.OnLifeAmountChanged += UpdateLifeBarFill;
        damageReceiptionSystem.OnLifeReachedZero += Die;
        TurnManager.Instance.OnEnemyTurnInterruption += InterruptAllAction;

        if (myIA == null)
            return;
        myIA.OnFinishTurn += EndTurn;
    }

    private void OnDisable()
    {
        damageReceiptionSystem.OnLifeAmountChanged -= UpdateLifeBarFill;
        damageReceiptionSystem.OnLifeReachedZero -= Die;
        TurnManager.Instance.OnEnemyTurnInterruption -= InterruptAllAction;

        if (myIA == null)
            return;

        myIA.OnFinishTurn -= EndTurn;
    }

    public void DisplayAndActualisePreviewAttack(Transform target)
    {
        myIA.myShowPath.SetValue(myIA.distanceOfDeplacement, myIA.attackRange);
        myIA.myShowPath.ShowOrHide(true);
        myIA.myShowPath.SetTargetPosition(target);
    }

    public void HidePreview(bool value)
    {
        myIA.myShowPath.ShowOrHide(value);
    }
}
