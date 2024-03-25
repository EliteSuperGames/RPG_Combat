using UnityEngine;

[System.Serializable]
public abstract class Effect
{
    [SerializeField]
    public EffectData data;

    public Effect(EffectData data)
    {
        this.data = data;
    }

    public abstract void Apply(BattleCharacter caster, BattleCharacter target);
}
