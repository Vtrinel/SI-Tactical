using System;
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

    public Action<Vector3> OnKnockbackUpdate = default;

    Vector3 currentKnockbackDirection = Vector3.forward;
    public void ReceiveKnockback(DamageTag knockbackTag, KnockbackParameters knockbackParams, Vector3 dir)
    {
        if (knockbackTag != DamageTag.Environment && knockbackTag == damageTag)
            return;

        if (damageTag == DamageTag.Disc && !knockbackParams.canKnockbackDiscs)
            return;

        if (knockbackParams.IsNull)
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
        Vector3 knockbackMovement = currentKnockbackDirection * currentKnockbackForce * Time.deltaTime;
        if (OnKnockbackUpdate == null)
            transformToMove.position += knockbackMovement;
        else
            OnKnockbackUpdate.Invoke(knockbackMovement);
    }
}


[System.Serializable]
public struct KnockbackParameters
{
    public KnockbackParameters(float force, float duration, float attenuationDuration, bool canKnockDiscs)
    {
        knockbackForce = force;
        knockbackDuration = duration;
        knockbackAttenuationDuration = attenuationDuration;
        canKnockbackDiscs = canKnockDiscs;
    }

    public static KnockbackParameters Lerp(KnockbackParameters a, KnockbackParameters b, float coeff)
    {
        return new KnockbackParameters(
            Mathf.Lerp(a.knockbackForce, b.knockbackForce, coeff),
            Mathf.Lerp(a.knockbackDuration, b.knockbackDuration, coeff),
            Mathf.Lerp(a.knockbackAttenuationDuration, b.knockbackAttenuationDuration, coeff),
            a.canKnockbackDiscs);
    }

    public float knockbackForce;
    public float knockbackDuration;
    public float knockbackAttenuationDuration;
    public bool canKnockbackDiscs;

    public bool IsNull => knockbackForce == 0 || (knockbackDuration == 0 && knockbackAttenuationDuration == 0);
}

