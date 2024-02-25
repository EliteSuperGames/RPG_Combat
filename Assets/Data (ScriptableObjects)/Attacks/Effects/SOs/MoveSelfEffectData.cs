using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Effects/MoveSelfEffectData")]
public class MoveSelfEffectData : StatusEffectData
{
    public int moveDistance;
    public MoveDirection moveDirection;
}

public enum MoveDirection
{
    Forward,
    Backward
}