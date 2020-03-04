using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private void Start()
    {
        damageReceiptionSystem.SetUpSystem();
        damageReceiptionSystem.OnCurrentLifeAmountChanged += DebugLifeChange;
        damageReceiptionSystem.OnLifeReachedZero += Die;
    }

    [Header("References")]
    [SerializeField] DamageableEntity damageReceiptionSystem = default;

    public void Die()
    {
        Debug.Log(name + " (Enemy) is dead");
    }
    
    public void DebugLifeChange(int currentAmount, int delta)
    {
        Debug.Log((delta < 0 ? "Lost" : "Regained") + delta + "HP. Current life if " + currentAmount + "HP");
    }
}
