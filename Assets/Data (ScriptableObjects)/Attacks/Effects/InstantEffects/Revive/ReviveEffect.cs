using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveEffect : Effect
{
    private ReviveEffectData ReviveData => (ReviveEffectData)data;

    public int HealthAfterRevive => ReviveData.healthAfterRevive;

    public ReviveEffect(ReviveEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        ReviveEffectData reviveData = (ReviveEffectData)data;
        target.Revive();
        target.RestoreHealth(reviveData.healthAfterRevive);
    }
}
