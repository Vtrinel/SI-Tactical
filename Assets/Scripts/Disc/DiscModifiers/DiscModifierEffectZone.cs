using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Effect Zone Modifier", menuName = "Disc Modifiers/Effect Zone")]
public class DiscModifierEffectZone : DiscModifier
{
    [SerializeField] EffectZoneType effectZoneToCreateOnHit = EffectZoneType.ExplosiveDisc;
    public EffectZoneType GetEffectZoneToCreateOnHit => effectZoneToCreateOnHit;
    [SerializeField] bool destroyProjectileOnHit = true;
    public bool GetDiscProjectileOnHit => destroyProjectileOnHit;
}
