using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetSelectionHandler
{
    public static event Action<List<BattlePosition>> OnActiveTargetsFound = delegate { };

    public static List<EligibleAbility> DetermineAvailableAbilitiesBasedOnPosition(
        BattleCharacter battleCharacter,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions
    )
    {
        List<EligibleAbility> eligibleAbilities = new List<EligibleAbility>();

        foreach (var ability in battleCharacter.Abilities)
        {
            EligibleAbility eligibleAbility = new EligibleAbility(ability)
            {
                EligibleBasedOnLaunchPosition = true,
                // // EligibleBasedOnLaunchPosition = ability.AbilityData.launchPositions.Contains(battleCharacter.BattlePosition.PositionNumber),
                EligibleBasedOnLandingTargets = DoesLandingPositionHaveValidTarget(battleCharacter, ability, playerPositions, enemyPositions)
            };

            Debug.Log("eligibleAbility: " + eligibleAbility.Ability.AbilityData.abilityName);
            eligibleAbilities.Add(eligibleAbility);
        }
        Debug.Log("Returning these: " + eligibleAbilities.Count);
        return eligibleAbilities;
    }

    public static void HideAllTargetIndicators(List<BattlePosition> allPositions)
    {
        foreach (var position in allPositions)
        {
            position.HideTargetableIndicator();
        }
    }

    public static void HideTargetIndicators(List<BattlePosition> allPositions)
    {
        foreach (var pos in allPositions)
        {
            pos.HideTargetableIndicator();
            pos.HideTransparentTarget();
        }
    }

    private static bool DoesLandingPositionHaveValidTarget(
        BattleCharacter caster,
        Ability ability,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions
    )
    {
        IEnumerable<BattlePosition> targetList;

        if (caster.PlayerCharacter)
        {
            targetList = enemyPositions;
        }
        else
        {
            targetList = playerPositions;
        }
        if (ability.AbilityData.onlyTargetUnconscious)
        {
            return targetList.Any(pos => pos.BattleCharacter.CurrentState == CharacterState.Unconscious);
        }

        // if caster is in positions 0, 1, or 2, then short range ability can target enemy positions 0, 1, 2
        // if caster is in positions 3, 4, or 5, then short range ability cannot be used

        // if caster is in positions 0, 1, or 2, then medium range ability can target any enemy position
        // if caster is in positions 3, 4, or 5, then medium range ability can target enemy positions 0, 1, 2

        // long range ability can target any position from any position

        if (ability.AbilityData.abilityRange == AbilityRange.Short)
        {
            Debug.Log("Short range ability");
            if (caster.BattlePosition.PositionNumber < 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (ability.AbilityData.abilityRange == AbilityRange.Medium)
        {
            return true;
        }
        else if (ability.AbilityData.abilityRange == AbilityRange.Long)
        {
            return true;
        }
        else if (ability.AbilityData.abilityRange == AbilityRange.Inherit)
        {
            if (caster.CharData.Range == Range.Short && caster.BattlePosition.PositionNumber > 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    public static void OnTargetHover(
        List<BattlePosition> currentValidTargets,
        BattleCharacter hoverTarget,
        bool isAlly,
        ref BattleCharacter target,
        BattleCharacter caster,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions,
        // ref UI_ActionPanel actionPanel,
        BattleUIParent battleUIParent,
        Ability SelectedAbility
    )
    {
        Debug.Log("OnTargetHover");
        Debug.Log("active character: " + caster.CharData.CharacterName);
        Debug.Log("hover target: " + hoverTarget.CharData.CharacterName);
        Debug.Log("selected ability: " + SelectedAbility.AbilityData.abilityName);
        if (target != null)
        {
            target.BattlePosition.HideTransparentTarget();
        }

        IEnumerable<BattlePosition> positionList;

        if (SelectedAbility.AbilityData.targetFaction == TargetFaction.Allies)
        {
            positionList = caster.PlayerCharacter ? playerPositions : enemyPositions;
        }
        else
        {
            positionList = caster.PlayerCharacter ? enemyPositions : playerPositions;
        }
        target = hoverTarget;

        battleUIParent.SetTargetCharacterData(caster, target);

        // if it is a moveSelf ability, show the transparent target indicator
        // if (SelectedAbility.AbilityData.effects.Any(effect => effect.effectType == EffectType.MoveSelf))
        // {
        //     foreach (var position in positionList)
        //     {
        //         if (currentValidTargets.Contains(position) && position.PositionNumber == activeTarget.BattlePosition.PositionNumber)
        //         {
        //             position.EnableTransparentTarget(!isAlly);
        //         }
        //     }
        //     return;
        // }
        // else
        if (currentValidTargets.Contains(hoverTarget.BattlePosition))
        {
            if (SelectedAbility.AbilityData.areaOfEffect != AreaOfEffect.Single)
            // if (SelectedAbility.AbilityData.areaOfEffect == AreaOfEffect. == TargetingType.Multiple)
            {
                foreach (var position in positionList)
                {
                    if (currentValidTargets.Contains(position))
                    {
                        position.EnableTransparentTarget(!isAlly);
                    }
                }
            }
            else
            {
                hoverTarget.BattlePosition.EnableTransparentTarget(!isAlly);
            }
        }
    }

    public static void OnTargetClick(
        BattleCharacter target,
        BattleCharacter activeCharacter,
        Ability SelectedAbility,
        ref BattleManagerState currentBattleState,
        ref bool isHandlingTargetSelection,
        List<BattlePosition> allPositions,
        BattleUIParent battleUIParent,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions
    )
    {
        List<BattleCharacter> targets = new List<BattleCharacter>();

        if (SelectedAbility.AbilityData.areaOfEffect == AreaOfEffect.Single)
        {
            targets.Add(target);
        }
        else
        {
            targets = GetMultipleAbilityTargets(activeCharacter, SelectedAbility, target, playerPositions, enemyPositions);
        }

        if (targets.Contains(target))
        {
            activeCharacter.BattlePosition.HideActiveCharacterIndicator();
            activeCharacter.UseAbility(targets, SelectedAbility);
            currentBattleState = BattleManagerState.ChoosingAction;
            isHandlingTargetSelection = false;
            HideTargetIndicators(allPositions);
            battleUIParent.ClearCharacterData();

            TurnOrderManager.Instance.CharacterTurnComplete(activeCharacter);
        }
    }

    // public static void OnTargetClick(
    //     BattleCharacter target,
    //     BattleCharacter activeCharacter,
    //     Ability SelectedAbility,
    //     ref BattleManagerState currentBattleState,
    //     ref bool isHandlingTargetSelection,
    //     List<BattlePosition> allPositions,
    //     BattleUIParent battleUIParent,
    //     List<BattlePosition> playerPositions,
    //     List<BattlePosition> enemyPositions
    // )
    // {
    //     List<BattleCharacter> targets = new List<BattleCharacter>();
    //     // if (SelectedAbility.AbilityData.targetingType == TargetingType.Single)
    //     // {
    //     //     targets.Add(target);
    //     //     if (targets.Contains(target))
    //     //     {
    //     //         activeCharacter.BattlePosition.HideActiveCharacterIndicator();
    //     //         // activeCharacter.UseSingleTargetAbility(target, SelectedAbility);
    //     //         activeCharacter.UseAbility(targets, SelectedAbility);
    //     //         currentBattleState = BattleManagerState.ChoosingAction;
    //     //         isHandlingTargetSelection = false;
    //     //         HideTargetIndicators(allPositions);
    //     //         battleUIParent.ClearCharacterData();

    //     //         TurnOrderManager.Instance.CharacterTurnComplete(activeCharacter);
    //     //     }
    //     // }
    //     // else
    //     // {
    //     targets = GetMultipleAbilityTargets(activeCharacter, SelectedAbility, playerPositions, enemyPositions);
    //     Debug.Log("clicked on this shit: " + target.CharData.CharacterName);
    //     Debug.Log("using this ability: " + SelectedAbility.AbilityData.abilityName);
    //     foreach (var effect in SelectedAbility.TargetEffects)
    //     {
    //         Debug.Log("effect: " + effect.data.effectName);
    //     }

    //     foreach (var effect in SelectedAbility.CasterEffects)
    //     {
    //         Debug.Log("effect: " + effect.data.effectName);
    //     }
    //     if (targets.Contains(target))
    //     {
    //         activeCharacter.BattlePosition.HideActiveCharacterIndicator();
    //         activeCharacter.UseAbility(targets, SelectedAbility);
    //         currentBattleState = BattleManagerState.ChoosingAction;
    //         isHandlingTargetSelection = false;
    //         HideTargetIndicators(allPositions);
    //         battleUIParent.ClearCharacterData();

    //         TurnOrderManager.Instance.CharacterTurnComplete(activeCharacter);
    //     }
    //     // }
    // }

    private static List<BattleCharacter> GetMultipleAbilityTargets(
        BattleCharacter activeCharacter,
        Ability ability,
        BattleCharacter target,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions
    )
    {
        IEnumerable<BattlePosition> positionsToProcess;
        List<BattleCharacter> targetsToReturn = new List<BattleCharacter>();
        if (ability.AbilityData.targetFaction == TargetFaction.Allies)
        {
            positionsToProcess = activeCharacter.PlayerCharacter ? playerPositions : enemyPositions;
        }
        else
        {
            positionsToProcess = activeCharacter.PlayerCharacter ? enemyPositions : playerPositions;
        }

        foreach (var pos in positionsToProcess)
        {
            if (ability.AbilityData.areaOfEffect == AreaOfEffect.SameRow)
            {
                if (
                    (target.BattlePosition.PositionNumber <= 2 && pos.PositionNumber <= 2)
                    || (target.BattlePosition.PositionNumber > 2 && pos.PositionNumber > 2)
                )
                {
                    targetsToReturn.Add(pos.BattleCharacter);
                }
            }
            else if (ability.AbilityData.areaOfEffect == AreaOfEffect.Line)
            {
                if (Math.Abs(target.BattlePosition.PositionNumber - pos.PositionNumber) % 3 == 0)
                {
                    targetsToReturn.Add(pos.BattleCharacter);
                }
            }
        }

        return targetsToReturn;
    }

    public static BattleCharacter FindTargetCharacter(Vector2 mousePosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            GameObject targetObject = hit.collider.gameObject;
            BattleCharacter targetCharacter = targetObject.GetComponent<BattleCharacter>();
            return targetCharacter;
        }
        return null;
    }

    private static bool IsTargetValid(BattleCharacter target, List<BattlePosition> positions, Ability selectedAbility)
    {
        return true;
        // return selectedAbility.AbilityData.landingPositions.Contains(target.BattlePosition.PositionNumber);
    }

    public static void SetAbilityTargets(
        Ability ability,
        BattleCharacter caster,
        ref List<BattlePosition> currentValidTargets,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions
    )
    {
        int activeCharIndex = caster.BattlePosition.PositionNumber;
        int maxForwardPosition = activeCharIndex + -caster.CharData.ForwardMovement;
        int maxBackwardPosition = activeCharIndex + caster.CharData.BackwardMovement;
        IEnumerable<BattlePosition> battlePositionsToProcess;

        bool showHostileColor = false;

        Debug.Log("character is at position: " + activeCharIndex);
        Debug.Log("maxForwardPosition: " + maxForwardPosition);
        Debug.Log("maxBackwardPosition: " + maxBackwardPosition);

        // check if the ability has a move self effect
        // foreach (var effect in ability.AbilityData.effects)
        // {
        //     Debug.Log("effect Name: " + effect.effectName);
        //     Debug.Log("effect type: " + effect.effectType);
        // }
        if (ability.AbilityData.abilityName == "Change Positions")
        {
            Debug.Log("has move self effect");
            currentValidTargets = new List<BattlePosition>();

            // Assume characterCurrentPosition is the current position of the character
            int characterCurrentPosition = caster.BattlePosition.PositionNumber;

            // Assume forwardMovement and backwardMovement are the character's movement capabilities
            int forwardMovement = caster.CharData.ForwardMovement;
            int backwardMovement = caster.CharData.BackwardMovement;

            // Calculate the range of valid positions
            int forwardPosition = Math.Max(0, characterCurrentPosition - forwardMovement);
            int backwardPosition = Math.Min(3, characterCurrentPosition + backwardMovement);

            Debug.Log("forwardPosition: " + forwardPosition);
            Debug.Log("backwardPosition: " + backwardPosition);
            Debug.Log("forwardMovement: " + forwardMovement);
            Debug.Log("backwardMovement: " + backwardMovement);

            // Add all positions in the range to currentValidTargets
            for (int i = forwardPosition; i <= backwardPosition; i++)
            {
                if (i != characterCurrentPosition)
                {
                    Debug.Log("adding position: " + i + " to currentValidTargets");
                    BattlePosition validPosition = GetPlayerPositionByNumber(caster.PlayerCharacter ? playerPositions : enemyPositions, i);
                    validPosition.EnableTargetableIndicator(showHostileColor); // Add this line
                    currentValidTargets.Add(validPosition);
                }
            }
            OnActiveTargetsFound?.Invoke(currentValidTargets);
        }
        else
        {
            Debug.LogError("Else");
            if (ability.AbilityData.targetFaction == TargetFaction.Allies)
            {
                Debug.Log("Ally targets");
                battlePositionsToProcess = caster.PlayerCharacter ? playerPositions : enemyPositions;
            }
            else
            {
                Debug.Log("Enemy Targets");
                showHostileColor = true;
                battlePositionsToProcess = caster.PlayerCharacter ? enemyPositions : playerPositions;
            }

            currentValidTargets = new List<BattlePosition>();

            Debug.Log(battlePositionsToProcess.Count());
            Debug.Log(battlePositionsToProcess);

            // ability.AbilityData.abilityRange
            bool isInFrontRow = caster.BattlePosition.PositionNumber < 3;

            if (isInFrontRow && ability.AbilityData.abilityRange == AbilityRange.Short)
            {
                battlePositionsToProcess = battlePositionsToProcess.Where(pos => pos.PositionNumber < 3);
            }
            else if (!isInFrontRow && ability.AbilityData.abilityRange == AbilityRange.Medium)
            {
                battlePositionsToProcess = battlePositionsToProcess.Where(pos => pos.PositionNumber < 3);
            }

            foreach (var pos in battlePositionsToProcess)
            {
                if (ability.AbilityData.onlyTargetUnconscious)
                {
                    if (pos.BattleCharacter.CurrentState == CharacterState.Unconscious)
                    {
                        pos.EnableTargetableIndicator(showHostileColor);
                        currentValidTargets.Add(pos);
                    }
                    continue;
                }
                if (pos.BattleCharacter.CurrentState != CharacterState.Unconscious)
                {
                    pos.EnableTargetableIndicator(showHostileColor);
                    currentValidTargets.Add(pos);
                }
                OnActiveTargetsFound?.Invoke(currentValidTargets);
            }

            // foreach (var pos in ability.AbilityData.landingPositions)
            // {
            //     // Debug.Log(pos);

            //     if (ability.AbilityData.onlyTargetUnconscious)
            //     {
            //         if (battlePositionsToProcess.ElementAt(pos).BattleCharacter.CurrentState == CharacterState.Unconscious)
            //         {
            //             battlePositionsToProcess.ElementAt(pos).EnableTargetableIndicator(showHostileColor);
            //             currentValidTargets.Add(battlePositionsToProcess.ElementAt(pos));
            //         }
            //         continue;
            //     }
            //     if (battlePositionsToProcess.ElementAt(pos).BattleCharacter.CurrentState != CharacterState.Unconscious)
            //     {
            //         battlePositionsToProcess.ElementAt(pos).EnableTargetableIndicator(showHostileColor);
            //         currentValidTargets.Add(battlePositionsToProcess.ElementAt(pos));
            //     }
            //     OnActiveTargetsFound?.Invoke(currentValidTargets);
            // }
        }
    }

    public static BattlePosition GetPlayerPositionByNumber(List<BattlePosition> battlePositions, int positionNumber)
    {
        foreach (BattlePosition position in battlePositions)
        {
            if (position.PositionNumber == positionNumber)
            {
                return position;
            }
        }
        return null;
    }
}
