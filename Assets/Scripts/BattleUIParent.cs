using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIParent : MonoBehaviour
{
    [SerializeField]
    private UI_ActionPanel actionPanel;

    public void ShowActionPanel()
    {
        this.actionPanel.gameObject.SetActive(true);
    }

    public void HideActionPanel()
    {
        this.actionPanel.gameObject.SetActive(false);
    }
}
