using System.Collections.Generic;
using UnityEngine;

public class TestGameStarter : MonoBehaviour
{
    [SerializeField]
    private List<CharacterDataSO> charactersToMake;

    [SerializeField]
    private CharacterManager characterManager;

    [SerializeField]
    private PartyManager partyManager;

    [SerializeField]
    private BattleManager battleManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (CharacterDataSO charDataSO in charactersToMake)
            {
                CharacterData character = new CharacterData(charDataSO);
                partyManager.AddCharacterToActiveParty(character);
                partyManager.AddCharacterToBattleParty(character);
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            battleManager.PopulatePlayersFromParty(partyManager.ActivePartyMembers);
        }
    }
}
