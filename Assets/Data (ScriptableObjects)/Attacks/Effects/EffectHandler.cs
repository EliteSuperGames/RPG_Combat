using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectHandler
{
    public static event Action<BattleCharacter, int> OnMoveCharacterRequest = delegate { };

    /// <summary>
    /// If the target already has a stun or stat modifier applied, it will just refresh the duration (if the new effect's duration is longer than the existing)
    /// Otherwise, it will add a new effect to the character
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="effectData"></param>
    public static void AddInitialEffectToCharacter(BattleCharacter caster, BattleCharacter target, StatusEffectData effectData)
    {
        EffectInstance existingEffect = target.GetEffectInstance(effectData);
        if (!DetermineAbilitySuccessChance(effectData))
        {
            Debug.Log("Failed to apply status");
            return;
        }
        if (effectData is StunEffectData)
        {
            if (existingEffect != null)
            {
                RefreshSameStatus(existingEffect, effectData);
            }
            else
            {
                target.AddStatusEffect(caster, effectData);
            }
            ProcessStun(target, (StunEffectData)effectData);
        }
        else if (effectData is StatModifierEffectData statModEffectData)
        {
            Debug.Log("Add statModifierEffect to character...: " + statModEffectData.effectName);
            if (existingEffect != null)
            {
                Debug.Log("refresh effect");
                RefreshSameStatus(existingEffect, effectData);
            }
            else
            {
                Debug.Log("Add effect");
                target.AddStatusEffect(caster, effectData);
                StatModifier statModifier = statModEffectData.statModifier;
                statModifier.modifierName = effectData.effectName;
                target.ApplyStatModifier(statModifier);
            }
            // how to access the effectData.statModifier ?
        }
        else if (effectData is MoveSelfEffectData moveSelfEffectData)
        {
            Debug.Log("invoking OnMoveCharacterRequest");
            OnMoveCharacterRequest?.Invoke(target, moveSelfEffectData.moveDistance);
        }
        else if (effectData is MoveTargetEffectData moveTargetEffectData)
        {
            OnMoveCharacterRequest?.Invoke(target, moveTargetEffectData.moveDistance);
        }
        else
        {
            target.AddStatusEffect(caster, effectData);
        }
    }

    private static bool DetermineAbilitySuccessChance(StatusEffectData effectData)
    {
        float successChance = effectData.effectChance;
        float randomNum = UnityEngine.Random.Range(1, 101);
        return randomNum <= successChance;
    }

    private static void RefreshSameStatus(EffectInstance existingEffectInstance, StatusEffectData effectData)
    {
        existingEffectInstance.remainingDuration = Math.Max(existingEffectInstance.remainingDuration, effectData.effectDuration);
    }

    public static void ProcessAllPeriodicEffects(BattleCharacter character)
    {
        foreach (EffectInstance effectInstance in character.StatusEffects)
        {
            ProcessEffect(character, effectInstance);
        }
    }

    /// <summary>
    /// Class called from the BattleCharacter when their turn begins. It will iterate through all effects that they have applied to them
    /// and apply all the various "effects" to the character, whether positive or negative.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="effectInstance"></param>
    public static void ProcessEffect(BattleCharacter character, EffectInstance effectInstance)
    {
        StatusEffectData effectData = effectInstance.effectData;
        switch (effectData)
        {
            case DamageOverTimeEffectData dotEffect:
                ProcessDamageOverTime(character, dotEffect);
                break;
            case HealOverTimeEffectData hotEffect:
                ProcessHealOverTime(character, hotEffect);
                break;
            case StunEffectData stunEffect:
                // ProcessStun(character, stunEffect);
                break;
            case StatModifierEffectData statModEffect:
                // ApplyStatModifier(character, statModEffect, effectInstance);
                break;
            case MoveSelfEffectData moveSelfEffect:
                OnMoveCharacterRequest(character, moveSelfEffect.moveDistance);
                break;
            case MoveTargetEffectData moveTargetEffect:
                OnMoveCharacterRequest(character, moveTargetEffect.moveDistance);
                break;
        }
    }

    /// <summary>
    /// On Character's turn, Will reduce the character's health by the amount of the DOT effect.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="effectData"></param>
    private static void ProcessDamageOverTime(BattleCharacter target, DamageOverTimeEffectData effectData)
    {
        Debug.Log("ProcessDamageOverTime");
        target.TakeDamage(effectData.damagePerRound);
    }

    /// <summary>
    /// On Character's turn, Will restore the character's health by the amount of the HOT effect.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="effectData"></param>
    private static void ProcessHealOverTime(BattleCharacter target, HealOverTimeEffectData effectData)
    {
        target.RestoreHealth(effectData.healthRestoredPerRound);
    }

    /// <summary>
    /// Will stun the character (not sure this is gonna be handled this way. Stun should really only be applied one time, not every turn of its duration)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="effectData"></param>
    public static void ProcessStun(BattleCharacter target, StunEffectData effectData)
    {
        // target.AddStatusEffect(effectData);
        target.SetCurrentState(CharacterState.Stunned);
    }

    private static void ApplyStatModifier(BattleCharacter target, StatModifierEffectData effectData, EffectInstance effectInstance)
    {
        target.ApplyStatModifier(effectData.statModifier);
    }

    public static List<EffectInstance> ReduceDurationForAllEffects(BattleCharacter character, List<EffectInstance> effects)
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            EffectInstance effect = effects[i];
            effect.remainingDuration--;
            if (effect.remainingDuration <= 0)
            {
                Debug.Log("Removing effect: " + effect.effectData.effectName);
                // logic to remove any existing changes to the character related to that effect...
                // remove stun effect
                // remove positive/negative stat mod
                // if ()
                switch (effect.effectData)
                {
                    case DamageOverTimeEffectData damageOverTimeEffectData:
                        break;
                    case StunEffectData stunEffectData:
                        character.SetCurrentState(CharacterState.Normal);
                        break;
                    case StatModifierEffectData statModEffectData:
                        character.RemoveStatModifierEffects(statModEffectData.statModifier);
                        break;
                    case HealOverTimeEffectData healOverTimeEffectData:
                        break;
                }

                effect.remainingDuration = 0;
                effects.RemoveAt(i);
            }
        }

        return effects;
    }
}
