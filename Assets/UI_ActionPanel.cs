using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    public event Action<BattleAction> OnActionButtonClicked;

    [SerializeField]
    private GameObject actionButtonParent;

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

    private void SetEnemyCharacterData(BattleCharacter enemy, bool enemyIsTarget)
    {
        enemyCharacterName.gameObject.SetActive(true);
        enemyCharacterImage.gameObject.SetActive(true);
        enemyCharacterName.text = enemy.GetCharacterData().characterName;
        enemyCharacterImage.sprite = enemy.GetCharacterData().characterPortrait;

        if (enemyIsTarget)
        {
            enemyCharacterHealth.text = enemy.GetCharacterData().currentHealth + "/" + enemy.GetCharacterData().maxHealth;
        }
    }

    private void SetPlayerCharacterData(BattleCharacter character, bool playerIsTarget)
    {
        characterName.gameObject.SetActive(true);
        characterImage.gameObject.SetActive(true);
        characterName.text = character.GetCharacterData().characterName;
        characterImage.sprite = character.GetCharacterData().characterPortrait;

        if (playerIsTarget)
        {
            characterHealth.text = character.GetCharacterData().currentHealth + "/" + character.GetCharacterData().maxHealth;
        }
    }

    public void SetActiveCharacterData(BattleCharacter activeCharacter)
    {
        if (activeCharacter.IsPlayer())
        {
            SetPlayerCharacterData(activeCharacter, false);
        }
        else
        {
            SetEnemyCharacterData(activeCharacter, false);
        }
    }

    public void ClearEnemyData()
    {
        enemyCharacterImage.sprite = null;
        enemyCharacterName.text = null;
        enemyCharacterImage.gameObject.SetActive(false);
        enemyCharacterName.gameObject.SetActive(false);
        enemyCharacterHealth.text = null;
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
        if (activeCharacter.IsPlayer())
        {
            SetEnemyCharacterData(targetCharacter, true);
        }
        else
        {
            SetPlayerCharacterData(targetCharacter, true);
        }
        // if (targetCharacter.IsPlayer())
        // {
        //     SetPlayerCharacterData(targetCharacter, true);
        // }
        // else
        // {
        //     SetEnemyCharacterData(targetCharacter, true);
        // }
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

    public void SetAvailableActions(List<BattleAction> availableActions)
    {
        Debug.Log("UI_ActionPanel SetAvailableActions");
        ClearActionButtons();
        foreach (BattleAction action in availableActions)
        {
            Debug.Log("action: " + action.ToString());
            CreateActionButton(action);
        }
    }

    private void ClearActionButtons()
    {
        foreach (Transform child in actionButtonParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateActionButton(BattleAction action)
    {
        Debug.Log("CreateActionButton: " + action.ToString());

        // GameObject buttonGO = new GameObject(action.ToString());
        GameObject buttonGO = Instantiate(actionButtonPrefab, actionButtonParent.transform);
        Button button = buttonGO.GetComponent<Button>();

        button.GetComponentInChildren<TMP_Text>().text = action.ToString();
        button.onClick.AddListener(() => HandleActionButtonClick(action));
    }

    private void HandleActionButtonClick(BattleAction action)
    {
        Debug.Log("Button Clicked: " + action.ToString());
        OnActionButtonClicked?.Invoke(action);
    }

    public void SetActionData(BattleAction action, BattleCharacter character)
    {
        if (character.IsPlayer())
        {
            actionText.text = action.ToString();
            enemyActionText.text = null;
        }
        else
        {
            enemyActionText.text = action.ToString();
            actionText.text = null;
        }
    }
}
