using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointActionBar : MonoBehaviour
{
    private int maxPoint = 6;
    private int currentPoint;
    public GameObject pointAction;
    [SerializeField] List<PAElement> allPointBarElement = new List<PAElement>();


    private void OnEnable()
    {
        GameManager.Instance.OnActionPointsAmountChanged += UpdatePointBar;
        //TurnManager.Instance.OnStartPlayerTurn += ResetStateBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnActionPointsAmountChanged -= UpdatePointBar;
        //TurnManager.Instance.OnStartPlayerTurn -= ResetStateBar;
    }

    void Start()
    {
        maxPoint = GameManager.Instance.maxActionPointsAmount;
        currentPoint = GameManager.Instance.GetCurrentActionPointsAmount;
    }

    void UpdatePointBar(int value)
    {
        currentPoint = value;
        int i = 0;

        foreach(PAElement _pointBar in allPointBarElement)
        {
            _pointBar.SetValue(i < currentPoint);
            i++;
        }
    }

    // Update the action point bar to show the previsualisation of the action poin bar
    public void UpdatePreConsommationPointBar(int possessedActionPoints, int aboutToUseActionPoints)
    {
        //A REFAIRE
    }
}
