using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusEffectData : ScriptableObject
{
    [Range(0, 100)]
    public float effectChance;
    public string effectName;
    public int effectDuration;

    /// <summary>
    /// Instant effect application vs persistent effect application (Stun or stat boost vs Poison, which gets applied every turn)
    /// </summary>
    public EffectApplicationType effectApplicationType;

    [Range(0, 1)]
    [SerializeField]
    [Tooltip("The urgency of the effect. Lower = more urgent. Will be applied in order of urgency. HOTS should be 0, DOTS should be 1")]
    public int applicationUrgency = 1;

    public string effectDescription;
    public EffectPolarity effectPolarity;
    public EffectType effectType;
}
