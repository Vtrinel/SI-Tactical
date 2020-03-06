using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private int maxHealth;

    [SerializeField] int currentHealth;

    public GameObject healthPoint;

    List<GameObject> allLifeBarElement = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerLifeAmountChanged += UpdateLifeBar;
        GameManager.Instance.OnPlayerMaxLifeAmountChanged += UpdateMaxLifeBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerLifeAmountChanged -= UpdateLifeBar;
        GameManager.Instance.OnPlayerMaxLifeAmountChanged -= UpdateMaxLifeBar;
    }


    void Start()
    {
        maxHealth = GameManager.Instance.maxPlayerLifeAmount;
        currentHealth = GameManager.Instance.GetCurrentPlayerLifeAmount;

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
        currentHealth = _numberLives;

        print("passage");
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

    void UpdateMaxLifeBar(int _numberLives)
    {
        // Add the new bars of lifes
        for (int i = 0; i < _numberLives; i++)
        {
            GameObject newLifeBarElement = Instantiate(healthPoint, gameObject.transform);
            allLifeBarElement.Add(newLifeBarElement);
        }

        maxHealth += _numberLives;
        currentHealth += _numberLives;

        print("passage");
        int j = 0;

        foreach (GameObject _lifeBar in allLifeBarElement)
        {
            if (j < currentHealth)
            {
                //Oui
                _lifeBar.GetComponent<Animator>().SetBool("Statut", true);
            }
            else
            {
                //Non
                _lifeBar.GetComponent<Animator>().SetBool("Statut", false);
            }
            j++;
        }
    }
}
