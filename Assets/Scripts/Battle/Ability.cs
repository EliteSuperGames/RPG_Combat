using System.Collections.Generic;

[System.Serializable]
public class Ability
{
    private AbilityData _abilityData;
    public AbilityData AbilityData
    {
        get => _abilityData;
        set => _abilityData = value;
    }

    private int _cooldownTimer;
    public int CooldownTimer
    {
        get => _cooldownTimer;
        set => _cooldownTimer = value;
    }

    private List<Effect> _targetEffects = new List<Effect>();
    public List<Effect> TargetEffects
    {
        get => _targetEffects;
        private set => _targetEffects = value;
    }
    private List<Effect> _casterEffects = new List<Effect>();
    public List<Effect> CasterEffects
    {
        get => _casterEffects;
        private set => _casterEffects = value;
    }

    public Ability(AbilityData abilityData)
    {
        this._abilityData = abilityData;
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
                effect.Apply(caster, target);
            }

            foreach (var effect in CasterEffects)
            {
                effect.Apply(caster, caster);
            }
        }
        if (_abilityData.cooldownLength > 0)
        {
            _cooldownTimer = _abilityData.cooldownLength;
        }
    }
}
