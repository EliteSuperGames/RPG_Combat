using UnityEngine;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Attacks/Effects/InstantEffects/Heal")]
public class HealEffectData : EffectData
{
    [SerializeField]
    public int healingPower;

    public override Effect CreateEffect()
    {
        return new HealEffect(this);
    }
}
