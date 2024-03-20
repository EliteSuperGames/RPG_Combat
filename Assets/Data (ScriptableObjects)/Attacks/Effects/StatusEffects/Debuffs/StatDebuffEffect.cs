using System.Linq;

public class StatDebuffEffect : StatusEffect
{
    private StatDebuffEffectData StatDebuffData => (StatDebuffEffectData)data;

    public int MaxHealthDebuff => StatDebuffData.maxHealthDebuff;
    public int AttackDebuff => StatDebuffData.attackDebuff;
    public int SpeedDebuff => StatDebuffData.speedDebuff;
    public int MagicDebuff => StatDebuffData.magicDebuff;
    public int MagicDefenseDebuff => StatDebuffData.magicDefenseDebuff;
    public int PhysicalDefenseDebuff => StatDebuffData.physicalDefenseDebuff;

    public StatDebuffEffect(StatDebuffEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        // RefreshIfNonStackable(target);

        StatDebuffEffectData StatDebuffData = (StatDebuffEffectData)data;
        target.CharData.MaxHealth -= StatDebuffData.maxHealthDebuff;
        target.CharData.PhysicalPower -= StatDebuffData.attackDebuff;
        target.CharData.MagicPower -= StatDebuffData.magicDebuff;
        target.CharData.Speed -= StatDebuffData.speedDebuff;
        target.CharData.MagicDefense -= StatDebuffData.magicDefenseDebuff;
        target.CharData.PhysicalDefense -= StatDebuffData.physicalDefenseDebuff;

        // Only add the new effect if it's not already present
        if (!target.StatusEffects.OfType<StatDebuffEffect>().Any())
        {
            // target.StatusEffects.Add(this);
            base.Apply(caster, target);
        }
        else
        {
            StatDebuffEffect existingEffect = target.StatusEffects.OfType<StatDebuffEffect>().FirstOrDefault();
            if (existingEffect.duration < StatDebuffData.duration)
            {
                existingEffect.duration = StatDebuffData.duration;
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
        var clonedEffect = new StatDebuffEffect((StatDebuffEffectData)StatDebuffData.Clone());
        return clonedEffect;
    }

    public void RemoveEffect()
    {
        target.CharData.MaxHealth += StatDebuffData.maxHealthDebuff;
        target.CharData.PhysicalPower += StatDebuffData.attackDebuff;
        target.CharData.MagicPower += StatDebuffData.magicDebuff;
        target.CharData.Speed += StatDebuffData.speedDebuff;
        target.CharData.MagicDefense += StatDebuffData.magicDefenseDebuff;
        target.CharData.PhysicalDefense += StatDebuffData.physicalDefenseDebuff;
    }
}
