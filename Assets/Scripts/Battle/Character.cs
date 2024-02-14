using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

[System.Serializable]
public class Character : MonoBehaviour
{
    public CharacterData characterData;

    private int currentHealth;
    public string characterName;
    public int maxHealth;
    public int speed;
    public int physicalPower;
    public int magicPower;
    public bool isPlayer;
    public SpriteRenderer characterSpriteRenderer;
    public int formationPosition;
    public int battleSize;

    public void Initialize(CharacterData data)
    {
        this.characterData = data;
        characterName = data.characterName;
        maxHealth = data.maxHealth;
        speed = data.speed;
        physicalPower = data.physicalPower;
        magicPower = data.magicPower;
        isPlayer = data.playerCharacter;
        battleSize = data.battleSize;
        formationPosition = data.formationPosition;
    }

    private void LoadFromSpriteSheet(string sheetAddressableKey, string spriteName, System.Action<Sprite> callback)
    {
        Addressables.LoadAssetAsync<SpriteAtlas>(sheetAddressableKey).Completed += operation =>
        {
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                SpriteAtlas spriteAtlas = operation.Result;

                // Get the sprite with the specified name
                Sprite sprite = spriteAtlas.GetSprite(spriteName);

                callback?.Invoke(sprite);
            }
        };
    }

    private void OnSpriteLoaded(Sprite sprite)
    {
        characterSpriteRenderer.sprite = sprite;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrenthealth(int health)
    {
        currentHealth = health;
    }

    public void ReduceHealth(int damage)
    {
        Debug.Log("reducing health for: " + characterName);
        Debug.Log("Current health: " + currentHealth);
        Debug.Log("Incoming damage: " + damage);
        currentHealth -= damage;
        Debug.Log("New Health: " + currentHealth);
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public void IncreaseHealth(int healthToAdd)
    {
        currentHealth += healthToAdd;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public int GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(int newSpeed)
    {
        speed = newSpeed;
    }

    public string GetCharacterName()
    {
        return characterName;
    }

    public void SetPhysicalPower(int power)
    {
        physicalPower = power;
    }

    public void SetMagicPower(int power)
    {
        magicPower = power;
    }

    public int GetPhysicalPower()
    {
        return physicalPower;
    }

    public int GetMagicPower()
    {
        return magicPower;
    }

    public int GetBattleSize()
    {
        return characterData.battleSize;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(int maxHP)
    {
        maxHealth = maxHP;
    }

    public void SetName(string name)
    {
        characterName = name;
    }
}
