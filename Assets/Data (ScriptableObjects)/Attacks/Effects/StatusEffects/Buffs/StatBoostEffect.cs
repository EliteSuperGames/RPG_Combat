using System.Linq;

public class StatBoostEffect : StatusEffect
{
    private StatBoostEffectData StatBoostData => (StatBoostEffectData)data;

    public int MaxHealthBoost => StatBoostData.maxHealthBoost;
    public int AttackBoost => StatBoostData.attackBoost;
    public int SpeedBoost => StatBoostData.speedBoost;
    public int MagicBoost => StatBoostData.magicBoost;

    public StatBoostEffect(StatBoostEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        RefreshIfNonStackable(target);

        StatBoostEffectData statBoostData = (StatBoostEffectData)data;
        target.CharData.MaxHealth += statBoostData.maxHealthBoost;
        target.CharData.PhysicalPower += statBoostData.attackBoost;
        target.CharData.MagicPower += statBoostData.magicBoost;
        target.CharData.Speed += statBoostData.speedBoost;
        target.StatusEffects.Add(this);
        if (!target.StatusEffects.OfType<StatBoostEffect>().Any())
        {
            target.StatusEffects.Add(this);
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

    public void RemoveEffect()
    {
        target.CharData.MaxHealth -= StatBoostData.maxHealthBoost;
        target.CharData.PhysicalPower -= StatBoostData.attackBoost;
        target.CharData.MagicPower -= StatBoostData.magicBoost;
        target.CharData.Speed -= StatBoostData.speedBoost;
    }
}
