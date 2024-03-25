using System;
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

        if (ability.AbilityData.abilityRange == AbilityRange.Short)
        {
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
        BattleUIParent battleUIParent,
        Ability SelectedAbility
    )
    {
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

        if (currentValidTargets.Contains(hoverTarget.BattlePosition))
        {
            if (SelectedAbility.AbilityData.areaOfEffect == AreaOfEffect.Single)
            {
                hoverTarget.BattlePosition.EnableTransparentTarget(!isAlly);
            }
            else
            {
                // check the ability's aoe, and only enable the target on targets that will be affected if the ability gets used
                if (SelectedAbility.AbilityData.areaOfEffect == AreaOfEffect.SameRow)
                {
                    foreach (var position in positionList)
                    {
                        // Determine the row of the hover target
                        int hoverTargetRow = hoverTarget.BattlePosition.PositionNumber <= 2 ? 0 : 1;

                        // Determine the row of the current position
                        int positionRow = position.PositionNumber <= 2 ? 0 : 1;

                        // If they're in the same row, enable the transparent target
                        if (hoverTargetRow == positionRow)
                        {
                            position.EnableTransparentTarget(!isAlly);
                        }
                        else
                        {
                            position.HideTransparentTarget();
                        }
                    }
                }
                else if (SelectedAbility.AbilityData.areaOfEffect == AreaOfEffect.Line)
                {
                    foreach (var position in positionList)
                    {
                        if (Math.Abs(hoverTarget.BattlePosition.PositionNumber - position.PositionNumber) % 3 == 0)
                        {
                            position.EnableTransparentTarget(!isAlly);
                        }
                    }
                }
                else if (SelectedAbility.AbilityData.areaOfEffect == AreaOfEffect.Cross)
                {
                    foreach (var position in positionList)
                    {
                        int hoverPos = hoverTarget.BattlePosition.PositionNumber;
                        int currentPos = position.PositionNumber;

                        bool isSamePosition = hoverPos == currentPos;

                        bool isVerticalNeighbor = Math.Abs(hoverPos - currentPos) == 3;

                        bool isHorizontalNeighbor = (hoverPos / 3 == currentPos / 3) && (Math.Abs(hoverPos - currentPos) == 1);

                        if (isSamePosition || isVerticalNeighbor || isHorizontalNeighbor)
                        {
                            position.EnableTransparentTarget(!isAlly);
                        }
                    }
                }
                else
                {
                    foreach (var position in positionList)
                    {
                        position.EnableTransparentTarget(!isAlly);
                    }
                }
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

            if (ability.AbilityData.areaOfEffect == AreaOfEffect.SameRow)
            {
                int clickTargetRow = target.BattlePosition.PositionNumber <= 2 ? 0 : 1;
                int currentPositionRow = pos.PositionNumber <= 2 ? 0 : 1;
                if (clickTargetRow == currentPositionRow)
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
            else if (ability.AbilityData.areaOfEffect == AreaOfEffect.Cross)
            {
                int targetPos = target.BattlePosition.PositionNumber;
                int currentPos = pos.PositionNumber;
                bool isSamePosition = targetPos == currentPos;
                bool isVerticalNeighbor = Math.Abs(targetPos - currentPos) == 3;
                bool isHorizontalNeighbor = (targetPos / 3 == currentPos / 3) && (Math.Abs(targetPos - currentPos) == 1);

                if (isSamePosition || isVerticalNeighbor || isHorizontalNeighbor)
                {
                    targetsToReturn.Add(pos.BattleCharacter);
                }
            }
            else
            {
                targetsToReturn.Add(pos.BattleCharacter);
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
        if (ability.AbilityData.abilityName == "Change Positions")
        {
            currentValidTargets = DoChangePositionStuff(caster, playerPositions, enemyPositions, showHostileColor);
        }
        else
        {
            battlePositionsToProcess = GetBattlePositionsToProcess(ability, caster, playerPositions, enemyPositions, ref showHostileColor);
            currentValidTargets = new List<BattlePosition>();

            bool isInFrontRow = caster.BattlePosition.PositionNumber < 3;

            /**
                If character is in front row and the ability is short, only allow front row to be targeted
            */
            if (isInFrontRow && ability.AbilityData.abilityRange == AbilityRange.Short)
            {
                battlePositionsToProcess = battlePositionsToProcess.Where(pos => pos.PositionNumber < 3);
            }
            /**
                If the character is in the back row and it is a medium range ability, only allow front row to be targeted
            */
            else if (!isInFrontRow && ability.AbilityData.abilityRange == AbilityRange.Medium)
            {
                battlePositionsToProcess = battlePositionsToProcess.Where(pos => pos.PositionNumber < 3);
            }
            /**
                If the ability is long range, allow all positions to be targeted.
                battlePositionsToProcess is already using all positions, so nothing needs done
            */

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
        }
    }

    private static List<BattlePosition> DoChangePositionStuff(
        BattleCharacter caster,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions,
        bool showHostileColor
    )
    {
        List<BattlePosition> currentValidTargets = new List<BattlePosition>();

        // Assume characterCurrentPosition is the current position of the character
        int characterCurrentPosition = caster.BattlePosition.PositionNumber;

        // Assume forwardMovement and backwardMovement are the character's movement capabilities
        int forwardMovement = caster.CharData.ForwardMovement;
        int backwardMovement = caster.CharData.BackwardMovement;

        // Calculate the range of valid positions
        int forwardPosition = Math.Max(0, characterCurrentPosition - forwardMovement);
        int backwardPosition = Math.Min(3, characterCurrentPosition + backwardMovement);

        // Add all positions in the range to currentValidTargets
        for (int i = forwardPosition; i <= backwardPosition; i++)
        {
            if (i != characterCurrentPosition)
            {
                BattlePosition validPosition = GetPlayerPositionByNumber(caster.PlayerCharacter ? playerPositions : enemyPositions, i);
                validPosition.EnableTargetableIndicator(showHostileColor); // Add this line
                currentValidTargets.Add(validPosition);
            }
        }
        OnActiveTargetsFound?.Invoke(currentValidTargets);
        return currentValidTargets;
    }

    private static IEnumerable<BattlePosition> GetBattlePositionsToProcess(
        Ability ability,
        BattleCharacter caster,
        List<BattlePosition> playerPositions,
        List<BattlePosition> enemyPositions,
        ref bool showHostileColor
    )
    {
        IEnumerable<BattlePosition> battlePositionsToProcess;
        if (ability.AbilityData.targetFaction == TargetFaction.Allies)
        {
            battlePositionsToProcess = caster.PlayerCharacter ? playerPositions : enemyPositions;
        }
        else
        {
            showHostileColor = true;
            battlePositionsToProcess = caster.PlayerCharacter ? enemyPositions : playerPositions;
        }

        return battlePositionsToProcess;
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
