using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
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
            Debug.Log("Space pressed");
            foreach (CharacterDataSO charDataSO in charactersToMake)
            {
                Debug.Log("character in charactersToMake");

                CharacterData character = new CharacterData(charDataSO);
                partyManager.AddCharacterToActiveParty(character);
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // partyManager.activePartyMembers
            battleManager.PopulatePlayersFromParty(partyManager.ActivePartyMembers);
        }
        // Character newCharacter = characterManager.InstantiateCharacter(character);
        // newCharacter.gameObject.GetComponent<SpriteRenderer>().sprite = character.characterSprite;
        // newCharacter.gameObject.GetComponent<SpriteRenderer>().color = character.spriteColor;
    }
}
