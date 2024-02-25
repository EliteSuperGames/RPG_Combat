using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private GameObject characterPrefab;

    private List<CharacterData> allCharacters = new List<CharacterData>();

    public void InstantiateCharacter(CharacterDataSO characterDataSO)
    {
        // GameObject newCharacter = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
        // CharacterData characterScript = newCharacter.GetComponent<Character>();
        // // characterScript.Initialize(characterData);
        // newCharacter.name = characterDataSO.characterName;
        // return characterScript;
    }
}
