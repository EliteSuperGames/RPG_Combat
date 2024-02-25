using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_TurnOrder : MonoBehaviour
{
    // public List<TMP_Text> turnOrderText = new List<TMP_Text>();
    public TMP_Text roundText;
    private TurnOrderManager turnOrderManager;

    [SerializeField]
    private GameObject turnOrderElementPrefab;

    [SerializeField]
    private Transform turnOrderParent;

    void Awake()
    {
        Debug.Log("UI TurnOrder Awake");
        Initialize();
    }

    private void Initialize()
    {
        turnOrderManager = TurnOrderManager.Instance;
        if (turnOrderManager != null)
        {
            turnOrderManager.OnTurnOrderChanged += UpdateTurnOrderUI;
            turnOrderManager.OnRoundChanged += UpdateRoundUI;
        }
    }

    void Start()
    {
        // TurnOrderManager.Instance.OnTurnOrderChanged += UpdateTurnOrderUI;
    }

    void OnDestroy()
    {
        turnOrderManager.OnTurnOrderChanged -= UpdateTurnOrderUI;
        turnOrderManager.OnRoundChanged -= UpdateRoundUI;
    }

    void UpdateRoundUI(string newRound)
    {
        roundText.text = "Round: " + newRound;
    }

    void UpdateTurnOrderUI(List<BattleCharacter> newTurnOrder)
    {
        Debug.LogError("UpdateTurnOrderUI");

        // Destroy all existing children of turnOrderParent
        foreach (Transform child in turnOrderParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < newTurnOrder.Count; i++)
        {
            BattleCharacter character = newTurnOrder[i];

            GameObject instantiatedObject = Instantiate(turnOrderElementPrefab, turnOrderParent);
            TextMeshProUGUI textMeshPro = instantiatedObject.GetComponentInChildren<TextMeshProUGUI>();
            Image image = instantiatedObject.GetComponentInChildren<Image>();

            if (textMeshPro != null)
            {
                textMeshPro.text = character.GetCharacterData().CharacterName;
                if (character.CurrentState == CharacterState.Stunned)
                {
                    textMeshPro.text += "(S)";
                }
                // Set a different color for the active character (element 0)
                Outline outline = image.GetComponent<Outline>();
                if (i == 0)
                {
                    outline.effectColor = Color.yellow;
                    textMeshPro.color = Color.yellow; // Change to the desired color

                    // instantiatedObject.transform.Find("Background/Active_Character_Indicator").gameObject.SetActive(true);
                }
                else
                {
                    outline.effectColor = Color.black;
                    // instantiatedObject.transform.Find("Background/Active_Character_Indicator").gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in children of: " + instantiatedObject.name);
                Destroy(instantiatedObject);
            }
        }
    }
}
