using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectZone : MonoBehaviour
{
    [SerializeField] EffectZoneType effectZoneType = EffectZoneType.PlayerRage;
    public EffectZoneType GetEffectZoneType => effectZoneType;
    public void SetEffectZoneType(EffectZoneType zoneType)
    {
        effectZoneType = zoneType;
    }

    [Header("Dimensions")]
    [SerializeField] LayerMask effectMask = ~0;
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 5f;
    [SerializeField] float growDuration = 0.1f;
    [SerializeField] float persistanceDuration = 0.05f;
    TimerSystem growingDurationSystem = new TimerSystem();
    TimerSystem persistanceDurationSystem = new TimerSystem();
    float currentRadius = 0f;

    [Header("Effects : Damaging")]
    [SerializeReference] DamageTag sourceTag = DamageTag.Player;
    [SerializeField] bool damaging = false;
    [SerializeField] int damageAmount = 1;
    [Header("Effects : Knockback")]
    [SerializeField] bool knockbacking = false;
    [SerializeField] KnockbackParameters maxKnockbackParameters = new KnockbackParameters(20f, 0.05f, 0.2f, false);
    [SerializeField] float maxKnockbackDistance = 2f;
    [SerializeField] KnockbackParameters minKnockbackParameters = new KnockbackParameters(10f, 0.05f, 0.2f, false);
    [SerializeField] float minKnockbackDistance = 5f;
    [SerializeField] AnimationCurve knockbackDependingOnDistanceAttenuationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [Header("Effects : Stun")]
    [SerializeField] int numberOfStunedTurns = 0;

    [Header("Feedbacks")]
    [SerializeField] Sound soundToPlayOnStart = Sound.ExplosionDisc;
    [SerializeField] FxType fxToPlayOnStart = FxType.none;

    [Header("Debug")]
    [SerializeField] MeshRenderer debugZoneRenderer = default;

    public void StartZone(Vector3 pos)
    {
        alreadyHitColliders = new List<Collider>();

        transform.position = pos;

        growingDurationSystem.ChangeTimerValue(growDuration);
        growingDurationSystem.StartTimer();

        persistanceDurationSystem.ChangeTimerValue(persistanceDuration);
        persistanceDurationSystem.SetUp(EndZone);
        persistanceDurationSystem.StartTimer();

        SoundManager.Instance.PlaySound(soundToPlayOnStart, transform.position);
        FxManager.Instance.CreateFx(fxToPlayOnStart, transform.position);

        UpdateRadius();
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
                    hitDamageable.ReceiveDamage(sourceTag, new DamagesParameters(damageAmount, numberOfStunedTurns));
            }

            if (knockbacking)
            {
                KnockbackableEntity hitKnockable = hitCollider.GetComponent<KnockbackableEntity>();
                if(hitKnockable != null)
                {
                    Vector3 hitPosition = hitCollider.transform.position;
                    Vector3 knockbackDirection = hitCollider.transform.position - transform.position;
                    knockbackDirection.y = 0;
                    knockbackDirection.Normalize();

                    hitKnockable.ReceiveKnockback(sourceTag, GetKnockbackParametersWithDistance(hitPosition), knockbackDirection);
                }
            }
        }
    }

    public KnockbackParameters GetKnockbackParametersWithDistance(Vector3 hitKnockablePosition)
    {
        hitKnockablePosition.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, hitKnockablePosition);

        float coeff = Mathf.Clamp((maxKnockbackDistance - distance) /(maxKnockbackDistance - minKnockbackDistance), 0, 1);

        coeff = knockbackDependingOnDistanceAttenuationCurve.Evaluate(coeff);

        return KnockbackParameters.Lerp(maxKnockbackParameters, minKnockbackParameters, coeff);
    }

    public void UpdateRadius()
    {
        currentRadius = Mathf.Lerp(startRadius, endRadius, !growingDurationSystem.TimerOver ? growingDurationSystem.GetTimerCoefficient : 1);

        if (debugZoneRenderer != null)
            debugZoneRenderer.transform.localScale = Vector3.one * currentRadius * 2;
    }

    public void EndZone()
    {
        EffectZonesManager.Instance.ReturnEffectZoneInPool(this);
    }
}
