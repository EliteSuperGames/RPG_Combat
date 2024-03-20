using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealOverTimeEffectData", menuName = "Attacks/Effects/StatusEffects/HealOverTime")]
public class HealOverTimeEffectData : StatusEffectData
{
    public int healPerTurn;

    public override StatusEffectData Clone()
    {
        return new HealOverTimeEffectData() { healPerTurn = healPerTurn };
    }

    public override Effect CreateEffect()
    {
        return new HealOverTimeEffect(this);
    }
}
