using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullEffect : Effect
{
    private PullEffectData PullData => (PullEffectData)data;

    public int PullDistance => PullData.pullDistance;

    public PullEffect(PullEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        PullEffectData pullData = (PullEffectData)data;
        Debug.Log(pullData.pullDistance);
    }
}
