using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class Ability
{
    [SerializeField]
    private AbilityData _abilityData;
    public AbilityData AbilityData
    {
        get => _abilityData;
        set => _abilityData = value;
    }

    [SerializeField]
    private int _cooldownTimer;
    public int CooldownTimer
    {
        get => _cooldownTimer;
        set => _cooldownTimer = value;
    }

    [SerializeField]
    public string goddamBitch = "Fuck U";

    [SerializeField]
    public Effect fuckYouBitch;

    [SerializeField]
    public List<Effect> _targetEffects = new List<Effect>();
    public List<Effect> TargetEffects
    {
        get => _targetEffects;
        set => _targetEffects = value;
    }

    [SerializeField]
    public List<Effect> _casterEffects = new List<Effect>();
    public List<Effect> CasterEffects
    {
        get => _casterEffects;
        set => _casterEffects = value;
    }

    public Ability(AbilityData abilityData)
    {
        _abilityData = abilityData;
        _cooldownTimer = 0;
        TargetEffects = new List<Effect>();
        foreach (var effectData in abilityData.targetEffects)
        {
            TargetEffects.Add(effectData.CreateEffect());
        }

        foreach (var effectData in abilityData.casterEffects)
        {
            CasterEffects.Add(effectData.CreateEffect());
        }
    }

    public void Use(BattleCharacter caster, List<BattleCharacter> targets)
    {
        foreach (var target in targets)
        {
            foreach (var effect in TargetEffects)
            {
                if (effect is StatusEffect statusEffect)
                {
                    var clonedEffect = statusEffect.Clone();
                    clonedEffect.Apply(caster, target);
                }
                else
                {
                    effect.Apply(caster, target);
                }
            }

            foreach (var effect in CasterEffects)
            {
                if (effect is StatusEffect statusEffect)
                {
                    var clonedEffect = statusEffect.Clone();
                    clonedEffect.Apply(caster, caster);
                }
                else
                {
                    effect.Apply(caster, caster);
                }
            }
        }
        if (_abilityData.cooldownLength > 0)
        {
            _cooldownTimer = _abilityData.cooldownLength;
        }
    }
}
