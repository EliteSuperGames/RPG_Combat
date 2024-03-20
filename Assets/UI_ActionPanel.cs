using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The Action Panel is the screen that includes buttons like "Attack", "Defend" etc.
/// When an action is selected, it will display the next step in the action process
/// If it is an Item, it will display the UI_ItemPanel which has the list of items
/// If it is a Spell, it will display the UI_SpellPanel for the list of spells
/// Once complete, sends the Action and Target back to BattleManager
/// </summary>
public class UI_ActionPanel : MonoBehaviour
{
    public event Action<Ability> OnAbilityButtonClicked;

    [SerializeField]
    private GameObject actionButtonParent;

    [SerializeField]
    private GameObject staticButtonParent;

    [SerializeField]
    private GameObject characterDisplay;

    [SerializeField]
    private GameObject enemyDisplay;

    [SerializeField]
    private Image characterImage;

    [SerializeField]
    private TMP_Text characterName;

    [SerializeField]
    private GameObject actionButtonPrefab;

    [SerializeField]
    private Image enemyCharacterImage;

    [SerializeField]
    private TMP_Text enemyCharacterName;

    [SerializeField]
    public TMP_Text actionText;

    [SerializeField]
    public TMP_Text enemyActionText;

    [SerializeField]
    public TMP_Text characterHealth;

    [SerializeField]
    public TMP_Text enemyCharacterHealth;

    [SerializeField]
    private GameObject battleMessageGO;

    [SerializeField]
    private TMP_Text battleMessageText;

    [SerializeField]
    public AbilityData changePositionsAbilityData;

    [SerializeField]
    public AbilityData skipTurnAbilityData;

    [SerializeField]
    public AbilityData useItemAbilityData;

    private void SetEnemyCharacterData(BattleCharacter enemy, bool enemyIsTarget)
    {
        enemyCharacterName.gameObject.SetActive(true);
        enemyCharacterImage.gameObject.SetActive(true);
        enemyCharacterName.text = enemy.GetCharacterData().CharacterName;
        enemyCharacterImage.sprite = enemy.GetCharacterData().CharacterPortrait;

        if (enemyIsTarget)
        {
            enemyCharacterHealth.text = enemy.GetCharacterData().Health + "/" + enemy.GetCharacterData().MaxHealth;
        }
    }

    private void SetPlayerCharacterData(BattleCharacter character, bool playerIsTarget)
    {
        characterName.gameObject.SetActive(true);
        characterImage.gameObject.SetActive(true);
        characterName.text = character.GetCharacterData().CharacterName;
        characterImage.sprite = character.GetCharacterData().CharacterPortrait;

        if (playerIsTarget)
        {
            characterHealth.text = character.GetCharacterData().Health + "/" + character.GetCharacterData().MaxHealth;
        }
    }

    public void SetActiveCharacterData(BattleCharacter activeCharacter)
    {
        if (activeCharacter.PlayerCharacter)
        {
            SetPlayerCharacterData(activeCharacter, false);
        }
        else
        {
            SetEnemyCharacterData(activeCharacter, false);
        }
    }

    public void ClearTargetData(BattleCharacter activeCharacter)
    {
        if (activeCharacter.PlayerCharacter)
        {
            enemyCharacterImage.sprite = null;
            enemyCharacterName.text = null;
            enemyCharacterHealth.text = null;
        }
        else
        {
            characterImage.sprite = null;
            characterName.text = null;
            characterHealth.text = null;
        }
    }

    public void ClearPlayerData()
    {
        characterImage.sprite = null;
        characterName.text = null;
        characterImage.gameObject.SetActive(false);
        characterName.gameObject.SetActive(false);
        characterHealth.text = null;
    }

    public void SetTargetCharacterData(BattleCharacter activeCharacter, BattleCharacter targetCharacter)
    {
        if (activeCharacter.PlayerCharacter)
        {
            SetEnemyCharacterData(targetCharacter, true);
        }
        else
        {
            SetPlayerCharacterData(targetCharacter, true);
        }
    }

    public void ClearCharacterData()
    {
        characterName.text = null;
        characterImage.sprite = null;
        characterName.gameObject.SetActive(false);
        characterImage.gameObject.SetActive(false);
        enemyCharacterName.text = null;
        enemyCharacterImage.sprite = null;
        enemyCharacterName.gameObject.SetActive(false);
        enemyCharacterImage.gameObject.SetActive(false);
        actionText.text = null;
        enemyActionText.text = null;
        characterHealth.text = null;
        enemyCharacterHealth.text = null;
    }

