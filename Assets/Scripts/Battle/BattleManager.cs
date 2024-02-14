using System;
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
    private UI_ActionPanel actionPanel;

    [SerializeField]
    private BattleUIParent battleUIParent;

    [Space(15)]
    [Header("Battle Positions")]
    [SerializeField]
    private Transform[] playerPositions;

    [SerializeField]
    private Transform[] enemyPositions;

    [Space(15)]
    [Header("Character Data")]
    [SerializeField]
    private List<CharacterData> playerBaseCharacters;

    [SerializeField]
    private List<CharacterDataSO> enemyBaseCharacters;

    [Space(15)]
    [Header("BattleCharacters")]
    public List<BattleCharacter> playerBattleCharacters = new List<BattleCharacter>();

    public List<BattleCharacter> enemyBattleCharacters = new List<BattleCharacter>();

    [Space(15)]
    [Header("Active Character")]
    [SerializeField]
    public BattleCharacter activeCharacter;

    [Space(15)]
    [Header("State Machine for BattleManager")]
    [SerializeField]
    public BattleManagerState currentBattleState = BattleManagerState.ChoosingAction;

    [Space(15)]
    [Header("Prefabs")]
    [ReadOnly]
    public GameObject battleCharacterPrefab;

    private BattleCharacter activeTarget;

    private enum BattleCharacterSubset
    {
        All,
        Players,
        Enemies
    }

    void Awake()
    {
        Debug.Log("BattleManager Awake");
        actionPanel.OnActionButtonClicked += HandleActionButtonClick;
    }

    private void CallMethodOnAllBattleCharacters(Action<BattleCharacter> action, BattleCharacterSubset subset)
    {
        // not super clear on the whole purpose of IEnumerable, but seems like it
        // lets you initialize a collection of objects and delay assigning them?
        // if I tried List<BattleCharacter> instead, the syntax would be very different
        IEnumerable<BattleCharacter> charactersToProcess;

        switch (subset)
        {
            case BattleCharacterSubset.Players:
                charactersToProcess = playerBattleCharacters;
                break;
            case BattleCharacterSubset.Enemies:
                charactersToProcess = enemyBattleCharacters;
                break;
            default:
                charactersToProcess = playerBattleCharacters.Concat(enemyBattleCharacters);
                break;
        }

        foreach (var character in charactersToProcess)
        {
            action(character);
        }
    }

    private List<BattleCharacter> GetAllBattleCharacters()
    {
        List<BattleCharacter> allChars = new List<BattleCharacter>();
        allChars.AddRange(playerBattleCharacters);
        allChars.AddRange(enemyBattleCharacters);
        return allChars;
    }

    // will later pass the actual ability being used so it will know the possible launch and landing positions.
    private void SetPossibleTargets(BattleAction action)
    {
        CallMethodOnAllBattleCharacters(character => character.HideTargetableIndicator(), BattleCharacterSubset.All);

        switch (action)
        {
            case BattleAction.Attack:

                {
                    CallMethodOnAllBattleCharacters(
                        character => character.ShowTargetableIndicator(true),
                        activeCharacter.IsPlayer() ? BattleCharacterSubset.Enemies : BattleCharacterSubset.Players
                    );
                }
                break;
            case BattleAction.Heal:
            {
                {
                    CallMethodOnAllBattleCharacters(
                        character => character.ShowTargetableIndicator(false),
                        activeCharacter.IsPlayer() ? BattleCharacterSubset.Players : BattleCharacterSubset.Enemies
                    );
                }

                // {
                //     if (activeCharacter.IsPlayer())
                //     {
                //         foreach (BattleCharacter character in playerBattleCharacters)
                //         {
                //             character.ShowTargetableIndicator(false);
                //         }
                //     }
                //     else
                //     {
                //         if (!activeCharacter.IsPlayer())
                //         {
                //             foreach (BattleCharacter character in enemyBattleCharacters)
                //             {
                //                 character.ShowTargetableIndicator(false);
                //             }
                //         }
                //     }
                // }
                break;
            }
        }
    }

    /// <summary>
    /// When an "Action" is chosen in the actionPanel, this method will clear the "targetedBG"
    /// graphics from every character, then re-add the targetedBG for the characters that are
    /// potential targets for the chosen action. Ex: Allies can only heal allies, so only they
    /// are potential heal targets
    /// </summary>
    private void HandleActionButtonClick(BattleAction action)
    {
        actionPanel.SetActionData(action, activeCharacter);
        CallMethodOnAllBattleCharacters(character => character.HideTargetedBG(), BattleCharacterSubset.All);

        if (action == BattleAction.Attack)
        {
            SetPossibleTargets(action);
            currentBattleState = BattleManagerState.ChoosingEnemyTarget;
        }
        else if (action == BattleAction.Heal)
        {
            SetPossibleTargets(action);
            currentBattleState = BattleManagerState.ChoosingAllyTarget;
        }
    }

    private void HandleTurnChange(BattleCharacter newActiveCharacter)
    {
        if (activeCharacter != null)
        {
            activeCharacter.HideActiveCharacterIndicator();
        }
        activeCharacter = newActiveCharacter;

        activeCharacter.ShowActiveCharacterIndicator();
        List<BattleAction> availableActions = GetAvailableActionsForCharacter(activeCharacter);
        battleUIParent.ShowActionPanel();
        actionPanel.SetAvailableActions(availableActions);
        actionPanel.SetActiveCharacterData(activeCharacter);
    }

    private List<BattleAction> GetAvailableActionsForCharacter(BattleCharacter character)
    {
        return character.GetCharacterData().battleActions;
    }

    public void PopulatePlayersFromParty(List<CharacterData> players)
    {
        // enemies dont have baseCharacters because they are just instantiated using their
        // CharacterDataSO data. Player Characters use a baseCharacter (CharacterData type) because their
        // CharacterDataSO (scriptable Object) is not updated during runtime, only the CharacterData is.
        playerBaseCharacters = players;
        StartBattle();
    }

    void StartBattle()
    {
        List<CharacterData> allChars = new List<CharacterData>();
        allChars.AddRange(playerBaseCharacters);
        allChars.AddRange(SetEnemies());
        SetBattleCharacters(allChars);
    }

    List<CharacterData> SetEnemies()
    {
        List<CharacterData> enemies = new List<CharacterData>();
        foreach (CharacterDataSO character in enemyBaseCharacters)
        {
            CharacterData charData = new CharacterData(character);
            enemies.Add(charData);
        }
        return enemies;
    }

    private void SetBattleCharacters(List<CharacterData> characters)
    {
        int currentEnemyIndex = 0; // enemies are index 4-7
        playerBattleCharacters.Clear();
        enemyBattleCharacters.Clear();

        foreach (CharacterData character in characters)
        {
            GameObject battleCharObject = Instantiate(battleCharacterPrefab, Vector3.zero, Quaternion.identity);
            BattleCharacter battleCharacter = battleCharObject.GetComponent<BattleCharacter>();
            battleCharacter.InitializeCharacter(character);

            battleCharObject.name = character.characterName;

            if (character.playerCharacter)
            {
                if (battleCharacter.CurrentBattlePosition != -1)
                {
                    // Character has a specified position, use it
                    battleCharacter.SetPositionIndex(battleCharacter.CurrentBattlePosition);
                    playerBattleCharacters.Add(battleCharacter);
                }
                else
                {
                    // Character doesn't have a specified position, set later
                    // ** this probably won't actually occur after the game is fully setup,
                    // all characters should receive a battle position as they get added
                    // to the party **
                    playerBattleCharacters.Add(battleCharacter);
                }
            }
            else
            {
                battleCharacter.SetPositionIndex(currentEnemyIndex);
                battleCharacter.transform.position = enemyPositions[currentEnemyIndex].position;
                battleCharacter.transform.SetParent(enemyPositions[currentEnemyIndex].transform);
                currentEnemyIndex++;
                enemyBattleCharacters.Add(battleCharacter);
            }
        }

        int availablePosition = 0;
        foreach (BattleCharacter playerCharacter in playerBattleCharacters)
        {
            if (playerCharacter.CurrentBattlePosition == -1)
            {
                playerCharacter.SetPositionIndex(availablePosition);
                playerCharacter.transform.position = playerPositions[availablePosition].position;
                playerCharacter.transform.SetParent(playerPositions[availablePosition].transform);
                availablePosition++;
            }
        }

        List<BattleCharacter> allChars = new List<BattleCharacter>();
        allChars.AddRange(playerBattleCharacters);
        allChars.AddRange(enemyBattleCharacters);
        foreach (BattleCharacter character in allChars)
        {
            character.HideActiveCharacterIndicator();
        }
        TurnOrderManager.Instance.OnActiveCharacterChanged += HandleTurnChange;
        TurnOrderManager.Instance.SetMasterCharacterList(allChars);
    }

    void HandleAllyTargetSelection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] racastHits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
        bool validTargetFound = false;
        BattleCharacter targetCharacter;
        foreach (var hit in racastHits)
        {
            GameObject targetObject = hit.collider.gameObject;
            targetCharacter = targetObject.GetComponent<BattleCharacter>();
            if (targetCharacter != null && CharactersAreSameFaction(activeCharacter, targetCharacter))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnAllyClick(targetCharacter);
                }
                else
                {
                    OnAllyHover(targetCharacter);
                }

                validTargetFound = true;
                break;
            }
        }

        if (!validTargetFound)
        {
            DisableAllCharacterSelectors();
        }
    }

    void HandleEnemyTargetSelection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
        bool validTargetFound = false;
        BattleCharacter targetCharacter = null;
        foreach (var hit in raycastHits)
        {
            GameObject targetObject = hit.collider.gameObject;
            targetCharacter = targetObject.GetComponent<BattleCharacter>();

            if (targetCharacter != null && !CharactersAreSameFaction(activeCharacter, targetCharacter))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnEnemyClick(targetCharacter);
                }
                else
                {
                    OnEnemyHover(targetCharacter);
                }
                validTargetFound = true;
                break;
            }
        }

        if (!validTargetFound)
        {
            DisableAllCharacterSelectors();
        }
    }

    void DisableAllCharacterSelectors()
    {
        CallMethodOnAllBattleCharacters(character => character.HideTargetedBG(), BattleCharacterSubset.All);
        actionPanel.ClearEnemyData();
    }

    void OnAllyClick(BattleCharacter target)
    {
        activeCharacter.Heal(target);
        activeCharacter.EndTurn();
        HideAllTargetIndicators();
        TurnOrderManager.Instance.CharacterTurnComplete(activeCharacter);
    }

    void OnEnemyClick(BattleCharacter target)
    {
        activeCharacter.Attack(target);
        currentBattleState = BattleManagerState.ChoosingAction;
        activeCharacter.EndTurn();
        HideAllTargetIndicators();
        actionPanel.ClearCharacterData();
        TurnOrderManager.Instance.CharacterTurnComplete(activeCharacter);
    }

    private void HideAllTargetIndicators()
    {
        CallMethodOnAllBattleCharacters(character => character.HideTargetableIndicator(), BattleCharacterSubset.All);
        CallMethodOnAllBattleCharacters(character => character.HideTargetedBG(), BattleCharacterSubset.All);
    }

    private bool CharactersAreSameFaction(BattleCharacter charA, BattleCharacter charB)
    {
        return charA.IsPlayer() && charB.IsPlayer() || !charA.IsPlayer() && !charB.IsPlayer();
    }

    void OnAllyHover(BattleCharacter hoverTarget)
    {
        if (activeTarget != null)
        {
            activeTarget.HideTargetedBG();
        }

        if (CharactersAreSameFaction(activeCharacter, hoverTarget))
        {
            activeTarget = hoverTarget;
            hoverTarget.ShowTargetedBG(false);
            actionPanel.SetTargetCharacterData(activeCharacter, hoverTarget);
        }
    }

    void OnEnemyHover(BattleCharacter hoverTarget)
    {
        if (activeTarget != null)
        {
            activeTarget.HideTargetedBG();
        }

        if (!CharactersAreSameFaction(activeCharacter, hoverTarget))
        {
            activeTarget = hoverTarget;
            activeTarget.ShowTargetedBG(true);
            actionPanel.SetTargetCharacterData(activeCharacter, hoverTarget);
        }
    }

    void Update()
    {
        if (currentBattleState == BattleManagerState.ChoosingEnemyTarget)
        {
            HandleEnemyTargetSelection();
        }
        else if (currentBattleState == BattleManagerState.ChoosingAllyTarget)
        {
            HandleAllyTargetSelection();
        }
    }
}
