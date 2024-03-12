using UnityEngine;

[CreateAssetMenu(fileName = "PullEffectData", menuName = "Attacks/Effects/InstantEffects/Pull")]
public class PullEffectData : StatusEffectData
{
    [Range(0, 3)]
    public int pullDistance;

    public override Effect CreateEffect()
    {
        return new PullEffect(this);
    }
}