    public void SetAvailableAbilities(List<EligibleAbility> availableAbilities)
    {
        Debug.Log("SetAvailableAbilities");
        Debug.Log("Available Abilities: " + availableAbilities.Count);
        ClearActionButtons();

        // Create a new list that includes both the character's abilities and the static abilities
        List<EligibleAbility> allAbilities = new List<EligibleAbility>(availableAbilities)
        {
            CreateEligibleAbility(changePositionsAbilityData),
            CreateEligibleAbility(skipTurnAbilityData),
            CreateEligibleAbility(useItemAbilityData)
        };

        foreach (EligibleAbility eligibleAbility in allAbilities)
        {
            Debug.Log("Making ability buttons");
            CreateAbilityButton(
                eligibleAbility.Ability,
                eligibleAbility.EligibleBasedOnLaunchPosition,
                eligibleAbility.EligibleBasedOnLandingTargets
            );
        }
    }

    private EligibleAbility CreateEligibleAbility(AbilityData abilityData)
    {
        Ability ability = new Ability(abilityData);
        return new EligibleAbility(ability) { EligibleBasedOnLandingTargets = true, EligibleBasedOnLaunchPosition = true };
    }

    private void ClearActionButtons()
    {
        List<Transform> parents = new List<Transform> { actionButtonParent.transform, staticButtonParent.transform };

        foreach (Transform parent in parents)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateAbilityButton(Ability ability, bool IsEligibleBasedOnOwnPosition, bool IsEligibleBasedOnTargetPositions)
    {
        Debug.Log("CreateAbilityButton: " + IsEligibleBasedOnOwnPosition + " " + IsEligibleBasedOnTargetPositions);
        GameObject buttonGO;
        List<string> staticAbilities = new List<string> { "Change Positions", "Skip Turn", "Use Item" };

        if (staticAbilities.Contains(ability.AbilityData.abilityName.ToString()))
        {
            buttonGO = Instantiate(actionButtonPrefab, staticButtonParent.transform);
        }
        else
        {
            buttonGO = Instantiate(actionButtonPrefab, actionButtonParent.transform);
        }

        Button button = buttonGO.GetComponent<Button>();

        button.GetComponentInChildren<TMP_Text>().text = ability.AbilityData.abilityName.ToString();

        bool onCooldown = ability.CooldownTimer > 0;
        button.interactable = IsEligibleBasedOnOwnPosition && IsEligibleBasedOnTargetPositions && !onCooldown;
        AbilityButtonHoverListener hoverListener = button.AddComponent<AbilityButtonHoverListener>();
        hoverListener.ability = ability;
        if (!IsEligibleBasedOnOwnPosition)
        {
            hoverListener.OnPointerEnterEvent += (eventData) => DisabledAbilityPointerEnter(eventData, "Out Of Position");
            hoverListener.OnPointerExitEvent += DisabledAbilityPointerExit;
        }
        else if (!IsEligibleBasedOnTargetPositions)
        {
            hoverListener.OnPointerEnterEvent += (eventData) => DisabledAbilityPointerEnter(eventData, "No Target Available");
            hoverListener.OnPointerExitEvent += DisabledAbilityPointerExit;
        }
        else if (onCooldown)
        {
            hoverListener.OnPointerEnterEvent += (eventData) => DisabledAbilityPointerEnter(eventData, "On Cooldown");
            hoverListener.OnPointerExitEvent += DisabledAbilityPointerExit;
        }

        button.onClick.AddListener(() => HandleAbilityButtonClick(ability));
    }

    private void DisabledAbilityPointerEnter(PointerEventData eventData, string message)
    {
        battleMessageGO.gameObject.SetActive(true);
        battleMessageText.text = message;
    }

    private void DisabledAbilityPointerExit(PointerEventData eventData)
    {
        battleMessageGO.gameObject.SetActive(false);
    }

    private void HandleAbilityButtonClick(Ability ability)
    {
        OnAbilityButtonClicked?.Invoke(ability);
    }

    public void SetAbilityData(Ability ability, BattleCharacter character)
    {
        if (character.PlayerCharacter)
        {
            actionText.text = ability.AbilityData.abilityName.ToString();
            enemyActionText.text = null;
        }
        else
        {
            enemyActionText.text = ability.AbilityData.abilityName.ToString();
            actionText.text = null;
        }
    }
}
