using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacter : MonoBehaviour
{
    [SerializeField]
    [Header("Character Data")]
    private CharacterData _charData;
    public CharacterData CharData
    {
        get { return _charData; }
        set { _charData = value; }
    }

    [SerializeField]
    [Header("InitialCharacterData - snapshot of character data")]
    private CharacterData _initialCharacterData;
    public CharacterData InitialCharacterData
    {
        get { return _initialCharacterData; }
        set { _initialCharacterData = value; }
    }

    [SerializeField]
    [Header("Character State")]
    private CharacterState _currentState;

    public CharacterState CurrentState
    {
        get { return _currentState; }
        private set { _currentState = value; }
    }

    [SerializeField]
    [Header("Status Effects")]
    private List<EffectInstance> _statusEffects = new List<EffectInstance>();

    public List<EffectInstance> StatusEffects
    {
        get { return _statusEffects; }
        set { _statusEffects = value; }
    }

    [SerializeField]
    [Header("Abilities")]
    private List<Ability> _abilities = new List<Ability>();

    public List<Ability> Abilities
    {
        get { return _abilities; }
        private set { _abilities = value; }
    }

    [Header("Battle Position")]
    private BattlePosition _battlePosition;
    public BattlePosition BattlePosition
    {
        get { return _battlePosition; }
        set { _battlePosition = value; }
    }

    [SerializeField]
    [Header("Formation Position")]
    private int formationPosition;
    public int FormationPosition { get; private set; }

    public event Action<BattleCharacter> OnCharacterDeath;
    public event Action<BattleCharacter> OnCharacterRevive;

    private List<StatModifier> _positiveStatModifiers = new List<StatModifier>();

    public List<StatModifier> PositiveStatModifiers
    {
        get { return _positiveStatModifiers; }
        set { _positiveStatModifiers = value; }
    }

    private List<StatModifier> _negativeStatModifiers = new List<StatModifier>();

    public List<StatModifier> NegativeStatModifiers
    {
        get { return _negativeStatModifiers; }
        set { _negativeStatModifiers = value; }
    }

    public bool PlayerCharacter { get; private set; }
    public Vector3 targetPosition;
    public float moveSpeed = 1f;
    public bool isMoving = false;

    // public static event Action<BattleCharacter, int> OnSwapPositionsRequested = delegate { };

    [SerializeField]
    [Header("Other")]
    private bool _hasUsedTurn;
    public bool HasUsedTurn => _hasUsedTurn;

    [SerializeField]
    private HealthbarController HealthbarController;

    #region Initialization logic
    public void Awake()
    {
        // Initialize the lists if needed
        if (PositiveStatModifiers == null)
            PositiveStatModifiers = new List<StatModifier>();

        if (NegativeStatModifiers == null)
            NegativeStatModifiers = new List<StatModifier>();
        HealthbarController = FindObjectOfType<HealthbarController>();
    }

    void Update()
    {
        // if (isMoving)
        // {
        //     transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        //     if (transform.position == targetPosition)
        //     {
        //         isMoving = false;
        //         if (!target.isMoving) { }
        //     }
        // }
    }

    public void InitializeCharacter(CharacterData data)
    {
        CharData = data;
        InitialCharacterData = new CharacterData(data);
        PlayerCharacter = data.PlayerCharacter;
        FormationPosition = data.FormationPosition;

        Abilities = data.Abilities;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found on BattleCharacter gameObject.");
            return;
        }

        if (data.CharacterSprite != null)
        {
            spriteRenderer.sprite = data.CharacterSprite;
            if (!data.PlayerCharacter)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            Debug.LogWarning("CharacterSprite is null in CharacterDataSO.");
        }

        spriteRenderer.color = data.SpriteColor;
    }

    public CharacterData GetCharacterData()
    {
        return CharData;
    }

    #endregion

    #region Perform actions to others
    public void Attack(BattleCharacter target)
    {
        Debug.Log(CharData.CharacterName + " is attacking: " + target.GetCharacterData().CharacterName + "!");
        target.TakeDamage(GetAttackDamage());
    }

    public void Heal(BattleCharacter target)
    {
        Debug.Log(CharData.CharacterName + " is healing: " + target.GetCharacterData().CharacterName);

        int healAmount = 15;
        target.RestoreHealth(healAmount);
    }
    #endregion

    #region Modify Own Stats

    public void SetCurrentBattlePosition(BattlePosition position)
    {
        Debug.Log("SetCurrentBattlePosition for " + CharData.CharacterName + " to " + position.PositionNumber);
        BattlePosition = position;
        FormationPosition = position.PositionNumber;
    }

    public void RestoreHealth(int amountToRestore)
    {
        Debug.Log(CharData.CharacterName + " restoreHealth: " + amountToRestore);
        CharData.IncreaseHealth(amountToRestore);
        HealthbarController.UpdateHealthBar(this);
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.Log(CharData.CharacterName + " takeDamage: " + damageAmount);
        CharData.ReduceHealth(damageAmount);
        HealthbarController.UpdateHealthBar(this);

        if (CharData.Health <= 0)
        {
            Die();
        }
    }

    public void RefreshHealthbar()
    {
        HealthbarController.UpdateHealthBar(this);
    }

    private void Die()
    {
        CurrentState = CharacterState.Unconscious;
        StartCoroutine(RotateAndBounceCoroutine());
        OnCharacterDeath?.Invoke(this);
    }

    private void Revive()
    {
        CurrentState = CharacterState.Normal;
        StartCoroutine(ReviveCoroutine());
        OnCharacterRevive?.Invoke(this);
    }

    IEnumerator ReviveCoroutine()
    {
        Transform transform = GetComponent<Transform>();

        // Define the target rotation (back to upright)
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);

        // Define the bounce height on the Y-axis
        float bounceHeight = 0.5f;

        // Define the duration of the bounce and rotation in seconds
        float bounceDuration = 0.1f;
        float rotationDuration = 0.2f;

        // Get the initial position and rotation
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // Rotate back to upright
        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            // Interpolate between the start and target rotations based on the elapsed time
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / rotationDuration);

            // Increment the elapsed time
            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final rotation is set to the target rotation
        transform.rotation = targetRotation;

        // Wait for a short delay (you can adjust this)
        yield return new WaitForSeconds(0.01f);

        // Bounce Down
        elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            float newY = Mathf.Lerp(startPosition.y, startPosition.y - bounceHeight, elapsed / bounceDuration);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is set to the target position
        transform.position = new Vector3(transform.position.x, startPosition.y - bounceHeight, transform.position.z);
    }

    public void Defend()
    {
        // CurrentState = CharacterState.Defend;
    }

    #endregion

    #region Animations
    IEnumerator RotateAndBounceCoroutine()
    {
        Transform transform = GetComponent<Transform>();

        // Define the target rotation
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, -90f);

        // Define the bounce height on the Y-axis
        float bounceHeight = 0.5f;

        // Define the duration of the bounce and rotation in seconds
        float bounceDuration = 0.1f;
        float rotationDuration = 0.2f;

        // Get the initial position and rotation
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // Bounce Up
        float elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            float newY = Mathf.Lerp(startPosition.y, startPosition.y + bounceHeight, elapsed / bounceDuration);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is set to the target position
        transform.position = new Vector3(transform.position.x, startPosition.y + bounceHeight, transform.position.z);

        // Wait for a short delay (you can adjust this)
        yield return new WaitForSeconds(0.01f);

        // Rotate
        elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            // Interpolate between the start and target rotations based on the elapsed time
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / rotationDuration);

            // Increment the elapsed time
            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final rotation is set to the target rotation
        transform.rotation = targetRotation;
    }
    #endregion

    #region Status Effects

    public EffectInstance GetEffectInstance(StatusEffectData effectData)
    {
        foreach (EffectInstance effectInstance in StatusEffects)
        {
            if (effectInstance.effectData == effectData)
            {
                return effectInstance;
            }
        }
        return null;
    }

    public void ApplyStatModifier(StatModifier statModifier)
    {
        Debug.Log("ApplyStatModifier");
        Debug.Log(statModifier.modifierName);
        List<StatModifier> statModifiers;

        if (statModifier.positiveOrNegative == PositiveOrNegativeMod.Positive)
        {
            statModifiers = PositiveStatModifiers;
        }
        else
        {
            statModifiers = NegativeStatModifiers;
        }

        if (!statModifiers.Contains(statModifier))
        {
            statModifiers.Add(statModifier);
            ApplyModifierEffects(statModifier);
        }
    }

    private void ApplyModifierEffects(StatModifier statModifier)
    {
        Debug.Log("ApplyModifierEffects");
        Debug.Log(statModifier);
        CharData.MagicPower += statModifier.magicPowerMod;
        CharData.PhysicalPower += statModifier.physicalPowerMod;
        Debug.Log("Speed Mod: " + statModifier.speedMod);
        Debug.Log("Speed before mod: " + CharData.Speed);

        CharData.Speed += statModifier.speedMod;
        Debug.Log("Speed after mod: " + CharData.Speed);
        CharData.MaxHealth += statModifier.maxHealthMod;

        // Ensure that Health and MaxHealth don't go below 1 (character shouldn't die from a stat modifier)
        CharData.Health = Mathf.Max(CharData.Health, 1);
        CharData.MaxHealth = Mathf.Max(CharData.MaxHealth, 1);
    }

    public void RemoveStatModifierEffects(StatModifier statModEffect)
    {
        List<StatModifier> statModifiers;

        if (statModEffect.positiveOrNegative == PositiveOrNegativeMod.Positive)
        {
            statModifiers = PositiveStatModifiers;
        }
        else
        {
            statModifiers = NegativeStatModifiers;
        }

        statModifiers.Remove(statModEffect);

        CharData.MagicPower -= statModEffect.magicPowerMod;
        CharData.PhysicalPower -= statModEffect.physicalPowerMod;
        CharData.Speed -= statModEffect.speedMod;
        CharData.MaxHealth -= statModEffect.maxHealthMod;
        CharData.MaxHealth = Mathf.Max(CharData.MaxHealth, 1);
        if (CharData.MagicPower > InitialCharacterData.MagicPower)
        {
            CharData.MagicPower = InitialCharacterData.MagicPower;
        }

        if (CharData.PhysicalPower > InitialCharacterData.PhysicalPower)
        {
            CharData.PhysicalPower = InitialCharacterData.PhysicalPower;
        }

        if (CharData.Speed > InitialCharacterData.Speed)
        {
            CharData.Speed = InitialCharacterData.Speed;
        }

        if (CharData.MaxHealth > InitialCharacterData.MaxHealth)
        {
            CharData.MaxHealth = InitialCharacterData.MaxHealth;
        }
    }

    private void RemoveEffect(EffectType effectType)
    {
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            var effect = StatusEffects[i];
        }
    }

    private void ProcessStatusEffectsOnTurnStart()
    {
        Debug.Log("ReduceStatusDurationsOnTurnStart for " + CharData.CharacterName);

        EffectHandler.ProcessAllPeriodicEffects(this);
        if (CurrentState == CharacterState.Stunned)
        {
            TurnOrderManager.Instance.CharacterTurnComplete(this);
        }
    }
    #endregion

    #region Getters
    private int GetAttackDamage()
    {
        return CharData.PhysicalPower;
    }

    #endregion

    #region Abilities

    public void UseSingleTargetAbility(BattleCharacter target, Ability ability)
    {
        Debug.Log(CharData.CharacterName + " is using this ability: " + ability.AbilityData.abilityName + " on " + target.CharData.CharacterName);
        ability.ExecuteAbility(this, target);
    }

    public void UseAbility(List<BattleCharacter> targets, Ability ability)
    {
        Debug.Log(CharData.CharacterName + " is using this ability: " + ability.AbilityData.abilityName);
        foreach (var target in targets)
        {
            if (target.CurrentState == CharacterState.Unconscious)
            {
                if (ability.AbilityData.effects.Any(effect => effect.effectType == EffectType.Revive))
                {
                    target.Revive();
                }
            }
            else if (!ability.AbilityData.effects.Any(effect => effect.effectType == EffectType.MoveSelf))
            {
                ability.ExecuteAbility(this, target);
            }
            else if (ability.AbilityData.effects.Any(effect => effect.effectType == EffectType.MoveSelf))
            {
                Debug.Log("battleCharacter ability has a moveself effect");
                // get the distance that character will move forward.
                // get reference of the ally character that is in the position that the character will move to.
                // swap the positions of the characters.
                // then call ability.ExecuteAbility(this, target);
            }
        }
    }

    public void AddStatusEffect(BattleCharacter caster, StatusEffectData effectToAdd, int damagePerRound = 0)
    {
        EffectInstance instance = new EffectInstance()
        {
            effectData = effectToAdd,
            remainingDuration = effectToAdd.effectDuration,
            characterAppliedTo = CharData.CharacterName,
            characterApplying = caster.CharData.CharacterName
        };

        StatusEffects.Add(instance);
    }

    #endregion

    #region TurnOrder Logic

    public void SetCurrentState(CharacterState characterState)
    {
        CurrentState = characterState;
    }

    public void StartTurn()
    {
        ProcessStatusEffectsOnTurnStart();
    }

    public void EndTurn()
    {
        StatusEffects = EffectHandler.ReduceDurationForAllEffects(this, StatusEffects);
        Debug.Log("EndTurn for: " + CharData.CharacterName);
    }
    #endregion
}
