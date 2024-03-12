using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffectData", menuName = "Attacks/Effects/InstantEffects/Damage")]
public class DamageEffectData : EffectData
{
    public int power;

    public override Effect CreateEffect()
    {
        return new DamageEffect(this);
    }
}
