using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOverTimeEffect : StatusEffect
{
    public int healPerTurn;

    public HealOverTimeEffect(HealOverTimeEffectData data)
        : base(data)
    {
        healPerTurn = data.healPerTurn;
    }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        Debug.Log("Applying heal over time effect from: " + caster.name + " to: " + target.name);
        base.Apply(caster, target);
    }

    public override void Update()
    {
        Debug.Log("Heal over time effect is updating");
        base.Update();
    }
}
