using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Boost", menuName = "Attacks/Effects/StatusEffects/Buffs/StatBoost")]
public class StatBoostEffectData : StatusEffectData
{
    public int maxHealthBoost;
    public int attackBoost;
    public int speedBoost;
    public int magicBoost;
    public int magicDefenseBoost;
    public int physicalDefenseBoost;

    public override StatusEffectData Clone()
    {
        return new StatBoostEffectData()
        {
            maxHealthBoost = maxHealthBoost,
            attackBoost = attackBoost,
            speedBoost = speedBoost,
            magicBoost = magicBoost,
            magicDefenseBoost = magicDefenseBoost,
            physicalDefenseBoost = physicalDefenseBoost
        };
    }

    public override Effect CreateEffect()
    {
        return new StatBoostEffect(this);
    }
}
