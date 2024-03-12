using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : Effect
{
    private DamageEffectData DamageData => (DamageEffectData)data;

    public int Power => DamageData.power;

    public DamageEffect(DamageEffectData data)
        : base(data) { }

    // instantly applies damage to target
    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        Debug.Log("DamageEffect apply");
        DamageEffectData damageData = (DamageEffectData)data;
        Debug.Log("damageData.power: " + damageData.power);
        target.TakeDamage(damageData.power);
    }
}
