using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterMovementHandler : MonoBehaviour
{
    public static CharacterMovementHandler Instance { get; private set; }

    //public List<BattleCharacter> allCharacters = new List<BattleCharacter>();
    public float speed = 35.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update() { }

    public IEnumerator SlideCharacter(BattleCharacter character, Vector3 targetPosition, float duration)
    {
        Vector3 initialPosition = character.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            character.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            yield return null;
        }

        character.transform.position = targetPosition;
    }

    public void MoveCharacter(
        BattleCharacter movingCharacter,
        BattleCharacter targetCharacter,
        List<BattleCharacter> allCharacters,
        List<BattlePosition> battlePositions
    )
    {
        // Find the indices of the moving character and the target character
        int movingIndex = allCharacters.IndexOf(movingCharacter);
        int targetIndex = allCharacters.IndexOf(targetCharacter);

        // Remove the moving character from the list temporarily
        allCharacters.RemoveAt(movingIndex);

        // Insert the moving character at the target index
        allCharacters.Insert(targetIndex, movingCharacter);

        // Set the BattlePosition for each character in the updated list
        for (int i = 0; i < allCharacters.Count; i++)
        {
            BattleCharacter character = allCharacters[i];
            BattlePosition position = GetBattlePosition(i, battlePositions);

            StartCoroutine(SlideCharacter(character, position.transform.position, 0.15f));
            character.SetCurrentBattlePosition(position);
            position.SetOccupyingCharacter(character, true);
        }
    }

    private BattlePosition GetBattlePosition(int index, List<BattlePosition> battlePositions)
    {
        return battlePositions.Find(battlePosition => battlePosition.PositionNumber == index);
    }
}
