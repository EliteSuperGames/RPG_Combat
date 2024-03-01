using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public Ability(AbilityData abilityData)
    {
        this.abilityData = abilityData;
        cooldownTimer = 0;
    }

    /// <summary>
    /// Executes the ability by applying damage, effects, and healing to the target character.
    /// </summary>
    /// <param name="caster">The character casting the ability.</param>
    /// <param name="target">The character being targeted by the ability.</param>
    public void ExecuteAbility(BattleCharacter caster, BattleCharacter target)
    {
        Debug.Log("EXECUTE ABILITY" + AbilityData.abilityName);
        // MoveCharacter(caster, target);
        ApplyDamage(caster, target);
        ApplyEffects(caster, target);
        ApplyHealing(caster, target);
        MoveSelf(caster);
        // MoveSelf(caster);
        // MoveTarget(target);
    }

    private void MoveSelf(BattleCharacter caster)
    {
        // probably need to pass in the list of characters on the same faction so you can moveself to them
        Debug.Log("Ability.MoveSelf");

        if (AbilityData.effects.Count > 0)
        {
            foreach (var effect in AbilityData.effects)
            {
                Debug.Log("Effect: " + effect.effectName);
                if (effect is MoveSelfEffectData)
                {
                    Debug.Log("calling EffectHandler.AddInitialEffectToCharacter");
                    // EffectHandler.AddInitialEffectToCharacter(caster, caster, effect);
                }
            }
        }
    }

    private void MoveTarget(BattleCharacter target)
    {
        if (AbilityData.effects.Count > 0)
        {
            foreach (var effect in AbilityData.effects)
            {
                if (effect is MoveTargetEffectData)
                {
                    EffectHandler.AddInitialEffectToCharacter(target, target, effect);
                }
            }
        }
    }

    /// <summary>
    /// Applies the effects of the ability to the specified caster and target.
    /// </summary>
    /// <param name="caster">The character casting the ability.</param>
    /// <param name="target">The character being targeted by the ability.</param>
    private void ApplyEffects(BattleCharacter caster, BattleCharacter target)
    {
        if (AbilityData.abilityName == "Change Positions")
        {
            EffectHandler.RequestMove(caster, target);
        }
        if (AbilityData.effects.Count > 0)
        {
            foreach (var effect in AbilityData.effects)
            {
                EffectHandler.AddInitialEffectToCharacter(caster, target, effect);
            }
        }
    }

    private void ApplyHealing(BattleCharacter caster, BattleCharacter target)
    {
        if (AbilityData.effects.Any(effect => effect.effectType == EffectType.HealOverTime))
        {
            target.RestoreHealth(AbilityData.baseAbilityPower);
        }
        // if (AbilityData.abilityTypes.Contains(AbilityType.Heal))
        // {
        //     target.RestoreHealth(AbilityData.baseAbilityPower);
        // }
    }

    public void ApplyDamage(BattleCharacter caster, BattleCharacter target)
    {
        if (AbilityData.effects.Any(effect => effect.effectType == EffectType.DamageOverTime))
        {
            int damageAmount;
            if (AbilityData.baseAbilityPower > 0)
            {
                damageAmount = MakeComplicatedCalculationToGetDamageAmount(caster, target);
                target.TakeDamage(damageAmount);
            }
        }
    }

    /// <summary>
    /// Makes a complicated calculation to determine the damage amount inflicted by the ability.
    /// </summary>
    /// <param name="caster">The battle character casting the ability.</param>
    /// <param name="target">The battle character being targeted by the ability.</param>
    /// <returns>The calculated damage amount.</returns>
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
