public class HealOverTimeEffect : StatusEffect
{
    private HealOverTimeEffectData HealOverTimeData => (HealOverTimeEffectData)data;

    public int healPerTurn => HealOverTimeData.healPerTurn;

    public HealOverTimeEffect(HealOverTimeEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        if (!target.StatusEffects.Contains(this))
        {
            base.Apply(caster, target);
            // target.StatusEffects.Add(this);
        }
        else
        {
            target.StatusEffects.Find(effect => effect.GetType() == this.GetType()).duration = HealOverTimeData.duration;
        }
    }

    public override StatusEffect Clone()
    {
        var clonedEffect = new HealOverTimeEffect((HealOverTimeEffectData)HealOverTimeData.Clone());
        return clonedEffect;
    }

    public override void Update()
    {
        base.Update();
    }
}
