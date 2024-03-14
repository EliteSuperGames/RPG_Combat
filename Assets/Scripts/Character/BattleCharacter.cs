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

    [Header("Status Effects")]
    [SerializeField]
    private List<StatusEffect> _statusEffects = new List<StatusEffect>();
    public List<StatusEffect> StatusEffects
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

        HealthbarController = FindObjectOfType<HealthbarController>();
    }

    void Update() { }

    public void InitializeCharacter(CharacterData data)
    {
        Debug.Log("InitializeCharacter");
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

    public void Revive()
    {
        CurrentState = CharacterState.Normal;
        StartCoroutine(ReviveCoroutine());
        OnCharacterRevive?.Invoke(this);
    }

    public void Defend()
    {
        CurrentState = CharacterState.Defend;
    }

    #endregion

    #region Animations
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

    private void ProcessStatusEffectsOnTurnStart()
    {
        int totalPoisonDamage = 0;
        int totalFireDamage = 0;
        int totalHealing = 0;
        foreach (var effect in StatusEffects)
        {
            effect.Update();
        }

        totalPoisonDamage = StatusEffects.OfType<PoisonEffect>().Sum(effect => effect.DamagePerTurn);
        totalFireDamage = StatusEffects.OfType<FireEffect>().Sum(effect => effect.DamagePerTurn);
        totalHealing = StatusEffects.OfType<HealOverTimeEffect>().Sum(effect => effect.healPerTurn);
        StatusEffects.RemoveAll(effect => effect.duration <= 0);

        if (totalPoisonDamage > 0)
        {
            Debug.Log("Taking poison damage: " + totalPoisonDamage);
            TakeDamage(totalPoisonDamage);
        }

        if (totalFireDamage > 0)
        {
            Debug.Log("Taking fire damage: " + totalFireDamage);
            TakeDamage(totalFireDamage);
        }

        if (totalHealing > 0)
        {
            Debug.Log("Healing: " + totalHealing);
            RestoreHealth(totalHealing);
        }
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        StatusEffects.Add(effect);
    }

    #endregion

    #region Getters

    #endregion Getters

    #region Abilities

    public void UseAbility(List<BattleCharacter> targets, Ability ability)
    {
        Debug.Log(CharData.CharacterName + " is using this ability: " + ability.AbilityData.abilityName);
        ability.Use(this, targets);
    }

    #endregion Abilities

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
        Debug.Log("EndTurn for: " + CharData.CharacterName);
    }
    #endregion
}
