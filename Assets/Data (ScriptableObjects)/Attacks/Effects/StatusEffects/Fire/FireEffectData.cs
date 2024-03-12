using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireEffectData", menuName = "Attacks/Effects/StatusEffects/Damage/Fire")]
public class FireEffectData : StatusEffectData
{
    public int damagePerTurn;

    public override Effect CreateEffect()
    {
        return new FireEffect(this);
    }
}
