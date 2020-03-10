using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointActionBar : MonoBehaviour
{
    private int maxPoint = 6;

    private int currentPoint;

    public GameObject pointAction;

    CompetencesUsabilityManager competencesUsabilityManager = default;
    List<GameObject> allPointBarElement = new List<GameObject>();


    private void OnEnable()
    {
        competencesUsabilityManager = GameManager.Instance.GetCompetencesUsabilityManager();

        GameManager.Instance.OnActionPointsAmountChanged += UpdatePointBar;
        competencesUsabilityManager.OnCompetenceStateChanged += UpdatePreConsommationPointBar;
        competencesUsabilityManager.OnCompetenceStateChanged += UpdatePreConsommationPointBar;
        TurnManager.Instance.OnStartPlayerTurn += ResetStateBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnActionPointsAmountChanged -= UpdatePointBar;
        competencesUsabilityManager.OnCompetenceStateChanged -= UpdatePreConsommationPointBar;
        TurnManager.Instance.OnStartPlayerTurn -= ResetStateBar;
    }

    void Start()
    {
        maxPoint = GameManager.Instance.maxActionPointsAmount;
        currentPoint = GameManager.Instance.GetCurrentActionPointsAmount;

        CreateHealthBar();
    }

    void CreateHealthBar()
    {
        for (int i = 0; i < maxPoint; i++)
        {
            GameObject newPointBarElement = Instantiate(pointAction, gameObject.transform);
            allPointBarElement.Add(newPointBarElement);
        }
    }

    void UpdatePointBar(int value)
    {
        currentPoint = value;
        int i = 0;

        foreach(GameObject _pointBar in allPointBarElement)
        {
            if(i < currentPoint)
            {
                //Oui
                _pointBar.GetComponent<Animator>().SetBool("Statut", true);
            }
            else
            {
                //Non
                _pointBar.GetComponent<Animator>().SetBool("Statut", false);
            }
            i++;
        }
    }

    // Update the action point bar to show the previsualisation of the action poin bar
    void UpdatePreConsommationPointBar()
    {
        if (competencesUsabilityManager.IsPreparingCompetence)
        {
            int currentConsommationPoint = GameManager.Instance.GetCurrentActionPointsAmount - competencesUsabilityManager.GetCurrentCompetenceCost();
            int i = 0;

            foreach (GameObject _pointBar in allPointBarElement)
            {
                if (i < currentConsommationPoint)
                {
                    _pointBar.GetComponent<Animator>().SetBool("Preview", false);
                }
                else
                {
                    _pointBar.GetComponent<Animator>().SetBool("Preview", true);
                }
                i++;
            }
        }
        else
        {
            int currentActionPoint = GameManager.Instance.GetCurrentActionPointsAmount;
            int i = 0;

            foreach (GameObject _pointBar in allPointBarElement)
            {
                if (i < currentActionPoint)
                {
                    _pointBar.GetComponent<Animator>().SetBool("Statut", true);
                }
                else
                {
                    _pointBar.GetComponent<Animator>().SetBool("Statut", false);
                }
                i++;
            }
        }
        
    }

    void ResetStateBar()
    {
        foreach (GameObject _pointBar in allPointBarElement)
        {
            _pointBar.GetComponent<Animator>().SetBool("Preview", false);
        }
    }
}
