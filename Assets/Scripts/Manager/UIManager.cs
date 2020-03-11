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
        PlayerExperienceManager.Instance.OnGainGold += AddExperience;
        PlayerExperienceManager.Instance.OnLossGold += LossExperience;
        
    }

    void OnDisable()
    {
        PlayerExperienceManager.Instance.OnGainGold -= AddExperience;
        PlayerExperienceManager.Instance.OnLossGold -= LossExperience;
        //TurnManager.Instance.OnStartPlayerTurn -= StartPlayerTurn;
        //TurnManager.Instance.OnEndPlayerTurn -= EndPlayerTurn;
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

        //ShowStartPanel();
    }

    public void ChangeEndTurnButtonVisibility(bool visible)
    {
        buttonEndTurn.SetActive(visible);
    }

    public void AddExperience(int experience)
    {
        goldSlider.fillAmount += PlayerExperienceManager.Instance.GetGoldQuantity / 100.0f;
    }

    public void LossExperience(int experience)
    {
        if (PlayerExperienceManager.Instance.GetGoldQuantity > 0)
        {
            goldSlider.fillAmount += PlayerExperienceManager.Instance.GetGoldQuantity / 100.0f;

        }
        else
        {
            goldSlider.fillAmount = 0;
        }
    }

    #region AP Costs
    [Header("Action poins cost")]
    [SerializeField] Text actionPointsCostText = default;
    [SerializeField] RectTransform actionPointsCostTextParent = default;
    [SerializeField] PointActionBar actionBar = default;
    public PointActionBar GetActionBar => actionBar;

    public void ShowActionPointsCostText()
    {
        actionPointsCostTextParent.gameObject.SetActive(true);
    }

    public void UpdateActionPointCostText(int cost, int total)
    {
        actionPointsCostText.text = cost + "/" + total + "AP";
        Vector2 newPos = Input.mousePosition;
        if (newPos.x > Screen.width - actionPointsCostTextParent.sizeDelta.x)
            newPos.x -= actionPointsCostTextParent.sizeDelta.x;

        if (newPos.y < actionPointsCostTextParent.sizeDelta.y)
            newPos.y += actionPointsCostTextParent.sizeDelta.y;

        actionPointsCostTextParent.localPosition = newPos;
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

    #region Turn Management
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
    #endregion

    #region Goal Management
    [Header("Goal Management")]
    [SerializeField] Animator goalPanelAnimator = default;
    [SerializeField] Text turnGoalText = default;
    [SerializeField] Text turnGoalValueText = default;
    int totalNumberOfTurnsToWait = 0;
    int remainingNumberOfTurnsToWait = 0;

    public void SetUpGoalPanel(int numberOfTurnsToWait)
    {
        totalNumberOfTurnsToWait = numberOfTurnsToWait;
        remainingNumberOfTurnsToWait = numberOfTurnsToWait;
        turnGoalText.text = "Stay near the god's statue for " + totalNumberOfTurnsToWait + " turns";
        turnGoalValueText.text = remainingNumberOfTurnsToWait.ToString();

        goalPanelAnimator.SetTrigger("showGoalPanel");
    }

    public void OnGoalZoneReached()
    {
        goalPanelAnimator.SetTrigger("reachedGoalZone");
    }

    public void UpdateRemainingNumberOfTurns(int remaining)
    {
        remainingNumberOfTurnsToWait = remaining;
        //turnGoalText.text = "Stay near the god's statue for " + remainingNumberOfTurnsToWait + " more turns";
        turnGoalValueText.text = remainingNumberOfTurnsToWait.ToString();
        goalPanelAnimator.SetTrigger("updateGoal");
    }

    public void OnGoalTurnAmountReached()
    {
        goalPanelAnimator.SetTrigger("reachedGoalTurn");
    }
    #endregion
}
