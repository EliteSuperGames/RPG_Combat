public class HealEffect : Effect
{
    private HealEffectData HealData => (HealEffectData)data;

    public int HealingPower => HealData.healingPower;

    public HealEffect(HealEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        HealEffectData healData = (HealEffectData)data;
        target.RestoreHealth(healData.healingPower);
    }
}
