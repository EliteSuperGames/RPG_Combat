using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnOrderManager : MonoBehaviour
{
    [SerializeField]
    private List<BattleCharacter> currentTurnOrder = new List<BattleCharacter>();
    private List<BattleCharacter> completedTurnCharacters = new List<BattleCharacter>();

    [SerializeField]
    private List<BattleCharacter> masterCharacterList = new List<BattleCharacter>();

    public static TurnOrderManager Instance { get; private set; }

    public event Action<string> OnRoundChanged;
    public event Action<List<BattleCharacter>> OnTurnOrderChanged;
    public event Action<BattleCharacter> OnActiveCharacterChanged;
    private int currentRound = 0;

    [SerializeField]
    private BattleCharacter activeCharacter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMasterCharacterList(List<BattleCharacter> characters)
    {
        Debug.Log("SetMasterCharacterList");
        masterCharacterList = characters;
        AddCharactersToTurnOrder(masterCharacterList);
    }

    private void AddCharactersToTurnOrder(List<BattleCharacter> characters)
    {
        Debug.Log("AddCharactersToTurnOrder");
        foreach (BattleCharacter character in characters)
        {
            currentTurnOrder.Add(character);
            character.OnCharacterDeath += HandleCharacterDeath;
            character.OnCharacterRevive += HandleCharacterRevived;
        }
    }

    public void Reset()
    {
        currentTurnOrder.Clear();
        completedTurnCharacters.Clear();
        masterCharacterList.Clear();
        currentRound = 1;
        activeCharacter = null;
    }

    private void HandleCharacterRevived(BattleCharacter character)
    {
        // Only add the character back to the turn order if they haven't completed their turn yet
        if (!currentTurnOrder.Contains(character) && !completedTurnCharacters.Contains(character))
        {
            // Insert the character at the beginning of the turn order
            currentTurnOrder.Insert(0, character);
            UpdateTurnOrderUI();
        }
    }

    private void HandleCharacterDeath(BattleCharacter character)
    {
        if (currentTurnOrder.Contains(character))
        {
            if (character == activeCharacter)
            {
                StartCoroutine(StartNextTurnAfterDelay(0.5f));
            }
            else
            {
                currentTurnOrder.Remove(character);
                UpdateTurnOrderUI();
            }
        }
    }

    public void CreateNewTurnOrder()
    {
        Debug.Log("CreateNewTurnOrder");
        // Filter the currentTurnOrder list directly
        currentTurnOrder = currentTurnOrder.Where(character => character.CurrentState != CharacterState.Unconscious).ToList();

        // Sort the currentTurnOrder list
        if (currentTurnOrder.Count > 0)
        {
            currentTurnOrder.Sort((a, b) => b.GetCharacterData().Speed.CompareTo(a.GetCharacterData().Speed));
        }
        else
        {
            currentTurnOrder = masterCharacterList.Where(character => character.CurrentState != CharacterState.Unconscious).ToList();
            currentTurnOrder.Sort((a, b) => b.GetCharacterData().Speed.CompareTo(a.GetCharacterData().Speed));
        }

        activeCharacter = currentTurnOrder[0];
        UpdateTurnOrderUI();
    }

    private void UpdateTurnOrderUI()
    {
        Debug.Log("UpdateTurnOrderUI");
        OnRoundChanged?.Invoke(currentRound.ToString());
        OnTurnOrderChanged?.Invoke(currentTurnOrder);
        // Debug.LogError("OnActiveCharacterChanged being invoked");
        // Debug.Log("Active Character: " + activeCharacter.GetCharacterData().CharacterName);
        // Debug.Log("first ability: " + activeCharacter.GetCharacterData().Abilities[0].AbilityData.abilityName);
        OnActiveCharacterChanged?.Invoke(activeCharacter);
    }

    public BattleCharacter GetActiveCharacter()
    {
        Debug.Log("GetActiveCharacter");
        return activeCharacter;
    }

    public void StartTurn(BattleCharacter character)
    {
        Debug.LogError("StartNextTurn");
        character.StartTurn();
        // UpdateTurnOrderUI();



        // if (currentTurnOrder.Count == 0)
        // {
        //     // if it is, create a new turn order and start a new round
        //     CreateNewTurnOrder();
        //     StartNextRound();
        // }
        // activeCharacter = DetermineNextActiveCharacter();
        // UpdateTurnOrderUI();
    }

    private BattleCharacter DetermineNextActiveCharacter()
    {
        Debug.Log("DetermineNextActiveCharacter");
        if (currentTurnOrder.Count > 0)
        {
            return currentTurnOrder[0];
        }
        else
        {
            CreateNewTurnOrder();
            return DetermineNextActiveCharacter();
        }
        // StartNextRound();
        //     return currentTurnOrder[0];
        // }
        // else
        // {
        //     CreateNewTurnOrder();
        //     StartNextRound();
        //     return DetermineNextActiveCharacter();
        // }
    }

    public void StartNextRound()
    {
        Debug.Log("StartNextRound");
        currentRound++;
        completedTurnCharacters.Clear();
    }

    public void CharacterTurnComplete(BattleCharacter character)
    {
        Debug.Log(character.GetCharacterData().CharacterName + "'s turn is complete.");
        character.EndTurn();
        currentTurnOrder.Remove(character);
        completedTurnCharacters.Add(character);
        activeCharacter = DetermineNextActiveCharacter();
        UpdateTurnOrderUI();
    }

    private IEnumerator StartNextTurnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // StartTurn();
    }

    public List<BattleCharacter> GetCurrentTurnOrder()
    {
        Debug.Log("GetCurrentTurnOrder");
        return new List<BattleCharacter>(currentTurnOrder);
    }

    public BattleCharacter GetNextCharacter()
    {
        Debug.Log("GetNextCharacter");
        Debug.Log("Returning: " + DetermineNextActiveCharacter().GetCharacterData().CharacterName);

        Debug.Log(DetermineNextActiveCharacter().GetCharacterData().Abilities[0].AbilityData.abilityName);
        activeCharacter = DetermineNextActiveCharacter();
        return activeCharacter;
        // return DetermineNextActiveCharacter();
    }
}
