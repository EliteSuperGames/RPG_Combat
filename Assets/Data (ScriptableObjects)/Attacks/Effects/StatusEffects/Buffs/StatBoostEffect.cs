using System.Linq;

public class StatBoostEffect : StatusEffect
{
    private StatBoostEffectData StatBoostData => (StatBoostEffectData)data;

    public int MaxHealthBoost => StatBoostData.maxHealthBoost;
    public int AttackBoost => StatBoostData.attackBoost;
    public int SpeedBoost => StatBoostData.speedBoost;
    public int MagicBoost => StatBoostData.magicBoost;
    public int MagicDefenseBoost => StatBoostData.magicDefenseBoost;
    public int PhysicalDefenseBoost => StatBoostData.physicalDefenseBoost;

    public StatBoostEffect(StatBoostEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        // RefreshIfNonStackable(target);

        StatBoostEffectData statBoostData = (StatBoostEffectData)data;
        target.CharData.MaxHealth += statBoostData.maxHealthBoost;
        target.CharData.PhysicalPower += statBoostData.attackBoost;
        target.CharData.MagicPower += statBoostData.magicBoost;
        target.CharData.Speed += statBoostData.speedBoost;
        target.CharData.MagicDefense += statBoostData.magicDefenseBoost;
        target.CharData.PhysicalDefense += statBoostData.physicalDefenseBoost;
        target.StatusEffects.Add(this);
        if (!target.StatusEffects.OfType<StatBoostEffect>().Any())
        {
            base.Apply(caster, target);
            // target.StatusEffects.Add(this);
        }
        else
        {
            StatBoostEffect existingEffect = target.StatusEffects.OfType<StatBoostEffect>().FirstOrDefault();
            if (existingEffect.duration < StatBoostData.duration)
            {
                existingEffect.duration = StatBoostData.duration;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        if (duration == 0)
        {
            RemoveEffect();
        }
    }

    public override StatusEffect Clone()
    {
        var clonedEffect = new StatBoostEffect((StatBoostEffectData)StatBoostData.Clone());
        return clonedEffect;
    }

    public void RemoveEffect()
    {
        target.CharData.MaxHealth -= StatBoostData.maxHealthBoost;
        target.CharData.PhysicalPower -= StatBoostData.attackBoost;
        target.CharData.MagicPower -= StatBoostData.magicBoost;
        target.CharData.Speed -= StatBoostData.speedBoost;
        target.CharData.MagicDefense -= StatBoostData.magicDefenseBoost;
        target.CharData.PhysicalDefense -= StatBoostData.physicalDefenseBoost;
    }
}
