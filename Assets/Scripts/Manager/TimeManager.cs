using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TimeManager : MonoBehaviour
{
    [Header("Pause Values")]
    [SerializeField, ReadOnly] private bool timePaused;
    float previousTimeScale = 1;
    bool wasInSlowMotion = false;

    [Header("Slow Motion settings")]
    [SerializeField] AnimationCurve timeRecoverEvolution;
    [Min(0.0f)] public float timeRecoverDuration = 1.25f;
    [Range(0.01f, 1.0f)] public float slowMotionScale = 0.4f;
    float currentRecoverDuration = 0.0f;
    [SerializeField, ReadOnly] bool inSlowMotion = false;

    [Header("FreezeFrame settings")]
    [Tooltip("1 frame =  0,0165 seconds")]
    [SerializeField] float freezeFrameDuration = 0.036f;
    [SerializeField, ReadOnly] bool gameIsFrozen;

    public bool GameIsFrozen { get => gameIsFrozen; private set => gameIsFrozen = value; }
    public bool InSlowMotion { get => inSlowMotion; private set => inSlowMotion = value; }

    private static TimeManager instance;
    public static TimeManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        InSlowMotion = false;
    }

    private void Update()
    {
        SlowMotion();
    }

    #region Slow Motion Methods

    /// <summary>
    /// Trigger the slow Motion set up in the settings
    /// </summary>
    [Button] public void StartSlowMotion()
    {
        InSlowMotion = true;
    }

    void SlowMotion()
    {
        if (!InSlowMotion) return;

        currentRecoverDuration += Time.unscaledDeltaTime;

        float percent = currentRecoverDuration / timeRecoverDuration;
        float curveEval = timeRecoverEvolution.Evaluate(percent);
        float targetTimeScale = Mathf.Lerp(slowMotionScale, 1, curveEval);
        Time.timeScale = targetTimeScale;

        if (currentRecoverDuration >= timeRecoverDuration)
        {
            currentRecoverDuration = 0.0f;
            InSlowMotion = false;
            Time.timeScale = 1;
        }
    }

    #endregion

    private bool TimePaused
    {
        get
        {
            return timePaused;
        }
        set
        {
            if (value == true)
            {
                if (InSlowMotion)
                {
                    wasInSlowMotion = true;
                    InSlowMotion = false;
                }

                previousTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                if (wasInSlowMotion)
                {
                    InSlowMotion = true;
                }

                Time.timeScale = previousTimeScale;
            }

            timePaused = value;
        }
    }

    /// <summary>
    /// Stop the game by setting the timeScale to 0
    /// </summary>
    [Button] public void Pause()
    {
        TimePaused = true;
    }

    /// <summary>
    /// Resume the game by setting the timescale a 1
    /// </summary>
    [Button] public void Resume()
    {
        TimePaused = false;
    }

    #region Freeze Frame Methods

    /// <summary>
    /// Trigger a freezeframe set up in the freeze frame settings
    /// </summary>
    [Button] public void StartFreezeFrame()
    {
        StartCoroutine(FreezeFrame());
    }

    public IEnumerator FreezeFrame()
    {
        Time.timeScale = 0;
        GameIsFrozen = true;
        yield return new WaitForSecondsRealtime(freezeFrameDuration);
        GameIsFrozen = false;
        Time.timeScale = 1;
    }
    #endregion

}

