using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private GameObject characterPrefab;

    [SerializeField]
    [Tooltip(
        "Maximum size of the Battle Party. If you have 6 in your Active Party, you can have up to 4 in your Battle Party, and 2 reserves that can be swapped out"
    )]
    private int maxBattlePartySize = 6;

    [Tooltip("Maximum size of the Active Party, (Includes 4 in the Battle Party and 2 reserves)")]
    private int maxActivePartySize = 6;

    [SerializeField]
    private List<CharacterData> activePartyMembers = new List<CharacterData>();

    public List<CharacterData> ActivePartyMembers
    {
        get => activePartyMembers;
        private set => activePartyMembers = value;
    }

    public List<CharacterData> inactivePartyMembers = new List<CharacterData>();

    public List<CharacterData> InactivePartyMembers
    {
        get => inactivePartyMembers;
        private set => inactivePartyMembers = value;
    }

    private List<CharacterData> battlePartymembers = new List<CharacterData>();

    public List<CharacterData> BattlePartymembers
    {
        get => battlePartymembers;
        private set => battlePartymembers = value;
    }

    public void AddCharacterToActiveParty(CharacterData character)
    {
        if (!activePartyMembers.Contains(character) && activePartyMembers.Count < maxActivePartySize)
        {
            activePartyMembers.Add(character);
            InactivePartyMembers.Remove(character);
        }
        else
        {
            Debug.LogError("Active Party is full.");
        }
    }

    public void AddCharacterToBattleParty(CharacterData character)
    {
        // Character will probably not have a position, so fill them in to the next available slot
        if (character.FormationPosition == -1)
        {
            int nextBattlePosition = FindNextAvailablePosition();
            // Debug.Log("NextBattlePosition: " + nextBattlePosition);
            character.FormationPosition = nextBattlePosition;
        }

        if (!BattlePartymembers.Contains(character) && BattlePartymembers.Count < maxBattlePartySize)
        {
            BattlePartymembers.Add(character);
        }
        else
        {
            Debug.LogWarning("Battle Party is full.");
        }
    }

    private int FindNextAvailablePosition()
    {
        List<int> occupiedPositions = new List<int>();
        foreach (var member in BattlePartymembers)
        {
            occupiedPositions.Add(member.FormationPosition);
        }

        int nextPosition = 0;
        while (occupiedPositions.Contains(nextPosition))
        {
            nextPosition++;
        }

        return nextPosition;
    }

    public void RemoveCharacterFromParty(CharacterData character)
    {
        RemoveCharacterFromBattleParty(character);
        RemoveCharacterFromActiveParty(character);
    }

    public void RemoveCharacterFromActiveParty(CharacterData character)
    {
        if (activePartyMembers.Contains(character))
        {
            activePartyMembers.Remove(character);
        }
    }

    public void RemoveCharacterFromBattleParty(CharacterData character)
    {
        if (BattlePartymembers.Contains(character))
        {
            BattlePartymembers.Remove(character);
            character.FormationPosition = -1;
        }
    }
}
