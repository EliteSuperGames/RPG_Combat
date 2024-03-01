using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnOrderManager : MonoBehaviour
{
    [SerializeField]
    private List<BattleCharacter> currentTurnOrder = new();
    private List<BattleCharacter> completedTurnCharacters = new();

    [SerializeField]
    private List<BattleCharacter> masterCharacterList = new();

    public static TurnOrderManager Instance { get; private set; }

    private int currentRound = 1;

    public event Action<string> OnRoundChanged;
    public event Action<List<BattleCharacter>> OnTurnOrderChanged;
    public event Action<BattleCharacter> OnActiveCharacterChanged;

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
        masterCharacterList = characters;
        AddCharactersToTurnOrder(masterCharacterList);
    }

    private void AddCharactersToTurnOrder(List<BattleCharacter> characters)
    {
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
        if (!currentTurnOrder.Contains(character) && !completedTurnCharacters.Contains(character))
        {
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
        currentTurnOrder = currentTurnOrder.Where(character => character.CurrentState != CharacterState.Unconscious).ToList();

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
        OnRoundChanged?.Invoke(currentRound.ToString());
        OnTurnOrderChanged?.Invoke(currentTurnOrder);
        OnActiveCharacterChanged?.Invoke(activeCharacter);
    }

    public BattleCharacter GetActiveCharacter()
    {
        return activeCharacter;
    }

    public void StartTurn(BattleCharacter character)
    {
        character.StartTurn();
    }

    private BattleCharacter DetermineNextActiveCharacter()
    {
        while (currentTurnOrder.Count == 0)
        {
            StartNextRound();
            CreateNewTurnOrder();
        }

        return currentTurnOrder[0];
    }

    public void CharacterTurnComplete(BattleCharacter character)
    {
        character.EndTurn();
        currentTurnOrder.Remove(character);
        completedTurnCharacters.Add(character);
        activeCharacter = DetermineNextActiveCharacter();
        UpdateTurnOrderUI();
    }

    public void StartNextRound()
    {
        currentRound++;
        completedTurnCharacters.Clear();
    }

    private IEnumerator StartNextTurnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public List<BattleCharacter> GetCurrentTurnOrder()
    {
        return new List<BattleCharacter>(currentTurnOrder);
    }

    public BattleCharacter GetNextCharacter()
    {
        activeCharacter = DetermineNextActiveCharacter();
        return activeCharacter;
    }
}
