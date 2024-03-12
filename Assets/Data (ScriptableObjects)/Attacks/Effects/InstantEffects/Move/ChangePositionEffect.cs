using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePositionEffect : Effect
{
    public static event System.Action<BattleCharacter, BattleCharacter> OnChangePosition;

    public ChangePositionEffect(ChangePositionEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        OnChangePosition.Invoke(caster, target);
        // ChangePositionEffectData changePositionData = (ChangePositionEffectData)data;
        // target.SetCurrentBattlePosition(changePositionData.newPosition);
    }
}
