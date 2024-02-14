using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacter : MonoBehaviour
{
    [SerializeField]
    private Healthbar HealthBar;

    [SerializeField]
    private GameObject targetableIndicator;
    private SpriteRenderer targetableIndicatorSR;

    [SerializeField]
    private GameObject activeCharacterIndicator;

    [SerializeField]
    private GameObject targetedBG;

    [SerializeField]
    public CharacterData CharData;
    public CharacterState CurrentState;

    private int StunnedDuration = 0;
    private Color hostileColor = new(0f, 0.75f, 1f, 1f);
    private Color hostileFadedColor = new(0f, 0.75f, 1f, 0.75f);

    private Color allyColor = new(1f, 1f, .5f, 1f);
    private Color allyFadedColor = new(1f, 1f, .5f, .75f);

    [SerializeField]
    private int currentBattlePosition = -1;
    public int CurrentBattlePosition
    {
        get { return currentBattlePosition; }
        // placeholder just to remember syntax. battleposition will probably need some logic for setting it
        set { currentBattlePosition = value; }
    }

    public event Action<BattleCharacter> OnCharacterDeath;

    public int MagicPower { get; set; }
    public int PhysicalPower { get; set; }
    public int Speed { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }

    private int startingMagicPower;
    private int startingPhysicalPower;
    private int startingSpeed;
    private int startingHealth;
    private int startingMaxHealth;

    public void InitializeCharacter(CharacterData data)
    {
        CharData = data;
        startingMagicPower = data.magicPower;
        MagicPower = data.magicPower;

        startingPhysicalPower = data.physicalPower;
        PhysicalPower = data.physicalPower;

        startingSpeed = data.speed;
        Speed = data.speed;

        startingMaxHealth = data.maxHealth;
        MaxHealth = data.maxHealth;

        startingHealth = data.currentHealth;
        Health = data.currentHealth;

        // Find SpriteRenderer once
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        targetableIndicatorSR = targetableIndicator.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found on BattleCharacter gameObject.");
            return;
        }

        // Set Sprite
        if (data.characterSprite != null)
        {
            spriteRenderer.sprite = data.characterSprite;
            if (!data.playerCharacter)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            Debug.LogWarning("CharacterSprite is null in CharacterDataSO.");
        }

        // Set Color
        spriteRenderer.color = data.spriteColor;
    }

    public void ShowTargetableIndicator(bool showHostileColor)
    {
        Debug.Log("ShowTargetableIndicator");
        targetableIndicator.gameObject.SetActive(true);
        if (showHostileColor)
        {
            targetableIndicator.gameObject.GetComponent<SpriteRenderer>().color = hostileColor;
        }
        else
        {
            targetableIndicator.gameObject.GetComponent<SpriteRenderer>().color = allyColor;
        }
    }

    public void HideTargetableIndicator()
    {
        Debug.Log("Hide TargetableIndiecator");
        if (targetableIndicator.activeInHierarchy)
        {
            targetableIndicator.gameObject.SetActive(false);
        }
    }

    public void ShowActiveCharacterIndicator()
    {
        activeCharacterIndicator.gameObject.SetActive(true);
    }

    public void HideActiveCharacterIndicator()
    {
        activeCharacterIndicator.gameObject.SetActive(false);
    }

    public void ShowTargetedBG(bool showHostileColor)
    {
        targetedBG.gameObject.SetActive(true);
        if (showHostileColor)
        {
            targetedBG.gameObject.GetComponent<Image>().color = hostileFadedColor;
        }
        else
        {
            targetedBG.gameObject.GetComponent<Image>().color = allyFadedColor;
        }
    }

    public void HideTargetedBG()
    {
        targetedBG.gameObject.SetActive(false);
    }

    public void Attack(BattleCharacter target)
    {
        Debug.Log(CharData.characterName + " is attacking: " + target.GetCharacterData().characterName + "!");
        target.TakeDamage(GetAttackDamage());
    }

    public void Heal(BattleCharacter target)
    {
        Debug.Log(CharData.characterName + " is healing: " + target.GetCharacterData().characterName);

        int healAmount = 15;
        target.RestoreHealth(healAmount);
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

    public CharacterData GetCharacterData()
    {
        return CharData;
    }

    private void Die()
    {
        CurrentState = CharacterState.Unconscious;
        StartCoroutine(RotateAndBounceCoroutine());
        OnCharacterDeath?.Invoke(this);
    }

    private void RestoreHealth(int amountToRestore)
    {
        Debug.Log("restoring: " + amountToRestore + " for : " + CharData.characterName);
        CharData.IncreaseHealth(amountToRestore);
        CharData.IncreaseHealth(amountToRestore);
        HealthBar.SetSize((float)CharData.currentHealth / CharData.maxHealth);
    }

    private void TakeDamage(int damageAmount)
    {
        Debug.Log(this.GetCharacterData().characterName + " is being attacked!");
        Debug.Log("Incoming damage is: " + damageAmount);
        int randomBlock = UnityEngine.Random.Range(0, 2);
        // if (randomBlock == 0)
        // {
        //     CurrentState = CharacterState.Defend;
        // }
        // else
        // {
        //     CurrentState = CharacterState.Normal;
        // }
        if (CurrentState == CharacterState.Defend)
        {
            Debug.Log(GetCharacterData().characterName + " was defending, will take half damage!");
            // charData.ReduceHealth(damageAmount / 2);
            Debug.Log("Damage Amount: " + damageAmount / 2);
            CharData.ReduceHealth(damageAmount / 2);
            HealthBar.SetSize((float)CharData.currentHealth / CharData.maxHealth);
        }
        else
        {
            Debug.Log(CharData.characterName + " was not defending, will take full damage!");
            Debug.Log("Damage amount: " + damageAmount);
            CharData.ReduceHealth(damageAmount);
            HealthBar.SetSize((float)CharData.currentHealth / CharData.maxHealth);
        }
        if (CharData.currentHealth <= 0)
        {
            Die();
        }
    }

    public void StartTurn()
    {
        Debug.Log("StartTurn for: " + CharData.characterName);
        ReduceStatusDurationsOnTurnStart();

        // EndTurn();
    }

    public void EndTurn()
    {
        Debug.Log("EndTurn for: " + CharData.characterName);
    }

    public void Defend()
    {
        CurrentState = CharacterState.Defend;
    }

    private int GetAttackDamage()
    {
        return CharData.physicalPower;
        // Debug.Log("GetAttackDamage: " + baseCharacter.data.physicalPower);
        // return baseCharacter.data.physicalPower;
    }

    public void ApplyStunned(int stunStacks)
    {
        StunnedDuration += stunStacks;
        CurrentState = CharacterState.Stunned;
    }

    private void ReduceStatusDurationsOnTurnStart()
    {
        Debug.Log("ReduceStatusDurationsOnTurnStart");
        StunnedDuration--;
        if (StunnedDuration <= 0)
        {
            StunnedDuration = 0;
        }

        if (StunnedDuration == 0)
        {
            CurrentState = CharacterState.Normal;
        }
    }

    public void PerformStunAttack(BattleCharacter target)
    {
        // just hard coding for now, will add abilities later with effects like stun duration
        const int stunDuration = 1;
        target.ApplyStunned(stunDuration);
    }

    public int GetPositionIndex()
    {
        return CurrentBattlePosition;
    }

    public void SetPositionIndex(int positionIndex)
    {
        CurrentBattlePosition = positionIndex;
    }

    public bool IsPlayer()
    {
        return CharData.playerCharacter;
    }

    public void EndCharacterTurn()
    {
        TurnOrderManager.Instance.CharacterTurnComplete(this);
    }

    public void SetSpeed(int val)
    {
        Speed = val;
    }
}
