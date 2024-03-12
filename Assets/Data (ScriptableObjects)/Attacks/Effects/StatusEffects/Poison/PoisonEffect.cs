using UnityEngine;

public class PoisonEffect : StatusEffect
{
    private PoisonEffectData PoisonData => (PoisonEffectData)data;

    public int DamagePerTurn => PoisonData.damagePerTurn;

    public PoisonEffect(PoisonEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        Debug.Log("Applying poison effect from: " + caster.name + " to: " + target.name);
        base.Apply(caster, target);
        // target.StatusEffects.Add(this);
    }

    public override void Update()
    {
        Debug.Log(data.name + " is updating");
        base.Update();
        // target.TakeDamage(((PoisonEffectData)data).damagePerTurn);
    }
}
