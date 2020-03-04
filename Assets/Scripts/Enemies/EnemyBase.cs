using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    private void Start()
    {
        damageReceiptionSystem.SetUpSystem();
        damageReceiptionSystem.OnCurrentLifeAmountChanged += DebugLifeChange;
        damageReceiptionSystem.OnCurrentLifeAmountChanged += UpdateLifeBarFill;
        damageReceiptionSystem.OnLifeReachedZero += Die;
    }

    [Header("References")]
    [SerializeField] DamageableEntity damageReceiptionSystem = default;
    [SerializeField] Image lifeBar = default;
    public void UpdateLifeBarFill(int currentAmount, int delta)
    {
        lifeBar.fillAmount = damageReceiptionSystem.GetCurrentLifePercent;
    }

    public void Die()
    {
        Debug.Log(name + " (Enemy) is dead");
        Destroy(gameObject);
    }
    
    public void DebugLifeChange(int currentAmount, int delta)
    {
        Debug.Log((delta < 0 ? "Lost" : "Regained") + delta + "HP. Current life if " + currentAmount + "HP");
    }
}
