using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Effects/MoveTargetEffectData")]
public class MoveTargetEffectData : StatusEffectData
{
    public int moveDistance;
    public MoveDirection moveDirection;
}
