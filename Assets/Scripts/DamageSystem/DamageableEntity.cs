using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour
{
    [SerializeField] DamageTag damageTag = default;
    public DamageTag GetDamageTag => damageTag;

    int maxLifeAmount = 10;
    [SerializeField] int currentLifeAmount;
    public int GetCurrentLifeAmount => currentLifeAmount;
    public float GetCurrentLifePercent => (float)currentLifeAmount / maxLifeAmount;

    /// <summary>
    /// First parameter is current life, second parameter is life differential
    /// </summary>
    public Action<int, int> OnReceivedDamages;
    public Action OnLifeReachedZero;

    public void SetUpSystem()
    {
        ResetLifeAmount();
    }

    public void ResetLifeAmount()
    {
        maxLifeAmount = GameManager.Instance.maxPlayerLifeAmount;
        currentLifeAmount = maxLifeAmount;
        GameManager.Instance.PlayerLifeChange(currentLifeAmount);
    }

    public void ReceiveDamage(DamageTag sourceDamageTag, int damageAmount)
    {
        if (sourceDamageTag != DamageTag.Environment && sourceDamageTag == damageTag)
            return;

        if (damageAmount == 0)
            return;

        LoseLife(damageAmount);

        OnReceivedDamages?.Invoke(currentLifeAmount, -Mathf.Abs(damageAmount));
    }

    public void LoseLife(int amount)
    {
        currentLifeAmount = Mathf.Clamp(currentLifeAmount - Mathf.Abs(amount), 0, maxLifeAmount);
        GameManager.Instance.PlayerLifeChange(currentLifeAmount);

        if (currentLifeAmount == 0)
            LifeReachedZero();
    }

    public void RegainLife(int amount)
    {
        currentLifeAmount = Mathf.Clamp(currentLifeAmount + Mathf.Abs(amount), 0, maxLifeAmount);
        GameManager.Instance.PlayerLifeChange(currentLifeAmount);
    }

    public void LifeReachedZero()
    {
        OnLifeReachedZero?.Invoke();
    }
}

public enum DamageTag { Player, Enemy, Environment }