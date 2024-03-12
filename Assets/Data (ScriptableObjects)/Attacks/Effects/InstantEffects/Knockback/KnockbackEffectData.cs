using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnockbackEffectData", menuName = "Attacks/Effects/InstantEffects/Knockback")]
public class KnockbackEffectData : EffectData
{
    public float knockbackPower;

    public override Effect CreateEffect()
    {
        return new KnockbackEffect(this);
    }
}
