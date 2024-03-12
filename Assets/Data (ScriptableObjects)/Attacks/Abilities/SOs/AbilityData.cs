using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Attacks/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;

    [Space(15)]
    public string abilityDescription;

    [Space(15)]
    public TargetingType targetingType;

    [Space(15)]
    public TargetFaction targetFaction;

    [Space(15)]
    [Range(0, 3)]
    public List<int> launchPositions = new List<int>();

    [Space(15)]
    [Range(0, 3)]
    public List<int> landingPositions = new List<int>();

    [Space(15)]
    [Range(0, 10)]
    public int cooldownLength;

    [Space(15)]
    public int baseAbilityPower;

    [Space(15)]
    public bool onlyTargetUnconscious = false;

    [Space(15)]
    public bool canOnlyTargetSelf = false;

    [Space(15)]
    public List<EffectData> targetEffects = new List<EffectData>();

    [Space(15)]
    public List<EffectData> casterEffects = new List<EffectData>();
}
