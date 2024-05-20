using UnityEngine;

[CreateAssetMenu(fileName = "FireEffectData", menuName = "Attacks/Effects/StatusEffects/Damage/Fire")]
public class FireEffectData : StatusEffectData
{
    [SerializeField]
    public int damagePerTurn;

    public override StatusEffectData Clone()
    {
        var clonedData = CreateInstance<FireEffectData>();

        clonedData.damagePerTurn = damagePerTurn;

        clonedData.duration = duration;
        clonedData.isStackable = isStackable;
        clonedData.isRenewable = isRenewable;
        clonedData.dispelResistance = dispelResistance;
        clonedData.applicationChance = applicationChance;
        clonedData.effectDescription = effectDescription;
        clonedData.effectName = effectName;
        clonedData.name = name;
        clonedData.effectType = effectType;
        return clonedData;
    }

    public override Effect CreateEffect()
    {
        return new FireEffect(this);
    }
}
