using UnityEngine;

public class FireEffect : StatusEffect
{
    private FireEffectData FireData => (FireEffectData)data;

    public int DamagePerTurn => FireData.damagePerTurn;

    public FireEffect(FireEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        FireEffectData fireData = (FireEffectData)data;
        base.Apply(caster, target);
        Debug.Log(fireData.damagePerTurn);
    }

    public override void Update()
    {
        Debug.Log(data.name + " is updating");
        base.Update();
    }

    public override StatusEffect Clone()
    {
        var clonedEffect = new FireEffect((FireEffectData)FireData.Clone());
        return clonedEffect;
    }
}
