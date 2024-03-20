using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireEffectData", menuName = "Attacks/Effects/StatusEffects/Damage/Fire")]
public class FireEffectData : StatusEffectData
{
    public int damagePerTurn;

    public override StatusEffectData Clone()
    {
        var clonedData = CreateInstance<FireEffectData>();
        clonedData.duration = duration;
        clonedData.name = name;
        clonedData.applicationChance = applicationChance;
        clonedData.isStackable = isStackable;
        clonedData.isRenewable = isRenewable;
        clonedData.dispelResistance = dispelResistance;
        clonedData.damagePerTurn = damagePerTurn;
        clonedData.effectDescription = effectDescription;
        clonedData.effectName = effectName;

        return clonedData;
    }

    public override Effect CreateEffect()
    {
        return new FireEffect(this);
    }
}
