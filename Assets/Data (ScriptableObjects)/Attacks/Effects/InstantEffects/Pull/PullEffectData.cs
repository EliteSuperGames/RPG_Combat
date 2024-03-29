using UnityEngine;

[CreateAssetMenu(fileName = "PullEffectData", menuName = "Attacks/Effects/InstantEffects/Pull")]
public class PullEffectData : EffectData
{
    [Range(0, 3)]
    public int pullDistance;

    public override Effect CreateEffect()
    {
        return new PullEffect(this);
    }
}
