using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterMovementHandler : MonoBehaviour
{
    public static CharacterMovementHandler Instance { get; private set; }

    public List<BattleCharacter> allCharacters = new List<BattleCharacter>();
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

    // public void StartSwap(BattleCharacter character1, BattleCharacter character2)
    // {
    //     // Debug.Log("CahracterMovementHandler StartSwap");
    //     // Vector3 tempPosition = character1.transform.position;
    //     // character1.targetPosition = character2.transform.position;
    //     // character2.targetPosition = tempPosition;

    //     movingCharacter1 = character1;
    //     movingCharacter2 = character2;
    //     character1.isMoving = true;
    //     character2.isMoving = true;
    // }

    void Update()
    {
        if (allCharacters.Any(character => character.isMoving))
        {
            // Start the SwapCharacters coroutine
            StartCoroutine(SwapCharacters(allCharacters, 0.15f));

            // Clear the isMoving flag for each character
            foreach (BattleCharacter character in allCharacters)
            {
                character.isMoving = false;
            }
        }
    }

    public IEnumerator SwapCharacters(List<BattleCharacter> characters, float duration)
    {
        float elapsedTime = 0;

        // Store the starting and target positions for each character
        List<Vector3> startingPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();
        foreach (BattleCharacter character in characters)
        {
            startingPositions.Add(character.transform.position);
            targetPositions.Add(character.BattlePosition.transform.position);
        }

        while (elapsedTime < duration)
        {
            // Interpolate each character's position
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].transform.position = Vector3.Lerp(startingPositions[i], targetPositions[i], elapsedTime / duration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set each character's position to their target position
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].transform.position = targetPositions[i];
        }
    }

    public void MoveCharacter(
        BattleCharacter movingCharacter,
        int positionsToMove,
        List<BattleCharacter> allCharacters
    // List<BattlePosition> battlePositions
    )
    {
        int currentPosition = movingCharacter.BattlePosition.PositionNumber;
        List<BattlePosition> battlePositions = allCharacters.Select(character => character.BattlePosition).ToList();
        int newPosition = currentPosition + positionsToMove;

        newPosition = Math.Max(0, newPosition);
        newPosition = Math.Min(newPosition, allCharacters.Count - 1);

        movingCharacter.BattlePosition.RemoveOccupyingCharacter();

        if (positionsToMove > 0)
        {
            for (int i = currentPosition; i < newPosition; i++)
            {
                allCharacters[i] = allCharacters[i + 1];
                allCharacters[i].BattlePosition = GetBattlePosition(i, battlePositions);
            }
        }
        else if (positionsToMove < 0)
        {
            for (int i = currentPosition; i > newPosition; i--)
            {
                allCharacters[i] = allCharacters[i - 1];
                allCharacters[i].BattlePosition = GetBattlePosition(i, battlePositions);
            }
        }

        allCharacters[newPosition] = movingCharacter;
        movingCharacter.BattlePosition = GetBattlePosition(newPosition, battlePositions);

        // After all the characters have been moved and their BattlePosition properties have been updated,
        // call SetOccupyingCharacter for each character.
        foreach (BattleCharacter character in allCharacters)
        {
            character.BattlePosition.SetOccupyingCharacter(character);
        }
    }

    private BattlePosition GetBattlePosition(int index, List<BattlePosition> battlePositions)
    {
        return battlePositions.Find(battlePosition => battlePosition.PositionNumber == index);
    }
}
