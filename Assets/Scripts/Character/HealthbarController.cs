using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarController : MonoBehaviour
{
    public void UpdateHealthBar(BattleCharacter character)
    {
        float healthPercentage = (float)character.CharData.Health / character.CharData.MaxHealth;
        character.BattlePosition.HealthBar.SetSize(healthPercentage);
    }
}
