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
    Vector3 movementStartPos = Vector3.zero;

    Transform trToFollow = default;

    public void StartMovement()
    {
        movementStartPos = transform.localPosition;
        float distance = movementStartPos.magnitude;

        float duration = Mathf.Lerp(minDuration, maxDuration, Mathf.Clamp((maxDurationDistance - distance)/ (maxDurationDistance - minDurationDistance), 0, 1));

        movementDurationSystem.ChangeTimerValue(duration);
        movementDurationSystem.StartTimer();
    }

    public void Update()
    {
        if (!movementDurationSystem.TimerOver)
        {
            movementDurationSystem.UpdateTimer();
            transform.localPosition = Vector3.Lerp(movementStartPos, Vector3.zero, movementCurve.Evaluate(movementDurationSystem.GetTimerCoefficient));
        }
    }
}
