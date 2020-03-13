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

    [Header("Progression Feedbacks")]
    [SerializeField] ParticleSystem[] particlesToPlayOnProgress = new ParticleSystem[0];
    public Transform GetNewFocusPoint
    {
        get
        {
            if(currentProgressValue + 1 == targetProgressValue)
            {
                return goalZone.GetTransformToLookAt;
            }

            if (currentProgressValue < particlesToPlayOnProgress.Length)
            {
                return particlesToPlayOnProgress[currentProgressValue].transform;
            }

            return goalZone.GetTransformToLookAt;
        }
    }

    [Header("Parameters")]
    [SerializeField] int targetProgressValue = 5;
    public int GetRemainingNumberOfTurn => targetProgressValue - currentProgressValue;
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
        ParticleSystem particlesToPlay = null;
        if (currentProgressValue < particlesToPlayOnProgress.Length && particlesToPlayOnProgress.Length > 0)
        {
            particlesToPlay = particlesToPlayOnProgress[currentProgressValue];
        }

        CameraManager.instance.GetPlayerCamera.AttachFollowTransformTo(GetNewFocusPoint);

        yield return new WaitForSeconds(1f);

        currentProgressValue += thisTurnProgress;
        currentProgressValue = Mathf.Clamp(currentProgressValue, 0, targetProgressValue);

        if (particlesToPlay != null)
            particlesToPlay.gameObject.SetActive(true);

        OnProgressValueChanged?.Invoke(currentProgressValue, thisTurnProgress, targetProgressValue);
        UIManager.Instance.UpdateRemainingNumberOfTurns(targetProgressValue - currentProgressValue);
        if (currentProgressValue == targetProgressValue)
        {
            currentProgressValue = targetProgressValue;
            StartCoroutine(WinCoroutine());
        }
        else
        {
            yield return new WaitForSeconds(1f);

            TurnManager.Instance.EndProgressionTurn();
        }
    }

    public IEnumerator WinCoroutine()
    {
        Debug.Log("Ouai c'est la cinématique de victoire");
        yield return new WaitForSeconds(3f);

        OnGoalReached?.Invoke();
        UIManager.Instance.OnGoalTurnAmountReached();
    }

    public void SetUpGoalAnimation()
    {
        UIManager.Instance.SetUpGoalPanel(targetProgressValue);
    }
}
