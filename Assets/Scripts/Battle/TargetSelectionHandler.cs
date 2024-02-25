using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetSelectionHandler
{
    public static List<EligibleAbility> DetermineAvailableAbilitiesBasedOnPosition(
        BattleCharacter battleCharacter,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions
    )
    {
        List<EligibleAbility> eligibleAbilities = new List<EligibleAbility>();
        Debug.LogError("ability count: " + battleCharacter.Abilities.Count);
        Debug.Log("First ability name: " + battleCharacter.Abilities[0].AbilityData.abilityName);
        foreach (var ability in battleCharacter.Abilities)
        {
            EligibleAbility eligibleAbility = new EligibleAbility(ability)
            {
                EligibleBasedOnLaunchPosition = ability.AbilityData.launchPositions.Contains(battleCharacter.BattlePosition.PositionNumber),
                EligibleBasedOnLandingTargets = DoesLandingPositionHaveValidTarget(battleCharacter, ability, playerPositions, enemyPositions)
            };
            eligibleAbilities.Add(eligibleAbility);
        }
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
            targetList = ability.AbilityData.targetFaction == TargetFaction.Allies ? playerPositions : enemyPositions;
        }
        else
        {
            targetList = ability.AbilityData.targetFaction == TargetFaction.Allies ? enemyPositions : playerPositions;
        }

        foreach (var (landingPos, targetPos) in ability.AbilityData.landingPositions.Zip(targetList, (lp, tp) => (lp, tp)))
        {
            // Check if the target position is valid and contains a character
            var targetCharacter = targetPos.GetOccupyingBattleCharacter();
            if (targetCharacter != null)
            {
                if (ability.AbilityData.onlyTargetUnconscious)
                {
                    // If onlyTargetUnconscious is true, check if the character is unconscious
                    if (targetCharacter.CurrentState == CharacterState.Unconscious)
                    {
                        return true; // At least one valid unconscious character found
                    }
                }
                else if (targetCharacter.CurrentState != CharacterState.Unconscious)
                {
                    return true; // At least one valid conscious character found
                }
            }
        }

        return false; // No valid character found
    }

    public static void OnTargetHover(
        BattleCharacter hoverTarget,
        bool isAlly,
        ref BattleCharacter activeTarget,
        BattleCharacter activeCharacter,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions,
        // ref UI_ActionPanel actionPanel,
        BattleUIParent battleUIParent,
        Ability SelectedAbility
    )
    {
        if (activeTarget != null)
        {
            activeTarget.BattlePosition.HideTransparentTarget();
        }

        IEnumerable<BattlePosition> positionList;

        if (SelectedAbility.AbilityData.targetFaction == TargetFaction.Allies)
        {
            positionList = activeCharacter.PlayerCharacter ? playerPositions : enemyPositions;
        }
        else
        {
            positionList = activeCharacter.PlayerCharacter ? enemyPositions : playerPositions;
        }
        activeTarget = hoverTarget;

        battleUIParent.SetTargetCharacterData(activeCharacter, activeTarget);
        if (SelectedAbility.AbilityData.landingPositions.Contains(hoverTarget.BattlePosition.PositionNumber))
        {
            if (SelectedAbility.AbilityData.targetingType == TargetingType.Multiple)
            {
                foreach (var position in positionList)
                {
                    if (SelectedAbility.AbilityData.landingPositions.Contains(position.PositionNumber))
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
        if (SelectedAbility.AbilityData.targetingType == TargetingType.Single)
        {
            targets.Add(target);
            if (targets.Contains(target))
            {
                activeCharacter.UseSingleTargetAbility(target, SelectedAbility);
                currentBattleState = BattleManagerState.ChoosingAction;
                isHandlingTargetSelection = false;
                HideTargetIndicators(allPositions);
                battleUIParent.ClearCharacterData();
                TurnOrderManager.Instance.CharacterTurnComplete(activeCharacter);
            }
        }
        else
        {
            targets = GetMultipleAbilityTargets(activeCharacter, SelectedAbility, playerPositions, enemyPositions);

            if (targets.Contains(target))
            {
                activeCharacter.UseAbility(targets, SelectedAbility);
                currentBattleState = BattleManagerState.ChoosingAction;
                isHandlingTargetSelection = false;
                HideTargetIndicators(allPositions);
                battleUIParent.ClearCharacterData();
                TurnOrderManager.Instance.CharacterTurnComplete(activeCharacter);
            }
        }
    }

    private static List<BattleCharacter> GetMultipleAbilityTargets(
        BattleCharacter activeCharacter,
        Ability ability,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions
    )
    {
        IEnumerable<BattlePosition> positionsToProcess;
        List<BattlePosition> positionsToReturn = new List<BattlePosition>();
        List<BattleCharacter> targetsToReturn = new List<BattleCharacter>();
        if (ability.AbilityData.targetFaction == TargetFaction.Allies)
        {
            positionsToProcess = activeCharacter.PlayerCharacter ? playerPositions : enemyPositions;
        }
        else
        {
            positionsToProcess = activeCharacter.PlayerCharacter ? enemyPositions : playerPositions;
        }

        foreach (var landingPos in ability.AbilityData.landingPositions)
        {
            var matchingPosition = positionsToProcess.FirstOrDefault(pos => pos.PositionNumber == landingPos);

            if (matchingPosition != null)
            {
                // positionsToReturn.Add(matchingPosition);
                targetsToReturn.Add(matchingPosition.GetOccupyingBattleCharacter());
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
        return selectedAbility.AbilityData.landingPositions.Contains(target.BattlePosition.PositionNumber);
    }

    public static void SetAbilityTargets(
        Ability ability,
        BattleCharacter activeCharacter,
        ref List<BattlePosition> currentValidTargets,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions
    )
    {
        IEnumerable<BattlePosition> battlePositionsToProcess;
        bool showHostileColor = false;
        if (ability.AbilityData.targetFaction == TargetFaction.Allies)
        {
            battlePositionsToProcess = activeCharacter.PlayerCharacter ? playerPositions : enemyPositions;
        }
        else
        {
            showHostileColor = true;
            battlePositionsToProcess = activeCharacter.PlayerCharacter ? enemyPositions : playerPositions;
        }

        currentValidTargets = new List<BattlePosition>();
        foreach (var pos in ability.AbilityData.landingPositions)
        {
            // Debug.Log(pos);

            if (ability.AbilityData.onlyTargetUnconscious)
            {
                if (battlePositionsToProcess.ElementAt(pos).GetOccupyingBattleCharacter().CurrentState == CharacterState.Unconscious)
                {
                    battlePositionsToProcess.ElementAt(pos).EnableTargetableIndicator(showHostileColor);
                    currentValidTargets.Add(battlePositionsToProcess.ElementAt(pos));
                }
                continue;
            }
            if (battlePositionsToProcess.ElementAt(pos).GetOccupyingBattleCharacter().CurrentState != CharacterState.Unconscious)
            {
                battlePositionsToProcess.ElementAt(pos).EnableTargetableIndicator(showHostileColor);
                currentValidTargets.Add(battlePositionsToProcess.ElementAt(pos));
            }
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