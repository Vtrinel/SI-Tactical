using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private int maxHealth;
    private int currentHealth;
    public GameObject healthPoint;

    void Start()
    {
        maxHealth = UIManager.Instance.maxHealth;
        currentHealth = UIManager.Instance.currentHealth;

        CreateHealthBar();
    }

    void CreateHealthBar()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            Instantiate(healthPoint, gameObject.transform);
        }
    }

    void ChangeHealth()
    {

    }
}
