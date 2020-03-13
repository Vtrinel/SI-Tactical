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
    [SerializeField] TooltipColliderUI tooltipCollider = default;


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
        tooltipCollider.SetName("Action Points : " + maxPoint + "/" + currentPoint);
    }

    void UpdatePointBar(int value)
    {
        currentPoint = value;
        int i = 0;
        tooltipCollider.SetName("Action Points : " + currentPoint + "/" + maxPoint);

        foreach (PAElement _pointBar in allPointBarElement)
        {
            _pointBar.SetValue(i < currentPoint);
            i++;
        }
    }

    // Update the action point bar to show the previsualisation of the action poin bar
    public void UpdatePreConsommationPointBar(int possessedActionPoints, int aboutToUseActionPoints)
    {
        int currentConsommationPoint = possessedActionPoints - aboutToUseActionPoints;

        int i = 0;

        foreach (PAElement _pointBar in allPointBarElement)
        {
            if (i < currentConsommationPoint)
            {
                _pointBar.PA_Animator.SetBool("InPreview", false);
            }
            else
            {
                _pointBar.PA_Animator.Rebind();
                _pointBar.PA_Animator.SetBool("InPreview", true);
            }
            i++;
        }
    }
}
