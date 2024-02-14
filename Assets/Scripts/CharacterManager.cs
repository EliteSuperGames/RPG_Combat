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

    private List<Character> allCharacters = new List<Character>();

    public Character InstantiateCharacter(CharacterDataSO characterData)
    {
        GameObject newCharacter = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
        Character characterScript = newCharacter.GetComponent<Character>();
        // characterScript.Initialize(characterData);
        newCharacter.name = characterData.characterName;
        return characterScript;
    }
}
