using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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

    bool isUsingPlayerMovement = default;
    public void SetIsUsingPlayerMovement(bool usingPlayerMovement)
    {
        isUsingPlayerMovement = usingPlayerMovement;
    }

    public void StartMovement(Transform newTrToFollow)
    {
        trToFollow = newTrToFollow;
        movementStartFakePos = transform.position - trToFollow.position;
        float distance = movementStartFakePos.magnitude;

        float duration = Mathf.Lerp(minDuration, maxDuration, Mathf.Clamp((maxDurationDistance - distance)/ (maxDurationDistance - minDurationDistance), 0, 1));

        movementDurationSystem.ChangeTimerValue(duration);
        movementDurationSystem.StartTimer();
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
