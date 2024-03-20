using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Debuff", menuName = "Attacks/Effects/StatusEffects/Debuffs/StatDebuff")]
public class StatDebuffEffectData : StatusEffectData
{
    public int maxHealthDebuff;
    public int attackDebuff;
    public int speedDebuff;
    public int magicDebuff;
    public int magicDefenseDebuff;
    public int physicalDefenseDebuff;

    public override StatusEffectData Clone()
    {
        return new StatDebuffEffectData()
        {
            maxHealthDebuff = maxHealthDebuff,
            attackDebuff = attackDebuff,
            speedDebuff = speedDebuff,
            magicDebuff = magicDebuff,
            magicDefenseDebuff = magicDefenseDebuff,
            physicalDefenseDebuff = physicalDefenseDebuff
        };
    }

    public override Effect CreateEffect()
    {
        return new StatDebuffEffect(this);
    }
}
