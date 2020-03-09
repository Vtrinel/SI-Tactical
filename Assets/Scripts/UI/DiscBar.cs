using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscBar : MonoBehaviour
{
    private int maxDisc;

    [SerializeField] int currentDisc;

    [Header("Disc types")]
    // Types : Basic, Piercing, Ghost, Explosive, Heavy, Shockwave
    public GameObject discBasic;
    public GameObject discPiercing;
    public GameObject discGhost;

    [SerializeField] List<GameObject> allDiscBarElement = new List<GameObject>();

    private void OnEnable()
    {
        DiscManager.Instance.OnDiskConsommed += RemoveDiscFromBar;
        GameManager.Instance.OnPlayerMaxLifeAmountChanged += UpdateMaxDiscBar;
    }

    private void OnDisable()
    {
        DiscManager.Instance.OnDiskConsommed -= RemoveDiscFromBar;
        GameManager.Instance.OnPlayerMaxLifeAmountChanged -= UpdateMaxDiscBar;
    }


    void Start()
    {
        maxDisc = GameManager.Instance.maxPlayerLifeAmount;
        currentDisc = GameManager.Instance.GetCurrentPlayerLifeAmount;

        CreateDiscBar();
    }

    void CreateDiscBar()
    {
        for (int i = 0; i < maxDisc; i++)
        {
            GameObject newLifeBarElement = Instantiate(discBasic, gameObject.transform);
            allDiscBarElement.Add(newLifeBarElement);
        }
    }

    void RemoveDiscFromBar()
    {
        int i = 0;

        foreach (GameObject _lifeBar in allDiscBarElement)
        {
            if (i < currentDisc)
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

    void UpdateMaxDiscBar(int _numberLives)
    {
        // Add the new bars of lifes
        for (int i = 0; i < _numberLives; i++)
        {
            GameObject newLifeBarElement = Instantiate(discBasic, gameObject.transform);
            allDiscBarElement.Add(newLifeBarElement);
        }

        maxDisc += _numberLives;
        currentDisc += _numberLives;

        int j = 0;

        foreach (GameObject _lifeBar in allDiscBarElement)
        {
            if (j < currentDisc)
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
