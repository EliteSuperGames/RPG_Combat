using UnityEngine;

public class PoisonEffect : StatusEffect
{
    [SerializeField]
    private PoisonEffectData PoisonData => (PoisonEffectData)data;

    [SerializeField]
    public int DamagePerTurn => PoisonData.damagePerTurn;

    public PoisonEffect(PoisonEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        base.Apply(caster, target);
    }

    public override StatusEffect Clone()
    {
        var clonedEffect = new PoisonEffect((PoisonEffectData)PoisonData.Clone());
        return clonedEffect;
    }

    public override void Update()
    {
        base.Update();
    }
}
