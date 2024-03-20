using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Poison Effect", menuName = "Attacks/Effects/StatusEffects/Damage/Poison")]
public class PoisonEffectData : StatusEffectData
{
    public int damagePerTurn;

    public override StatusEffectData Clone()
    {
        return new PoisonEffectData() { damagePerTurn = damagePerTurn };
    }

    public override Effect CreateEffect()
    {
        return new PoisonEffect(this);
    }
}
