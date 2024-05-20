using UnityEngine;

public class CureEffect : Effect
{
    private CureEffectData CureData => (CureEffectData)data;

    public CureEffect(CureEffectData data)
        : base(data) { }

    public override void Apply(BattleCharacter caster, BattleCharacter target)
    {
        Debug.Log("CureEffect Apply");
        CureEffectData cureData = (CureEffectData)data;
        for (int i = 0; i < cureData.TypesOfEffectsCured.Count; i++)
        {
            Debug.Log(cureData.TypesOfEffectsCured[i]);
            for (int j = target.StatusEffects.Count - 1; j >= 0; j--)
            {
                Debug.Log(target.StatusEffects[j].data.name);
                Debug.Log("Curing " + target.StatusEffects[j].data.name + " from " + target.CharData.CharacterName + " with " + cureData.name);
                StatusEffectData statusEffectData = (StatusEffectData)target.StatusEffects[j].data;
                if (cureData.TypesOfEffectsCured.Contains(statusEffectData.effectType))
                {
                    target.StatusEffects.RemoveAt(j);
                }
            }
        }
    }
}
