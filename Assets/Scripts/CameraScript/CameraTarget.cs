using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    AnimationCurve initialMovementCurve = default;

    [Header("Min Duration")]
    [SerializeField] float minDuration = 0.1f;
    [SerializeField] float minDurationDistance = 5;

    [Header("Max Duration")]
    [SerializeField] float maxDuration = 0.5f;
    [SerializeField] float maxDurationDistance = 30;

    TimerSystem movementDurationSystem = new TimerSystem();
    Vector3 movementStartFakePos = Vector3.zero;

    Vector3 fakedLocalPosition = default;
    Transform trToFollow = default;

    Action OnCameraMovementEndedEvent = default;

    private void Awake()
    {
        movementDurationSystem.SetUp(OnCameraMovementEnded);
        initialMovementCurve = movementCurve;
    }

    bool isUsingPlayerMovement = default;
    public void SetIsUsingPlayerMovement(bool usingPlayerMovement)
    {
        isUsingPlayerMovement = usingPlayerMovement;
    }

    public void StartMovement(Transform newTrToFollow)
    {
        float distance = movementStartFakePos.magnitude;
        float duration = Mathf.Lerp(minDuration, maxDuration, Mathf.Clamp((maxDurationDistance - distance) / (maxDurationDistance - minDurationDistance), 0, 1));
        StartMovement(newTrToFollow, duration, initialMovementCurve);
    }

    public void StartMovement(Transform newTrToFollow, float forcedDuration, AnimationCurve forcedMovementCurve)
    {
        movementCurve = forcedMovementCurve;

        trToFollow = newTrToFollow;
        movementStartFakePos = transform.position - trToFollow.position;

        float duration = forcedDuration;

        movementDurationSystem.ChangeTimerValue(duration);
        movementDurationSystem.StartTimer();
    }

    public void InstantPlace(Transform newTrToFollow)
    {
        trToFollow = newTrToFollow;
        fakedLocalPosition = Vector3.zero;
        movementDurationSystem.EndTimer();
    }

    public void SetActionToPlayOnEndedMovement(Action OnMovementEnd)
    {
        OnCameraMovementEndedEvent = OnMovementEnd;
    }

    public void OnCameraMovementEnded()
    {
        OnCameraMovementEndedEvent?.Invoke();
        OnCameraMovementEndedEvent = null;
        movementCurve = initialMovementCurve;
    }

    public void Update()
    {
        if (!movementDurationSystem.TimerOver)
        {
            movementDurationSystem.UpdateTimer();
            if (trToFollow == null)
                return;

            fakedLocalPosition = Vector3.Lerp(movementStartFakePos, Vector3.zero, movementCurve.Evaluate(movementDurationSystem.GetTimerCoefficient));
            transform.position = trToFollow.position + fakedLocalPosition;
        }
        else
        {
            SetIsUsingPlayerMovement(trToFollow == GameManager.Instance.GetPlayer.transform && GameManager.Instance.GetPlayerCanAct);
            if (!isUsingPlayerMovement)
            {
                if (trToFollow == null)
                    return;
                transform.position = trToFollow.position;
            }
        }
    }
}
