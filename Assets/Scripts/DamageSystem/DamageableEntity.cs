using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour
{
    bool _isPlayer = false;

    [SerializeField] DamageTag damageTag = default;
    public DamageTag GetDamageTag => damageTag;

    [SerializeField] int maxLifeAmount = 10;
    [SerializeField] int currentLifeAmount = 0;
    public int GetCurrentLifeAmount => currentLifeAmount;
    public float GetCurrentLifePercent => (float)currentLifeAmount / maxLifeAmount;

    /// <summary>
    /// First parameter is current life, second parameter is life differential
    /// </summary>
    public Action<int, int> OnLifeAmountChanged;
    /// <summary>
    /// First parameter is current life, second parameter is life differential
    /// </summary>
    public Action<int, int> OnReceivedDamages;
    public Action OnLifeReachedZero;
    public Action<int> OnReceivedStun;

    public void SetUpSystem(bool isPlayer)
    {
        _isPlayer = isPlayer;
        ResetLifeAmount();
    }

    public void ResetLifeAmount()
    {
        if (_isPlayer)
            maxLifeAmount = GameManager.Instance.maxPlayerLifeAmount;

        currentLifeAmount = maxLifeAmount;

        if (_isPlayer)
            GameManager.Instance.PlayerLifeChange(currentLifeAmount);
    }

    public void ReceiveDamage(DamageTag sourceDamageTag, DamagesParameters damagesParameters)
    {
        if (sourceDamageTag != DamageTag.Environment && sourceDamageTag == damageTag)
            return;

        if (damagesParameters._damages == 0)
            return;

        LoseLife(damagesParameters._damages);

        OnReceivedDamages?.Invoke(currentLifeAmount, -Mathf.Abs(damagesParameters._damages));

        if (damagesParameters._numberOfStunedTurns > 0)
        {
            Debug.Log(name + " is stuned for " + damagesParameters._numberOfStunedTurns + " turn" + (damagesParameters._numberOfStunedTurns > 1 ? "s" : ""));
            OnReceivedStun?.Invoke(damagesParameters._numberOfStunedTurns);
        }
    }

    public void LoseLife(int amount)
    {
        currentLifeAmount = Mathf.Clamp(currentLifeAmount - Mathf.Abs(amount), 0, maxLifeAmount);
        OnLifeAmountChanged?.Invoke(currentLifeAmount, -Mathf.Abs(amount));

        if (_isPlayer)
            GameManager.Instance.PlayerLifeChange(currentLifeAmount);

        if (currentLifeAmount == 0)
            LifeReachedZero();
    }

    public void RegainLife(int amount)
    {
        currentLifeAmount = Mathf.Clamp(currentLifeAmount + Mathf.Abs(amount), 0, maxLifeAmount);
        OnLifeAmountChanged?.Invoke(currentLifeAmount, Mathf.Abs(amount));

        if (_isPlayer)
            GameManager.Instance.PlayerLifeChange(currentLifeAmount);
    }

    public void LifeReachedZero()
    {
        OnLifeReachedZero?.Invoke();
    }

    public void AddLifeBar(int value)
    {
        currentLifeAmount += value;
        maxLifeAmount += value;
        OnLifeAmountChanged?.Invoke(currentLifeAmount, Mathf.Abs(value));
    }
}

public struct DamagesParameters
{
    public DamagesParameters(int damages)
    {
        _damages = damages;
        _numberOfStunedTurns = 0;
    }
    public DamagesParameters(int damages, int stunedTurns)
    {
        _damages = damages;
        _numberOfStunedTurns = stunedTurns;
    }

    public int _damages;
    public int _numberOfStunedTurns;
}

public enum DamageTag { Player, Enemy, Environment, Disc }