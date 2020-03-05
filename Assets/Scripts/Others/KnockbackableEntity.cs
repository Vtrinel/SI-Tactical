using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackableEntity : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] Transform transformToMove = default;
    [SerializeField] AnimationCurve knockbackAttenuationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] DamageTag damageTag = DamageTag.Player;

    float currentStartKnockbackForce = 0;
    float currentKnockbackForce = 0;

    TimerSystem knockbackDurationSystem = new TimerSystem();
    TimerSystem knockbackAttenuationDurationSystem = new TimerSystem();

    Vector3 currentKnockbackDirection = Vector3.forward;
    public void ReceiveKnockback(DamageTag knockbackTag, KnockbackParameters knockbackParams, Vector3 dir)
    {
        if (knockbackTag != DamageTag.Environment && knockbackTag == damageTag)
            return;

        currentStartKnockbackForce = knockbackParams.knockbackForce;
        currentKnockbackForce = currentStartKnockbackForce;

        knockbackDurationSystem.ChangeTimerValue(knockbackParams.knockbackDuration);
        knockbackDurationSystem.StartTimer();
        knockbackAttenuationDurationSystem.ChangeTimerValue(knockbackParams.knockbackAttenuationDuration);
        knockbackAttenuationDurationSystem.StartTimer();

        currentKnockbackDirection = dir;
    }

    private void Update()
    {
        if (!knockbackDurationSystem.TimerOver)
        {
            UpdateKnockbackMovement();

            knockbackDurationSystem.UpdateTimer();
        }
        else if (!knockbackAttenuationDurationSystem.TimerOver)
        {
            UpdateKnockbackMovement();

            knockbackAttenuationDurationSystem.UpdateTimer();
            currentKnockbackForce = Mathf.Lerp(currentStartKnockbackForce, 0, knockbackAttenuationCurve.Evaluate(knockbackAttenuationDurationSystem.GetTimerCoefficient));
        }
    }

    public void UpdateKnockbackMovement()
    {
        transformToMove.position += currentKnockbackDirection * currentKnockbackForce * Time.deltaTime;
    }
}


[System.Serializable]
public struct KnockbackParameters
{
    public KnockbackParameters(float force, float duration, float attenuationDuration)
    {
        knockbackForce = force;
        knockbackDuration = duration;
        knockbackAttenuationDuration = attenuationDuration;
    }

    public float knockbackForce;
    public float knockbackDuration;
    public float knockbackAttenuationDuration;
}

