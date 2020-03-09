using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] EnemyType enemyType = EnemyType.TouniBase;
    public EnemyType GetEnemyType => enemyType;


    private void Start()
    {
        damageReceiptionSystem.SetUpSystem(false);

        SetUpInitiative();
    }

    [Header("References")]
    [SerializeField] DamageableEntity damageReceiptionSystem = default;
    [SerializeField] KnockbackableEntity knockbackReceiptionSystem = default;
    [SerializeField] Image lifeBar = default;
    [SerializeField] MeshRenderer enemyRenderer = default;
    public void UpdateLifeBarFill(int currentAmount, int damageDelta)
    {
        lifeBar.fillAmount = damageReceiptionSystem.GetCurrentLifePercent;
    }

    public Action<EnemyBase> OnEnemyDeath;
    public void Die()
    {
        Debug.Log(name + " (Enemy) is dead");
        setedUpInitiative = false;

        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);
    }   

    [Header("Common Values")]
    [SerializeField] int baseInitiative = 1;
    float enemyInstanceInitiative = 1;
    public float GetEnemyInitiative => enemyInstanceInitiative;
    bool setedUpInitiative = false;

    public void SpawnEnemy(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        SetUpInitiative();
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
        myIA.isPlaying = true;
        PlayMyTurn();
    }

    public void EndTurn()
    {
        myIA.isPlaying = false;
        if(TurnManager.Instance.GetCurrentTurnState != TurnState.EnemyTurn)
        {
            return;
        }
        TurnManager.Instance.EndEnemyTurn(this);
    }

    public void InterruptAllAction()
    {
        //Debug.Log("Interrupt " + name + "'s actions");
        // TO DO : interrupt action of the linked AI, without calling EndTurn 
    }
    #endregion

    #region IA
    [Header("IA")]

    [SerializeField] IAEnemyVirtual myIA = default;
    public void SetPlayerDetected(bool detected)
    {
        myIA.haveDetectPlayer = detected;
    }

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
}
