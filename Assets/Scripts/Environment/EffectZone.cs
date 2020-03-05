using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectZone : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] LayerMask effectMask = ~0;
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 5f;
    [SerializeField] float growDuration = 0.1f;
    [SerializeField] float persistanceDuration = 0.05f;
    TimerSystem growingDurationSystem = new TimerSystem();
    TimerSystem persistanceDurationSystem = new TimerSystem();
    float currentRadius = 0f;

    [Header("Effects")]
    [SerializeReference] DamageTag sourceTag = DamageTag.Player;
    [SerializeField] bool damaging = false;
    [SerializeField] int damageAmount = 1;
    [SerializeField] bool knockbacking = false;
    [SerializeField] KnockbackParameters knockbackParameters = new KnockbackParameters();

    [Header("Debug")]
    [SerializeField] MeshRenderer debugZoneRenderer = default;

    public void StartZone(Vector3 pos)
    {
        alreadyHitColliders = new List<Collider>();

        transform.position = pos;

        growingDurationSystem.ChangeTimerValue(growDuration);
        growingDurationSystem.StartTimer();

        persistanceDurationSystem.ChangeTimerValue(persistanceDuration);
        persistanceDurationSystem.SetUp(DestroyZone);
        persistanceDurationSystem.StartTimer();

    }

    private void Update()
    {
        if (!growingDurationSystem.TimerOver)
        {
            growingDurationSystem.UpdateTimer();
            UpdateRadius();
            CastEffectZone();
        }
        else if (!persistanceDurationSystem.TimerOver)
        {
            persistanceDurationSystem.UpdateTimer();
            CastEffectZone();
        }
    }

    List<Collider> alreadyHitColliders = new List<Collider>();
    public void CastEffectZone()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentRadius, effectMask);
        foreach(Collider hitCollider in hitColliders)
        {
            if (alreadyHitColliders.Contains(hitCollider))
                continue;

            alreadyHitColliders.Add(hitCollider);

            if (damaging)
            {
                DamageableEntity hitDamageable = hitCollider.GetComponent<DamageableEntity>();
                if (hitDamageable != null)
                    hitDamageable.ReceiveDamage(sourceTag, damageAmount);
            }

            if (knockbacking)
            {
                KnockbackableEntity hitKnockable = hitCollider.GetComponent<KnockbackableEntity>();
                if(hitKnockable != null)
                {
                    Vector3 knockbackDirection = hitCollider.transform.position - transform.position;
                    knockbackDirection.y = 0;
                    knockbackDirection.Normalize();

                    hitKnockable.ReceiveKnockback(sourceTag, knockbackParameters, knockbackDirection);
                }
            }
        }
    }

    public void UpdateRadius()
    {
        currentRadius = Mathf.Lerp(startRadius, endRadius, !growingDurationSystem.TimerOver ? growingDurationSystem.GetTimerCoefficient : 1);

        if (debugZoneRenderer != null)
            debugZoneRenderer.transform.localScale = Vector3.one * currentRadius * 2;
    }

    public void DestroyZone()
    {
        Destroy(gameObject);
    }
}
