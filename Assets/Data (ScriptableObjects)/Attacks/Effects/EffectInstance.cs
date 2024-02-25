using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectInstance
{
    public StatusEffectData effectData;
    public int remainingDuration;
    public string characterAppliedTo;
    public string characterApplying;

    public EffectInstance() { }
}
