﻿using System;
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

    [Header("Parameters")]
    [SerializeField] int targetProgressValue = 5;
    int currentProgressValue = 0;

    /// <summary>
    /// First is progress value, second is progress delta, third is target progress value
    /// </summary>
    public Action<int, int, int> OnProgressValueChanged;
    public Action OnGoalReached;

    public void CheckForProgressTurn()
    {
        if (goalZone != null)
        {
            int thisTurnProgress = goalZone.GetProgressionAmount();

            if (thisTurnProgress > 0)
            {
                currentProgressValue += thisTurnProgress;
                currentProgressValue = Mathf.Clamp(currentProgressValue, 0, targetProgressValue);

                OnProgressValueChanged?.Invoke(currentProgressValue, thisTurnProgress, targetProgressValue);

                //Debug.Log("Progress : " + currentProgressValue + "/" + targetProgressValue);
                if (currentProgressValue == targetProgressValue)
                {
                    currentProgressValue = targetProgressValue;
                    OnGoalReached?.Invoke();
                }
            }
        }
        else
            Debug.LogWarning("WARNING : No LevelGoalZone on LevelManager");
    }
}
