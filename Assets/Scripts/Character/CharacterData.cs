using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [SerializeField]
    private string _characterName;
    public string CharacterName
    {
        get { return _characterName; }
        private set { _characterName = value; }
    }

    [SerializeField]
    private int _maxHealth;
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    [SerializeField]
    private int _health;
    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }

    [SerializeField]
    private int _speed;
    public int Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    [SerializeField]
    private int _physicalPower;
    public int PhysicalPower
    {
        get { return _physicalPower; }
        set { _physicalPower = value; }
    }

    [SerializeField]
    private int _magicPower;
    public int MagicPower
    {
        get { return _magicPower; }
        set { _magicPower = value; }
    }

    [SerializeField]
    private List<Ability> _abilities = new List<Ability>();
    public List<Ability> Abilities
    {
        get { return _abilities; }
        set { _abilities = value; }
    }

    [SerializeField]
    private bool _playerCharacter;
    public bool PlayerCharacter
    {
        get { return _playerCharacter; }
        private set { _playerCharacter = value; }
    }

    [SerializeField]
    private int _battleSize;
    public int BattleSize
    {
        get { return _battleSize; }
        private set { _battleSize = value; }
    }

    [SerializeField]
    private Color _spriteColor;
    public Color SpriteColor
    {
        get { return _spriteColor; }
        private set { _spriteColor = value; }
    }

    [SerializeField]
    private Sprite _characterSprite;
    public Sprite CharacterSprite
    {
        get { return _characterSprite; }
        private set { _characterSprite = value; }
    }

    [SerializeField]
    private Sprite _characterPortrait;
    public Sprite CharacterPortrait
    {
        get { return _characterPortrait; }
        private set { _characterPortrait = value; }
    }

    [SerializeField]
    private int _formationPosition = -1;
    public int FormationPosition
    {
        get { return _formationPosition; }
        set { _formationPosition = value; }
    }

    public CharacterData(CharacterData data)
    {
        CharacterName = data.CharacterName;
        MaxHealth = data.MaxHealth;
        Speed = data.Speed;
        Health = data.MaxHealth;
        PhysicalPower = data.PhysicalPower;
        MagicPower = data.MagicPower;
        PlayerCharacter = data.PlayerCharacter;
        BattleSize = data.BattleSize;
        SpriteColor = data.SpriteColor;
        CharacterSprite = data.CharacterSprite;
        CharacterPortrait = data.CharacterPortrait;
        Abilities = data.Abilities;
    }

    public CharacterData(CharacterDataSO data)
    {
        CharacterName = data.characterName;
        MaxHealth = data.maxHealth;
        Speed = data.speed;
        Health = data.maxHealth;
        PhysicalPower = data.physicalPower;
        MagicPower = data.magicPower;
        PlayerCharacter = data.playerCharacter;
        BattleSize = data.battleSize;
        SpriteColor = data.spriteColor;
        CharacterSprite = data.characterSprite;
        CharacterPortrait = data.characterPortrait;
        Abilities = ConvertAbilitiesToInstances(data.abilityList);
    }

    private List<Ability> ConvertAbilitiesToInstances(List<AbilityData> abiltiies)
    {
        List<Ability> abilityInstances = new List<Ability>();
        foreach (var currentAbility in abiltiies)
        {
            Ability instance = new Ability { AbilityData = currentAbility, CooldownTimer = 0f };
            abilityInstances.Add(instance);
        }
        return abilityInstances;
    }

    public void ReduceHealth(int amount)
    {
        Health -= amount;
        if (Health < 0)
        {
            Health = 0;
        }
    }

    public void IncreaseHealth(int amount)
    {
        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
}
