using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHealthbar : MonoBehaviour
{
    [SerializeField]
    private Healthbar healthBar;

    private float currentHealth = 1f;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthbar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            DecreaseHealth(0.05f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            IncreaseHealth(0.05f);
        }
    }

    void IncreaseHealth(float amount)
    {
        float previousHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp01(currentHealth);

        // Check if the health value has changed before updating the health bar
        if (!Mathf.Approximately(previousHealth, currentHealth))
        {
            UpdateHealthbar();

            if (Mathf.Approximately(currentHealth, 1.0f))
            {
                Debug.Log("Max Health!");
            }
        }
    }

    void DecreaseHealth(float amount)
    {
        float previousHealth = currentHealth;
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        // Check if the health value has changed before updating the health bar
        if (!Mathf.Approximately(previousHealth, currentHealth))
        {
            UpdateHealthbar();

            if (currentHealth == 0)
            {
                Debug.Log("Health has reached 0!");
            }
        }
    }

    void UpdateHealthbar()
    {
        Debug.Log("UpdateHealthBar!!");
        healthBar.SetSize(currentHealth);
    }
}
