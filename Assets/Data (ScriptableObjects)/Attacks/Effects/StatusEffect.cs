using UnityEngine;

[System.Serializable]
public class StatusEffect : Effect
{
    [SerializeField]
    public int duration;

    [SerializeField]
    protected BattleCharacter target;

    public StatusEffect(StatusEffectData data)
        : base(data)
    {
        this.data = data;
        duration = data.duration;
    }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        this.target = target;
        target.StatusEffects.Add(this);
    }

    public virtual StatusEffect Clone()
    {
        var clonedData = ((StatusEffectData)data).Clone();
        var clonedEffect = new StatusEffect(clonedData) { duration = duration };
        return clonedEffect;
    }

    public virtual void Update()
    {
        duration--;
        if (duration < 0)
        {
            duration = 0;
        }
    }

    public virtual void RefreshIfNonStackable(BattleCharacter target)
    {
        if (!((StatusEffectData)data).isStackable)
        {
            foreach (StatusEffect effect in target.StatusEffects)
            {
                if (effect.GetType() == this.GetType())
                {
                    // target already has this status effect
                    if (this.duration > effect.duration)
                    {
                        effect.duration = this.duration;
                    }
                }
                return;
            }
        }
    }
}
