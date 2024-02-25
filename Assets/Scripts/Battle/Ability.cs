using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    private AbilityData abilityData;
    private float cooldownTimer;
    public AbilityData AbilityData
    {
        get => abilityData;
        set => abilityData = value;
    }
    public float CooldownTimer
    {
        get => cooldownTimer;
        set => cooldownTimer = value;
    }

    public const int PlaceholderPowerModifier = 2;

    public void ExecuteAbility(BattleCharacter caster, BattleCharacter target)
    {
        // MoveCharacter(caster, target);
        ApplyDamage(caster, target);
        ApplyEffects(caster, target);
        ApplyHealing(caster, target);
    }

    private void ApplyEffects(BattleCharacter caster, BattleCharacter target)
    {
        if (AbilityData.effects.Count > 0)
        {
            foreach (var effect in AbilityData.effects)
            {
                EffectHandler.AddEffectToCharacter(caster, target, effect);
            }
        }
    }

    private void ApplyHealing(BattleCharacter caster, BattleCharacter target)
    {
        if (AbilityData.abilityTypes.Contains(AbilityType.Heal))
        {
            target.RestoreHealth(AbilityData.baseAbilityPower);
        }
    }

    public void ApplyDamage(BattleCharacter caster, BattleCharacter target)
    {
        if (AbilityData.abilityTypes.Contains(AbilityType.Damage))
        {
            int damageAmount;
            if (AbilityData.baseAbilityPower > 0)
            {
                damageAmount = MakeComplicatedCalculationToGetDamageAmount(caster, target);
                target.TakeDamage(damageAmount);
            }
        }
    }

    private int MakeComplicatedCalculationToGetDamageAmount(BattleCharacter caster, BattleCharacter target)
    {
        int damage = AbilityData.baseAbilityPower + (caster.CharData.MagicPower * PlaceholderPowerModifier);
        damage = Mathf.Max(0, damage);
        return damage;
    }

    public List<StatusEffectData> GetStatusEffectData()
    {
        return AbilityData.effects;
    }
}
