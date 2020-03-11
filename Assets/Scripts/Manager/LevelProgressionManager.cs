using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressionManager : MonoBehaviour
{
    private static LevelProgressionManager _instance;
    public static LevelProgressionManager Instance { get { return _instance; } }

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

        if (goalZone != null)
        {
            goalZone.SetUp();
            OnProgressValueChanged += goalZone.UpdateProgressionBar;
        }
        else
            Debug.LogWarning("WARNING : No LevelGoalZone on LevelManager");

        currentProgressValue = 0;
    }

    [Header("References")]
    [SerializeField] LevelGoalZone goalZone = default;
    public LevelGoalZone GetGoalZone => goalZone;

    [Header("Parameters")]
    [SerializeField] int targetProgressValue = 5;
    int currentProgressValue = 0;

    /// <summary>
    /// First is progress value, second is progress delta, third is target progress value
    /// </summary>
    public Action<int, int, int> OnProgressValueChanged;
    public Action OnGoalReached;

    public bool CheckForProgressTurn()
    {
        bool progressed = false;

        if (goalZone != null)
        {
            int thisTurnProgress = goalZone.GetProgressionAmount();

            if (thisTurnProgress > 0)
            {
                StartCoroutine(LookAtGoal(thisTurnProgress));
                progressed = true;
            }
        }
        else
            Debug.LogWarning("WARNING : No LevelGoalZone on LevelManager");

        return progressed;
    }

    public IEnumerator LookAtGoal(int thisTurnProgress)
    {
        CameraManager.instance.GetPlayerCamera.AttachFollowTransformTo(goalZone.transform);

        yield return new WaitForSeconds(1f);

        currentProgressValue += thisTurnProgress;
        currentProgressValue = Mathf.Clamp(currentProgressValue, 0, targetProgressValue);

        OnProgressValueChanged?.Invoke(currentProgressValue, thisTurnProgress, targetProgressValue);
        UIManager.Instance.UpdateRemainingNumberOfTurns(targetProgressValue - currentProgressValue);
        if (currentProgressValue == targetProgressValue)
        {
            currentProgressValue = targetProgressValue;
            OnGoalReached?.Invoke();
            UIManager.Instance.OnGoalTurnAmountReached();
        }

        yield return new WaitForSeconds(1f);

        TurnManager.Instance.EndProgressionTurn();
    }

    public void SetUpGoalAnimation()
    {
        UIManager.Instance.SetUpGoalPanel(targetProgressValue);
    }
}
