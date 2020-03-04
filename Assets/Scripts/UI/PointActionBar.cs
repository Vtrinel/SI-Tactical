using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointActionBar : MonoBehaviour
{
    private int maxPoint = 6;

    private int currentPoint;

    public GameObject pointAction;

    List<GameObject> allPointBarElement = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.OnActionPointsAmountChanged += UpdatePointBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnActionPointsAmountChanged -= UpdatePointBar;
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
}
