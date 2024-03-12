using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public EffectData data;

    public Effect(EffectData data)
    {
        this.data = data;
    }

    public abstract void Apply(BattleCharacter caster, BattleCharacter target);
}
