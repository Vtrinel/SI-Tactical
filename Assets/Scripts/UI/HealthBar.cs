using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private int maxHealth;

    private int currentHealth;

    public GameObject healthPoint;

    List<GameObject> allLifeBarElement = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.OnActionPointsAmountChanged += UpdateLifeBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnActionPointsAmountChanged -= UpdateLifeBar;
    }



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
            GameObject newLifeBarElement = Instantiate(healthPoint, gameObject.transform);
            allLifeBarElement.Add(newLifeBarElement);
        }
    }

    void UpdateLifeBar(int _numberLives)
    {
        int i = 0;

        foreach(GameObject _lifeBar in allLifeBarElement)
        {
            if(i < currentHealth)
            {
                //Oui
                _lifeBar.GetComponent<Animator>().SetBool("Statut", true);
            }
            else
            {
                //Non
                _lifeBar.GetComponent<Animator>().SetBool("Statut", false);
            }
            i++;
        }
    }
}
