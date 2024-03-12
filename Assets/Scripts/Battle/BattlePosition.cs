using UnityEngine;
using UnityEngine.UI;

public class BattlePosition : MonoBehaviour
{
    [SerializeField]
    private int positionNumber;

    public int PositionNumber
    {
        get { return positionNumber; }
        private set { positionNumber = value; }
    }

    private Color hostileColor = new(0f, 0.75f, 1f, 1f);
    private Color hostileFadedColor = new(0f, 0.75f, 1f, 0.75f);

    private Color allyColor = new(1f, 1f, .5f, 1f);
    private Color allyFadedColor = new(1f, 1f, .5f, .75f);

    [SerializeField]
    private BattleCharacter battleCharacter;

    [SerializeField]
    private Healthbar healthBar;

    public Healthbar HealthBar
    {
        get { return healthBar; }
        private set { healthBar = value; }
    }

    [SerializeField]
    private GameObject activeCharacterIndicator;

    [SerializeField]
    private GameObject targetableCharacterSprite;

    [SerializeField]
    private GameObject targetedBGSprite;

    public bool IsTargetable { get; set; }

    public void RemoveOccupyingCharacter()
    {
        battleCharacter.transform.SetParent(null);
        battleCharacter.SetCurrentBattlePosition(null);
        battleCharacter = null;
    }

    public void SetOccupyingCharacter(BattleCharacter character, bool setPosition)
    {
        battleCharacter = character;
        character.SetCurrentBattlePosition(this);
        character.transform.SetParent(transform);
        if (setPosition)
        {
            character.transform.position = transform.position;
        }
        healthBar.gameObject.SetActive(true);
        character.RefreshHealthbar();
    }

    public BattleCharacter GetOccupyingBattleCharacter()
    {
        return battleCharacter;
    }

    public void EnableHealthObject()
    {
        healthBar.gameObject.SetActive(true);
    }

    public void HideHealthObject()
    {
        healthBar.gameObject.SetActive(false);
    }

    public void EnableActiveCharacterIndicator()
    {
        activeCharacterIndicator.gameObject.SetActive(true);
    }

    public void HideActiveCharacterIndicator()
    {
        activeCharacterIndicator.gameObject.SetActive(false);
    }

    public void EnableTargetableIndicator(bool showHostileColor)
    {
        targetableCharacterSprite.gameObject.SetActive(true);

        if (showHostileColor)
        {
            targetableCharacterSprite.gameObject.GetComponent<SpriteRenderer>().color = hostileColor;
        }
        else
        {
            targetableCharacterSprite.gameObject.GetComponent<SpriteRenderer>().color = allyColor;
        }
    }

    public void HideTargetableIndicator()
    {
        targetableCharacterSprite.gameObject.SetActive(false);
    }

    public void EnableTransparentTarget(bool showHostileColor)
    {
        targetedBGSprite.gameObject.SetActive(true);
        if (showHostileColor)
        {
            targetedBGSprite.gameObject.GetComponent<Image>().color = hostileFadedColor;
        }
        else
        {
            targetedBGSprite.gameObject.GetComponent<Image>().color = allyFadedColor;
        }
    }

    public void HideTransparentTarget()
    {
        targetedBGSprite.gameObject.SetActive(false);
    }
}
