using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region ButtonUtilities 
    [Header("UI Parameters")]
    public GameObject ButtonMove;
    public GameObject ButtonLaunch;
    public GameObject ButtonRecall;
    public Image goldSlider;

    public GameObject buttonEndTurn;
    #endregion

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    void OnEnable()
    {
        PlayerExperienceManager.Instance.OnGainExperience += AddExperience;
        PlayerExperienceManager.Instance.OnLossExperience += LossExperience;
        TurnManager.Instance.OnStartPlayerTurn += StartPlayerTurn;
        TurnManager.Instance.OnEndPlayerTurn += EndPlayerTurn;
    }

    void OnDisable()
    {
        PlayerExperienceManager.Instance.OnGainExperience -= AddExperience;
        PlayerExperienceManager.Instance.OnLossExperience -= LossExperience;
        TurnManager.Instance.OnStartPlayerTurn -= StartPlayerTurn;
        TurnManager.Instance.OnEndPlayerTurn -= EndPlayerTurn;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void StartPlayerTurn()
    {
        buttonEndTurn.SetActive(true);
    }

    void EndPlayerTurn()
    {
        buttonEndTurn.SetActive(false);
    }

    public void AddExperience(int experience)
    {
        goldSlider.fillAmount += experience / 100;
    }

    public void LossExperience(int experience)
    {
        goldSlider.fillAmount -= experience / 100;
    }

    #region AP Costs
    [Header("Action poins cost")]
    [SerializeField] Text actionPointsCostText = default;
    [SerializeField] Transform actionPointsCostTextParent = default;

    public void ShowActionPointsCostText()
    {
        actionPointsCostTextParent.gameObject.SetActive(true);
    }

    public void UpdateActionPointCostText(int cost, int total)
    {
        actionPointsCostText.text = cost + "/" + total + "AP";
        actionPointsCostTextParent.localPosition = Input.mousePosition;
    }

    public void HideActionPointText()
    {
        actionPointsCostTextParent.gameObject.SetActive(false);
    }
    #endregion
}
