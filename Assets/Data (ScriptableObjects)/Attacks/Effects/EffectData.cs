using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    [Range(0, 100)]
    public float applicationChance;
    public string effectName;

    [TextArea(3, 10)]
    public string effectDescription;

    public abstract Effect CreateEffect();
}
