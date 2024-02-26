using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public enum BattleManagerState
{
    ChoosingAction,
    ChoosingEnemyTarget,
    ChoosingAllyTarget,
    ChoosingAbility,
    ChoosingItem,
    ChoosingNewPosition,
}

public class BattleManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField]
    private BattleUIParent battleUIParent;

    [Space(15)]
    [Header("Battle Positions")]
    [SerializeField]
    private List<BattlePosition> playerPositions = new List<BattlePosition>();
    public List<BattlePosition> PlayerPositions
    {
        get { return playerPositions; }
        set { playerPositions = value; }
    }

    [SerializeField]
    private List<BattlePosition> enemyPositions = new List<BattlePosition>();
    public List<BattlePosition> EnemyPositions
    {
        get { return enemyPositions; }
        set { enemyPositions = value; }
    }

    private List<BattlePosition> allPositions = new List<BattlePosition>();
    public List<BattlePosition> AllPositions
    {
        get { return allPositions; }
        set { allPositions = value; }
    }

    [Space(15)]
    [Header("Character Data")]
    [SerializeField]
    private List<CharacterData> playerBaseCharacters;
    public List<CharacterData> PlayerBaseCharacters
    {
        get { return playerBaseCharacters; }
        set { playerBaseCharacters = value; }
    }

    [SerializeField]
    private List<CharacterData> enemyBaseCharacters;
    public List<CharacterData> EnemyBaseCharacters
    {
        get { return enemyBaseCharacters; }
        set { enemyBaseCharacters = value; }
    }

    [SerializeField]
    private EnemyParty enemyParty;
    public EnemyParty EnemyParty
    {
        get { return enemyParty; }
        set { enemyParty = value; }
    }

    [Space(15)]
    [Header("BattleCharacters")]
    private List<BattleCharacter> playerBattleCharacters = new List<BattleCharacter>();
    public List<BattleCharacter> PlayerBattleCharacters
    {
        get { return playerBattleCharacters; }
        set { playerBattleCharacters = value; }
    }

    [SerializeField]
    private List<BattleCharacter> enemyBattleCharacters = new List<BattleCharacter>();
    public List<BattleCharacter> EnemyBattleCharacters
    {
        get { return enemyBattleCharacters; }
        set { enemyBattleCharacters = value; }
    }

    [Space(15)]
    [Header("Active Character")]
    [SerializeField]
    private BattleCharacter activeCharacter;
    public BattleCharacter ActiveCharacter
    {
        get { return activeCharacter; }
        set { activeCharacter = value; }
    }

    [Space(15)]
    [Header("State Machine for BattleManager")]
    [SerializeField]
    private BattleManagerState currentBattleState = BattleManagerState.ChoosingAction;
    public BattleManagerState CurrentBattleState
    {
        get { return currentBattleState; }
        set { HandleStateChange(value); }
    }

    private void HandleStateChange(BattleManagerState value)
    {
        Debug.Log("HandleStateChange");
        if (currentBattleState == BattleManagerState.ChoosingEnemyTarget || currentBattleState == BattleManagerState.ChoosingAllyTarget)
        {
            StopCoroutine(HandleTargetSelection(currentBattleState));
            isHandlingTargetSelection = false;
        }

        currentBattleState = value;

        if (currentBattleState == BattleManagerState.ChoosingEnemyTarget || currentBattleState == BattleManagerState.ChoosingAllyTarget)
        {
            StartCoroutine(HandleTargetSelection(currentBattleState));
            isHandlingTargetSelection = true;
        }
    }

    [Space(15)]
    [Header("Prefabs")]
    [ReadOnly]
    public GameObject battleCharacterPrefab;

    [SerializeField]
    private Ability selectedAbility;
    private Ability SelectedAbility
    {
        get { return selectedAbility; }
        set { selectedAbility = value; }
    }

    [SerializeField]
    private BattleCharacter activeTarget;
    public BattleCharacter ActiveTarget
    {
        get { return activeTarget; }
        set { activeTarget = value; }
    }

    [SerializeField]
    private bool isHandlingTargetSelection = false;
    public bool IsHandlingTargetSelection
    {
        get { return isHandlingTargetSelection; }
        set { isHandlingTargetSelection = value; }
    }

    [SerializeField]
    private List<BattlePosition> currentValidTargets = new List<BattlePosition>();
    public List<BattlePosition> CurrentValidTargets
    {
        get { return currentValidTargets; }
        set { currentValidTargets = value; }
    }

    public BattleCharacter movingCharacter1;
    public BattleCharacter movingCharacter2;

    void Awake()
    {
        battleUIParent.OnAbilityButtonClicked += HandleAbilityButtonClick;
        EffectHandler.OnMoveCharacterRequest += MoveCharacter;
        AllPositions = PlayerPositions.Concat(EnemyPositions).ToList();
    }

    void OnDestroy()
    {
        battleUIParent.OnAbilityButtonClicked -= HandleAbilityButtonClick;
        EffectHandler.OnMoveCharacterRequest -= MoveCharacter;
        TurnOrderManager.Instance.OnActiveCharacterChanged -= HandleTurnChange;
    }

    // if this is an item, then probably need to handle it different. maybe also pass the item in as a parameter?
    // or just have a separate method for handling items.
    private void HandleAbilityButtonClick(Ability ability)
    {
        SelectedAbility = ability;
        Debug.Log(selectedAbility.AbilityData.abilityName);
        if (ability.AbilityData.abilityTypes.Contains(AbilityType.SkipTurn))
        {
            battleUIParent.ClearCharacterData();
            TurnOrderManager.Instance.CharacterTurnComplete(ActiveCharacter);
            return;
        }

        battleUIParent.SetAbilityData(ability, activeCharacter);
        TargetSelectionHandler.HideAllTargetIndicators(AllPositions);
        TargetSelectionHandler.SetAbilityTargets(ability, ActiveCharacter, ref currentValidTargets, PlayerPositions, EnemyPositions);
        // if the ability is an item, then the item's target faction will determine this. not implemented yet though.
        CurrentBattleState =
            (ability.AbilityData.targetFaction == TargetFaction.Enemies)
                ? BattleManagerState.ChoosingEnemyTarget
                : BattleManagerState.ChoosingAllyTarget;
    }

    private void HandleTurnChange(BattleCharacter newActiveCharacter)
    {
        Debug.Log("HandleTurnChange");
        Debug.Log(newActiveCharacter.CharData.CharacterName);
        Debug.Log(newActiveCharacter.CharData.Abilities.Count);
        Debug.Log("Current Active Character: " + ActiveCharacter?.CharData.CharacterName);

        // Debug.LogError("HandleTurnChange: " + newActiveCharacter.CharData.CharacterName);
        // Debug.LogError("character abilities: " + newActiveCharacter.CharData.Abilities.Count);
        // Debug.LogError("First ability: " + newActiveCharacter.CharData.Abilities[0].AbilityData.abilityName);
        ActiveCharacter?.BattlePosition.HideActiveCharacterIndicator();
        ActiveCharacter = newActiveCharacter;
        ActiveCharacter?.BattlePosition.EnableActiveCharacterIndicator();

        battleUIParent.ShowActionPanelForCharacter(
            newActiveCharacter,
            TargetSelectionHandler.DetermineAvailableAbilitiesBasedOnPosition(newActiveCharacter, PlayerPositions, EnemyPositions)
        );
        TurnOrderManager.Instance.StartTurn(activeCharacter);
    }

    public void PopulatePlayersFromParty(List<CharacterData> players)
    {
        PlayerBaseCharacters = players;
        EnemyBaseCharacters = SetEnemies();
        StartBattle();
    }

    void StartBattle()
    {
        Debug.Log("StartBattle");
        List<CharacterData> allChars = new List<CharacterData>();
        allChars.AddRange(playerBaseCharacters);
        allChars.AddRange(EnemyBaseCharacters);
        TurnOrderManager.Instance.OnActiveCharacterChanged += HandleTurnChange;
        TurnOrderManager.Instance.SetMasterCharacterList(SetBattleCharacters(allChars));
        TurnOrderManager.Instance.CreateNewTurnOrder();
    }

    void EndBattle()
    {
        TurnOrderManager.Instance.OnActiveCharacterChanged -= HandleTurnChange;
        foreach (BattleCharacter character in PlayerBattleCharacters)
        {
            character.BattlePosition.SetOccupyingCharacter(null);
            Destroy(character.gameObject);
        }
        foreach (BattleCharacter character in EnemyBattleCharacters)
        {
            character.BattlePosition.SetOccupyingCharacter(null);
            Destroy(character.gameObject);
        }
        PlayerBattleCharacters.Clear();
        EnemyBattleCharacters.Clear();
    }

    List<CharacterData> SetEnemies()
    {
        List<CharacterData> enemies = new List<CharacterData>();
        int index = 0;

        foreach (CharacterDataSO character in EnemyParty.enemyPositions)
        {
            CharacterData newEnemy = new CharacterData(character) { FormationPosition = index };
            enemies.Add(newEnemy);
            index++;
        }

        return enemies;
    }

    private List<BattleCharacter> SetBattleCharacters(List<CharacterData> characters)
    {
        Debug.Log("SetBattleCharacters");
        PlayerBattleCharacters.Clear();
        EnemyBattleCharacters.Clear();

        Debug.Log(characters.Count);
        foreach (CharacterData character in characters)
        {
            Debug.Log(character.CharacterName);
            GameObject battleCharObject = Instantiate(battleCharacterPrefab, Vector3.zero, Quaternion.identity);
            BattleCharacter battleCharacter = battleCharObject.GetComponent<BattleCharacter>();
            battleCharacter.InitializeCharacter(character);
            battleCharObject.name = character.CharacterName;
            if (character.PlayerCharacter)
            {
                BattlePosition desiredPosition = TargetSelectionHandler.GetPlayerPositionByNumber(PlayerPositions, battleCharacter.FormationPosition);
                desiredPosition.SetOccupyingCharacter(battleCharacter);
                PlayerBattleCharacters.Add(battleCharacter);
            }
            else
            {
                BattlePosition desiredPosition = TargetSelectionHandler.GetPlayerPositionByNumber(EnemyPositions, battleCharacter.FormationPosition);
                desiredPosition.SetOccupyingCharacter(battleCharacter);
                EnemyBattleCharacters.Add(battleCharacter);
            }
        }

        List<BattleCharacter> allChars = new List<BattleCharacter>();
        allChars.AddRange(PlayerBattleCharacters);
        allChars.AddRange(EnemyBattleCharacters);
        foreach (BattleCharacter character in allChars)
        {
            character.BattlePosition.HideActiveCharacterIndicator();
        }
        return allChars;
    }

    #region Calls to UI Action Panel
    private void ClearUITargetData()
    {
        battleUIParent.ClearTargetData(ActiveCharacter);
    }
    #endregion


    private bool CharactersAreSameFaction(BattleCharacter charA, BattleCharacter charB)
    {
        return charA.PlayerCharacter && charB.PlayerCharacter || !charA.PlayerCharacter && !charB.PlayerCharacter;
    }

    void Update() { }

    private IEnumerator HandleTargetSelection(BattleManagerState targetSelectionState)
    {
        IsHandlingTargetSelection = true;
        while (CurrentBattleState == targetSelectionState)
        {
            yield return null;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            BattleCharacter targetCharacter = TargetSelectionHandler.FindTargetCharacter(mousePosition);
            bool validTargetFound = false;

            if (targetCharacter != null)
            {
                bool targetingSameFaction = CharactersAreSameFaction(ActiveCharacter, targetCharacter);
                if (
                    (targetSelectionState == BattleManagerState.ChoosingEnemyTarget && !targetingSameFaction)
                    || (targetSelectionState == BattleManagerState.ChoosingAllyTarget && targetingSameFaction)
                )
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (CurrentValidTargets.Contains(targetCharacter.BattlePosition))
                        {
                            TargetSelectionHandler.OnTargetClick(
                                targetCharacter,
                                ActiveCharacter,
                                SelectedAbility,
                                ref currentBattleState,
                                ref isHandlingTargetSelection,
                                AllPositions,
                                battleUIParent,
                                PlayerPositions,
                                EnemyPositions
                            );
                        }
                    }
                    else
                    {
                        TargetSelectionHandler.OnTargetHover(
                            targetCharacter,
                            targetingSameFaction,
                            ref activeTarget,
                            activeCharacter,
                            playerPositions,
                            enemyPositions,
                            battleUIParent,
                            SelectedAbility
                        );
                    }
                    validTargetFound = true;
                }
            }

            if (!validTargetFound)
            {
                ClearUITargetData();
                foreach (var pos in AllPositions)
                {
                    pos.HideTransparentTarget();
                }
            }
        }
        isHandlingTargetSelection = false;
    }

    private void MoveCharacter(BattleCharacter character, int positionsToMove)
    {
        List<BattleCharacter> battleCharacters = character.PlayerCharacter ? PlayerBattleCharacters : EnemyBattleCharacters;
        CharacterMovementHandler.Instance.MoveCharacter(character, positionsToMove, battleCharacters);
        // character.BattlePosition.SetOccupyingCharacter(null);
        // character.BattlePosition = newPosition;
        // newPosition.SetOccupyingCharacter(character);
    }

    private BattlePosition GetBattlePosition(int index, TargetFaction targetFaction)
    {
        return targetFaction == TargetFaction.Allies ? playerPositions[index] : enemyPositions[index];
    }
}
