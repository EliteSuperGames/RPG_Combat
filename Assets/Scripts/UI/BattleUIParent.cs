using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIParent : MonoBehaviour
{
    [SerializeField]
    private UI_ActionPanel actionPanel;

    public event Action<Ability> OnAbilityButtonClicked;

    void Awake()
    {
        actionPanel.OnAbilityButtonClicked += HandleAbilityButtonClick;
    }

    void OnDestroy()
    {
        actionPanel.OnAbilityButtonClicked -= HandleAbilityButtonClick;
    }

    public void ShowActionPanelForCharacter(BattleCharacter character, List<EligibleAbility> availableAbilities)
    {
        ShowActionPanel();
        actionPanel.SetAvailableAbilities(availableAbilities);
        actionPanel.SetActiveCharacterData(character);
    }

    public void ClearTargetData(BattleCharacter activeCharacter)
    {
        actionPanel.ClearTargetData(activeCharacter);
    }

    public void ClearCharacterData()
    {
        actionPanel.ClearCharacterData();
    }

    public void SetAbilityData(Ability ability, BattleCharacter activeCharacter)
    {
        actionPanel.SetAbilityData(ability, activeCharacter);
    }

    public void SetTargetCharacterData(BattleCharacter activeCharacter, BattleCharacter targetCharacter)
    {
        actionPanel.SetTargetCharacterData(activeCharacter, targetCharacter);
    }

    private void HandleAbilityButtonClick(Ability ability)
    {
        OnAbilityButtonClicked?.Invoke(ability);
    }

    public void ShowActionPanel()
    {
        this.actionPanel.gameObject.SetActive(true);
    }

    public void HideActionPanel()
    {
        this.actionPanel.gameObject.SetActive(false);
    }
}
