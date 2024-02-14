using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnOrderManager : MonoBehaviour
{
    [SerializeField]
    private List<BattleCharacter> currentTurnOrder = new List<BattleCharacter>();

    [SerializeField]
    private List<BattleCharacter> masterCharacterList = new List<BattleCharacter>();

    public static TurnOrderManager Instance { get; private set; }

    public event Action<string> OnRoundChanged;
    public event Action<List<BattleCharacter>> OnTurnOrderChanged;
    public event Action<BattleCharacter> OnActiveCharacterChanged;
    private int currentRound = 1;
    private BattleCharacter activeCharacter;

    private int maxRounds = 5;

    // Other methods and properties...

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

    private void Start() { }

    public void SetMasterCharacterList(List<BattleCharacter> characters)
    {
        Debug.Log("TurnOrderManager SetMasterCharacterList");
        this.masterCharacterList = characters;
        AddCharactersToTurnOrder(this.masterCharacterList);
    }

    /** Called by BattleManager to populate the Turn Order */
    private void AddCharactersToTurnOrder(List<BattleCharacter> characters)
    {
        foreach (BattleCharacter character in characters)
        {
            currentTurnOrder.Add(character);
            character.OnCharacterDeath += HandleCharacterDeath;
        }

        CreateNewTurnOrder();
    }

    private void HandleCharacterDeath(BattleCharacter character)
    {
        Debug.Log("TurnOrderManager, this guy ded: " + character.CharData.characterName);
        if (currentTurnOrder.Contains(character))
        {
            if (character == activeCharacter)
            {
                StartNextTurn();
            }
            else
            {
                currentTurnOrder.Remove(character);
                OnTurnOrderChanged?.Invoke(currentTurnOrder);
            }
        }
    }

    private void CreateNewTurnOrder()
    {
        Debug.LogError("Create Turn Order for Round: " + currentRound);
        List<BattleCharacter> filteredCharacters = masterCharacterList
            .Where(character => character.CurrentState != CharacterState.Unconscious)
            .ToList();

        currentTurnOrder = new List<BattleCharacter>(filteredCharacters);
        currentTurnOrder.Sort((a, b) => b.GetCharacterData().speed.CompareTo(a.GetCharacterData().speed));

        int turnIndex = 1;
        Debug.LogError("*******************");
        Debug.Log("Turn Order Set!");
        foreach (BattleCharacter turnOrder in currentTurnOrder)
        {
            Debug.Log("Turn #" + turnIndex + ": " + turnOrder.CharData.characterName);
            turnIndex++;
        }
        Debug.LogError("*******************");

        UpdateTurnOrderUI();
        StartNextTurn();
        // StartNextRound();
    }

    private void UpdateTurnOrderUI()
    {
        Debug.Log("UpdateTurnOrderUI");
        OnRoundChanged?.Invoke(currentRound.ToString());
        OnTurnOrderChanged?.Invoke(currentTurnOrder);
    }

    public BattleCharacter GetActiveCharacter()
    {
        return activeCharacter;
    }

    public void StartNextTurn()
    {
        Debug.Log("StartNextTurn");
        activeCharacter = DetermineNextActiveCharacter();
        Debug.Log("New Active Character is: " + activeCharacter.CharData.characterName);
        OnActiveCharacterChanged?.Invoke(activeCharacter);
        OnTurnOrderChanged?.Invoke(currentTurnOrder);
    }

    private BattleCharacter DetermineNextActiveCharacter()
    {
        if (currentTurnOrder.Count > 0)
        {
            return currentTurnOrder[0];
        }
        else
        {
            CreateNewTurnOrder();
            StartNextRound();
            return DetermineNextActiveCharacter();
        }
    }

    public void StartNextRound()
    {
        currentRound++;
        OnRoundChanged?.Invoke(currentRound.ToString());
    }

    public void CharacterTurnComplete(BattleCharacter character)
    {
        Debug.Log(character.GetCharacterData().characterName + "'s turn is complete.");
        int randomSpeed = UnityEngine.Random.Range(1, 11);
        character.SetSpeed(randomSpeed);
        currentTurnOrder.Remove(character);
        OnActiveCharacterChanged?.Invoke(DetermineNextActiveCharacter());
        OnTurnOrderChanged?.Invoke(currentTurnOrder);
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     OnTurnOrderChanged?.Invoke(turnOrder);
        // }
    }

    public List<BattleCharacter> GetCurrentTurnOrder()
    {
        return new List<BattleCharacter>(currentTurnOrder);
    }
}
