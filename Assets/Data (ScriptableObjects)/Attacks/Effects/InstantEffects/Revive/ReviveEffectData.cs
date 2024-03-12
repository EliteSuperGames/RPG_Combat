using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReviveEffectData", menuName = "Attacks/Effects/InstantEffects/Revive")]
public class ReviveEffectData : EffectData
{
    public int healthAfterRevive;

    public override Effect CreateEffect()
    {
        return new ReviveEffect(this);
    }
}
