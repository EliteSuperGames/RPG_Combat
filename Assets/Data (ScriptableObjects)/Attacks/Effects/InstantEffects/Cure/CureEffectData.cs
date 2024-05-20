using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Cure Effect", menuName = "Attacks/Effects/InstantEffects/Cure")]
public class CureEffectData : EffectData
{
    [SerializeField]
    public List<EffectType> TypesOfEffectsCured;

    public override Effect CreateEffect()
    {
        return new CureEffect(this);
    }
}
