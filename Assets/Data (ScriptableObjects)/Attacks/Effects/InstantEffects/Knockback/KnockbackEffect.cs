using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackEffect : Effect
{
    private KnockbackEffectData KnockbackData => (KnockbackEffectData)data;

    public float KnockbackPower => KnockbackData.knockbackPower;

    public KnockbackEffect(KnockbackEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        KnockbackEffectData knockbackData = (KnockbackEffectData)data;
        Debug.Log("Knockback effect applied to " + target.name + " with power " + knockbackData.knockbackPower);
        // TODO: Come up with how to knock back targets
    }
}
