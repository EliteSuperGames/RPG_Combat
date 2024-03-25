using UnityEngine;

[CreateAssetMenu(fileName = "HealOverTimeEffectData", menuName = "Attacks/Effects/StatusEffects/HealOverTime")]
public class HealOverTimeEffectData : StatusEffectData
{
    [SerializeField]
    public int healPerTurn;

    public override StatusEffectData Clone()
    {
        var clonedData = CreateInstance<HealOverTimeEffectData>();
        clonedData.healPerTurn = healPerTurn;

        clonedData.duration = duration;
        clonedData.isStackable = isStackable;
        clonedData.isRenewable = isRenewable;
        clonedData.dispelResistance = dispelResistance;
        clonedData.applicationChance = applicationChance;
        clonedData.effectDescription = effectDescription;
        clonedData.effectName = effectName;
        clonedData.name = name;

        return clonedData;
    }

    public override Effect CreateEffect()
    {
        return new HealOverTimeEffect(this);
    }
}
