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

    public override Effect CreateEffect()
    {
        return new StatBoostEffect(this);
    }
}
