using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    private void Awake()
    {
        Debug.Log("UI_Manager awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private UI_TurnOrder turnOrderUI;

    [SerializeField]
    private BattleUIParent battleUIParent;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void ActivateTurnOrderUI()
    {
        turnOrderUI.gameObject.SetActive(true);
    }

    public void DeactivateTurnOrder()
    {
        turnOrderUI.gameObject.SetActive(false);
    }
}
