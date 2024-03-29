using System;
using System.Collections.Generic;
using UnityEngine;

/** Represents the initial data for a Character. Once the Character is instantiated,
its state will be managed by a PartyManager and when the game's data is saved/loaded,
it will save the Character. The BattleCharacter will exist only inside the Battle scene,
and is instantiated when the battle begins by using the Character data
*/

[CreateAssetMenu(fileName = "New Character Data", menuName = "CharacterData")]
[Serializable]
public class CharacterDataSO : ScriptableObject
{
    public string characterName;
    public int maxHealth;
    public int speed;
    public int physicalPower;
    public int magicPower;
    public bool playerCharacter;
    public int battleSize;
    public int magicDefense;
    public int physicalDefense;
    public Color spriteColor;
    public Sprite characterSprite;
    public Sprite characterPortrait;
    public List<AbilityData> abilityList = new List<AbilityData>();
    public Range range;

    [Range(0, 3)]
    public int forwardMovement;

    [Range(0, 3)]
    public int backwardMovement;
}
