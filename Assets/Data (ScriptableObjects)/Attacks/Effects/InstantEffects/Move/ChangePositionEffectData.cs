using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChangePositionEffectData", menuName = "Attacks/Effects/InstantEffects/Move/ChangePositionEffectData")]
public class ChangePositionEffectData : EffectData
{
    public override Effect CreateEffect()
    {
        return new ChangePositionEffect(this);
    }
}
