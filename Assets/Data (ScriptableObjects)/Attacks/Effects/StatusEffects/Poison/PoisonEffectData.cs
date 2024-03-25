using UnityEngine;

[CreateAssetMenu(fileName = "New Poison Effect", menuName = "Attacks/Effects/StatusEffects/Damage/Poison")]
public class PoisonEffectData : StatusEffectData
{
    [SerializeField]
    public int damagePerTurn;

    public override StatusEffectData Clone()
    {
        var clonedData = CreateInstance<PoisonEffectData>();

        clonedData.damagePerTurn = damagePerTurn;

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
        return new PoisonEffect(this);
    }
}
