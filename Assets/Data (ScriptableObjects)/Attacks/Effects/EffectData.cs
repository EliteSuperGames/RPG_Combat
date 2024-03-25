using UnityEngine;

[System.Serializable]
public abstract class EffectData : ScriptableObject
{
    [SerializeField]
    [Range(0, 100)]
    public float applicationChance;

    [SerializeField]
    public string effectName;

    [SerializeField]
    [TextArea(3, 10)]
    public string effectDescription;

    [SerializeField]
    public abstract Effect CreateEffect();
}
