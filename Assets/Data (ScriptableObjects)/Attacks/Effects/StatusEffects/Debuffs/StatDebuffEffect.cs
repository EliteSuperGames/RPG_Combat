using System.Linq;

public class StatDebuffEffect : StatusEffect
{
    private StatDebuffEffectData StatDebuffData => (StatDebuffEffectData)data;

    public int MaxHealthDebuff => StatDebuffData.maxHealthDebuff;
    public int AttackDebuff => StatDebuffData.attackDebuff;
    public int SpeedDebuff => StatDebuffData.speedDebuff;
    public int MagicDebuff => StatDebuffData.magicDebuff;

    public StatDebuffEffect(StatDebuffEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        RefreshIfNonStackable(target);

        StatDebuffEffectData StatDebuffData = (StatDebuffEffectData)data;
        target.CharData.MaxHealth -= StatDebuffData.maxHealthDebuff;
        target.CharData.PhysicalPower -= StatDebuffData.attackDebuff;
        target.CharData.MagicPower -= StatDebuffData.magicDebuff;
        target.CharData.Speed -= StatDebuffData.speedDebuff;

        // Only add the new effect if it's not already present
        if (!target.StatusEffects.OfType<StatDebuffEffect>().Any())
        {
            target.StatusEffects.Add(this);
        }
    }

    public override void Update()
    {
        base.Update();
        if (duration == 0)
        {
            StatDebuffEffectData StatDebuffData = (StatDebuffEffectData)data;
            target.CharData.MaxHealth += StatDebuffData.maxHealthDebuff;
            target.CharData.PhysicalPower += StatDebuffData.attackDebuff;
            target.CharData.MagicPower += StatDebuffData.magicDebuff;
            target.CharData.Speed += StatDebuffData.speedDebuff;
        }
    }
}
