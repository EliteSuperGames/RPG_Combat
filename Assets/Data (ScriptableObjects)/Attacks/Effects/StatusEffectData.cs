using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : EffectData
{
    public int duration;
    public bool isStackable;
    public bool isRenewable;
    public float dispelResistance;
}
