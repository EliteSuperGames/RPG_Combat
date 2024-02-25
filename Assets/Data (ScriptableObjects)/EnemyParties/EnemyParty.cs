using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Party", menuName = "EnemyParty")]
public class EnemyParty : ScriptableObject
{
    public List<CharacterDataSO> enemyPositions = new List<CharacterDataSO>();
    // add things like loot, level ranges...idk
}
