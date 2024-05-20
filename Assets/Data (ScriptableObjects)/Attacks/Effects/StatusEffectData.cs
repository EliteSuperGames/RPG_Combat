using UnityEngine;

public abstract class StatusEffectData : EffectData
{
    [SerializeField]
    public int duration;

    [SerializeField]
    public bool isStackable;

    [SerializeField]
    public bool isRenewable;

    [SerializeField]
    public float dispelResistance;

    [SerializeField]
    public EffectType effectType;

    public abstract StatusEffectData Clone();
}
