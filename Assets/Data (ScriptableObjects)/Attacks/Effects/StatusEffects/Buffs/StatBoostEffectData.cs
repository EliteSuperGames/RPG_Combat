using UnityEngine;

[CreateAssetMenu(fileName = "Stat Boost", menuName = "Attacks/Effects/StatusEffects/Buffs/StatBoost")]
public class StatBoostEffectData : StatusEffectData
{
    [SerializeField]
    public int maxHealthBoost;

    [SerializeField]
    public int attackBoost;

    [SerializeField]
    public int speedBoost;

    [SerializeField]
    public int magicBoost;

    [SerializeField]
    public int magicDefenseBoost;

    [SerializeField]
    public int physicalDefenseBoost;

    public override StatusEffectData Clone()
    {
        var clonedData = CreateInstance<StatBoostEffectData>();

        clonedData.maxHealthBoost = maxHealthBoost;
        clonedData.attackBoost = attackBoost;
        clonedData.speedBoost = speedBoost;
        clonedData.magicBoost = magicBoost;
        clonedData.magicDefenseBoost = magicDefenseBoost;
        clonedData.physicalDefenseBoost = physicalDefenseBoost;

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
        return new StatBoostEffect(this);
    }
}
