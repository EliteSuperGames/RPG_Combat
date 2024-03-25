using UnityEngine;

[CreateAssetMenu(fileName = "Stat Debuff", menuName = "Attacks/Effects/StatusEffects/Debuffs/StatDebuff")]
public class StatDebuffEffectData : StatusEffectData
{
    [SerializeField]
    public int maxHealthDebuff;

    [SerializeField]
    public int attackDebuff;

    [SerializeField]
    public int speedDebuff;

    [SerializeField]
    public int magicDebuff;

    [SerializeField]
    public int magicDefenseDebuff;

    [SerializeField]
    public int physicalDefenseDebuff;

    public override StatusEffectData Clone()
    {
        var clonedData = CreateInstance<StatDebuffEffectData>();

        clonedData.maxHealthDebuff = maxHealthDebuff;
        clonedData.attackDebuff = attackDebuff;
        clonedData.speedDebuff = speedDebuff;
        clonedData.magicDebuff = magicDebuff;
        clonedData.magicDefenseDebuff = magicDefenseDebuff;
        clonedData.physicalDefenseDebuff = physicalDefenseDebuff;

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
        return new StatDebuffEffect(this);
    }
}
