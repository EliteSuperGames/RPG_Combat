using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public int speed;
    public int physicalPower;
    public int magicPower;

    /// <summary> Is this a player or an enemy? </summary>
    public bool playerCharacter;

    ///<summary>How many slots the character will occupy in battle</summary>
    public int battleSize;

    /// <summary>Just placeholder crap until I get different sprites and animators for each character</summary>
    public Color spriteColor;

    /// <summary>Just placeholder crap until I get different sprites and animators for each character</summary>
    public Sprite characterSprite;
    public Sprite characterPortrait;

    /// <summary>The position for the character, is retained between battles</summary>
    public int formationPosition = -1;

    public List<BattleAction> battleActions = new List<BattleAction>();

    public CharacterData(CharacterDataSO data)
    {
        characterName = data.characterName;
        maxHealth = data.maxHealth;
        speed = data.speed;
        currentHealth = data.maxHealth;
        physicalPower = data.physicalPower;
        magicPower = data.magicPower;
        playerCharacter = data.playerCharacter;
        battleSize = data.battleSize;
        spriteColor = data.spriteColor;
        characterSprite = data.characterSprite;
        battleActions = data.battleActions;
        characterPortrait = data.characterPortrait;
    }

    public void ReduceHealth(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public void IncreaseHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
