using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private int maxHealth;

    [SerializeField] int currentHealth;

    public GameObject healthPoint;

    [SerializeField] List<LifeElement> allLifeBarElement = new List<LifeElement>();

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerLifeAmountChanged += UpdateLifeBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerLifeAmountChanged -= UpdateLifeBar;
    }

    void UpdateLifeBar(int _numberLives)
    {
        currentHealth = _numberLives;

        int i = 0;

        foreach(LifeElement _lifeElement in allLifeBarElement)
        {
            _lifeElement.SetValue(i < currentHealth);
            i++;
        }
    }
}
