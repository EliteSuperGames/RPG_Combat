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

    /// <summary> Is this a player or an enemy? </summary>
    public bool playerCharacter;

    ///<summary>How many slots the character will occupy in battle</summary>
    public int battleSize;
    public Color spriteColor;
    public Sprite characterSprite;
    public Sprite characterPortrait;
    public List<AbilityData> abilityList = new List<AbilityData>();

    [Range(0, 3)]
    public int forwardMovement;

    [Range(0, 3)]
    public int backwardMovement;
    // public SpriteRenderer characterSpriteRenderer;
    // public string spriteAddressableKey;
    // public string initialSpriteName;
    // public Sprite characterSprite;
    // public AnimatorOverrideController characterAnimatorController;
}
