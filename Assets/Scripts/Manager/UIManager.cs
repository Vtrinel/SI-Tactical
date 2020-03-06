using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Actions 
    public UnityAction OnPlayerMove;
    public UnityAction OnPlayerSpecial;
    public UnityAction OnPlayerLaunch;
    public UnityAction OnPlayerRecall;
    public UnityAction OnPlayerSwap;
    #endregion

    #region ButtonUtilities 
    [Header("UI Parameters")]
    public GameObject ButtonMove;
    public GameObject ButtonLaunch;
    public GameObject ButtonRecall;
    public Image goldSlider;
    #endregion

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    void OnEnable()
    {
        PlayerExperienceManager.Instance.OnGainExperience += AddExperience;
        PlayerExperienceManager.Instance.OnLossExperience += LossExperience;
    }

    void OnDisable()
    {
        PlayerExperienceManager.Instance.OnGainExperience -= AddExperience;
        PlayerExperienceManager.Instance.OnLossExperience -= LossExperience;
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

    public void AddExperience(int experience)
    {
        goldSlider.fillAmount += experience / 100;
    }

    public void LossExperience(int experience)
    {
        goldSlider.fillAmount -= experience / 100;
    }

    #region Menu interactions

    public void Menu()
    {
        Debug.Log("Menu");
    }

    public void EndTurn()
    {
        Debug.Log("EndTurn");
    }

    public void UndoMove()
    {
        Debug.Log("UndoMove");
    }

    #endregion


    #region Skills

    public void Move()
    {
        Debug.Log("Move");
        OnPlayerMove?.Invoke();
    }

    public void Special()
    {
        Debug.Log("Special");
        OnPlayerSpecial?.Invoke();
    }

    public void Launch()
    {
        Debug.Log("Launch");
        OnPlayerLaunch?.Invoke();
    }

    public void Recall()
    {
        Debug.Log("Recall");
        OnPlayerRecall?.Invoke();
    }

    public void Swap()
    {
        Debug.Log("Swap");
        OnPlayerSwap?.Invoke();
    }

    #endregion
}
