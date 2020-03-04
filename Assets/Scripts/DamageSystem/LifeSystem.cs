using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LifeSystem
{
    [SerializeField] int maxLifeAmount = 10;
    int currentLifeAmount;
    public int GetCurrentLifeAmount => currentLifeAmount;

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
            OnLifeReachedZero?.Invoke();
    }

    public void RegainLife(int amount)
    {
        currentLifeAmount = Mathf.Clamp(currentLifeAmount + Mathf.Abs(amount), 0, maxLifeAmount);
        OnCurrentLifeAmountChanged?.Invoke(currentLifeAmount, Mathf.Abs(amount));
    }
}
