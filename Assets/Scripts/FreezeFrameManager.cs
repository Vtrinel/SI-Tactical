using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeFrameManager : MonoBehaviour
{
    [Header("Parameters")]
    private float freezeFrameSpeed = 1f;
    private AnimationCurve freezeFrameCurve;
    private float freezeFrameCompletion = 0f;
    private bool isFreezeFraming = false;

    [Header("default freeze frame")]


    private static FreezeFrameManager instance;
    public static FreezeFrameManager Instance { get { return instance; } }

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

    void Update()
    {
        FreezeFrameUpdate();
    }

    public void FreezeFrame(AnimationCurve curve, float speed)
    {
        if(!isFreezeFraming)
        {
            freezeFrameCompletion = 0f;
            isFreezeFraming = true;
            freezeFrameSpeed = speed;
            freezeFrameCurve = curve;
        }
    }

    private void FreezeFrameUpdate()
    {
        if(isFreezeFraming)
        {
            freezeFrameCompletion += freezeFrameSpeed * Time.unscaledDeltaTime;
            Time.timeScale = freezeFrameCurve.Evaluate(freezeFrameCompletion);

            if(freezeFrameCompletion >= 1)
            {
                isFreezeFraming = false;
                freezeFrameCompletion = 0f;
                Time.timeScale = 1f;
            }
        }
    }

}
