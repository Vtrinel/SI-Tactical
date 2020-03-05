using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    private void Start()
    {
        damageReceiptionSystem.SetUpSystem();
        damageReceiptionSystem.OnCurrentLifeAmountChanged += UpdateLifeBarFill;
        damageReceiptionSystem.OnLifeReachedZero += Die;

        enemyRenderer.material = normalMaterial;

        SetUpInitiative();
    }

    [Header("References")]
    [SerializeField] DamageableEntity damageReceiptionSystem = default;
    [SerializeField] KnockbackableEntity knockbackReceiptionSystem = default;
    [SerializeField] Image lifeBar = default;
    [SerializeField] MeshRenderer enemyRenderer = default;
    public void UpdateLifeBarFill(int currentAmount, int delta)
    {
        lifeBar.fillAmount = damageReceiptionSystem.GetCurrentLifePercent;
    }

    public Action<EnemyBase> OnEnemyDeath;
    public void Die()
    {
        Debug.Log(name + " (Enemy) is dead");
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);
    }   

    [Header("Common Values")]
    [SerializeField] int baseInitiative = 1;
    float enemyInstanceInitiative = 1;
    public float GetEnemyInitiative => enemyInstanceInitiative;
    bool setedUpInitiative = false;

    public void SetUpInitiative()
    {
        if (setedUpInitiative)
            return;

        setedUpInitiative = true;
        enemyInstanceInitiative = baseInitiative + UnityEngine.Random.Range(0f, 1f);
    }

    #region Placeholder
    [Header("Placeholder")]
    [SerializeField] Material normalMaterial = default;
    [SerializeField] Material activeMaterial = default;
    public void StartTurn()
    {
        Debug.Log(name + "' turn");
        enemyRenderer.material = activeMaterial;

        PlayMyTurn();
    }

    public void EndTurn()
    {
        enemyRenderer.material = normalMaterial;
        TurnManager.Instance.EndEnemyTurn(this);
    }

    #endregion
    
    
    #region IA
    [Header("IA")]

    [SerializeField] BasicEnemy myIA = default;

    void PlayMyTurn()
    {
        if (myIA == null)
            return;

        myIA.PlayerTurn();
    }
    #endregion

    private void OnEnable()
    {
        if (myIA == null)
            return;

        myIA.OnIsAtDestination += EndTurn;
    }

    private void OnDisable()
    {
        if (myIA == null)
            return;

        myIA.OnIsAtDestination -= EndTurn;
    }

}
