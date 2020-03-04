using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour
{
    [SerializeField] DamageTag damageTag = default;
    public DamageTag GetDamageTag => damageTag;

    [SerializeField] int maxLifeAmount = 10;
    int currentLifeAmount;
    public int GetCurrentLifeAmount => currentLifeAmount;
    public float GetCurrentLifePercent => (float)currentLifeAmount / maxLifeAmount;

    /// <summary>
    /// First parameter is current life, second parameter is life differential
    /// </summary>
    public Action<int, int> OnCurrentLifeAmountChanged;
    public Action OnLifeReachedZero;

    public void SetUpSystem()
    {
        ResetLifeAmount();
    }

    public void ResetLifeAmount()
    {
        currentLifeAmount = maxLifeAmount;

        OnCurrentLifeAmountChanged?.Invoke(currentLifeAmount, 0);
    }

    public void LoseLife(int amount)
    {
        currentLifeAmount = Mathf.Clamp(currentLifeAmount - Mathf.Abs(amount), 0, maxLifeAmount);
        OnCurrentLifeAmountChanged?.Invoke(currentLifeAmount, -Mathf.Abs(amount));

        if (currentLifeAmount == 0)
            LifeReachedZero();
    }

    public void RegainLife(int amount)
    {
        currentLifeAmount = Mathf.Clamp(currentLifeAmount + Mathf.Abs(amount), 0, maxLifeAmount);
        OnCurrentLifeAmountChanged?.Invoke(currentLifeAmount, Mathf.Abs(amount));
    }

    public void LifeReachedZero()
    {
        OnLifeReachedZero?.Invoke();
    }
}

public enum DamageTag { Player, Enemy, Environment }