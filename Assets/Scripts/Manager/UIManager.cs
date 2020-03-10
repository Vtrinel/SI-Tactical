﻿using NaughtyAttributes;
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

        ShowStartPanel();
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

    #region Game Management
    [Header("Game Management")]
    [SerializeField] GameObject startPanel = default;
    [SerializeField] GameObject winPanel = default;
    [SerializeField] GameObject losePanel = default;
    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
    }

    public void HideStartPanel()
    {
        startPanel.SetActive(false);
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
    }
    #endregion

    [Header("Turn Management")]
    [SerializeField] Animation startTurnAnimation = default;
    [SerializeField] Text turnStateText = default;
    [SerializeField] Text additionalInfoText1 = default;
    [SerializeField] Text additionalInfoText2 = default;
    public void PlayStartTurnAnimation(TurnState currentTurnState, bool newEnemiesappeared, bool newEnemiesIncoming)
    {
        startTurnAnimation.Play();
        switch (currentTurnState)
        {
            case TurnState.PlayerTurn:
                turnStateText.text = "NEW TURN";
                break;
            case TurnState.EnemyTurn:
                turnStateText.text = "ENEMIES TURN";
                break;
        }

        additionalInfoText1.gameObject.SetActive(false);
        additionalInfoText2.gameObject.SetActive(false);

        int usedTextsCounter = 0;
        if (newEnemiesappeared)
        {
            Text textToUse = (usedTextsCounter == 0 ? additionalInfoText1 : additionalInfoText2);
            textToUse.gameObject.SetActive(true);
            textToUse.text = "New foes appeared";

            usedTextsCounter++;
        }
        if (newEnemiesIncoming)
        {
            Text textToUse = (usedTextsCounter == 0 ? additionalInfoText1 : additionalInfoText2);
            textToUse.gameObject.SetActive(true);
            textToUse.text = "New foes incoming";

            usedTextsCounter++;
        }
    }
}
