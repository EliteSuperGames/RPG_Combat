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

    public override Effect CreateEffect()
    {
        return new StatDebuffEffect(this);
    }
}
