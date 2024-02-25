using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;

    [Space(15)]
    public TargetFaction targetFaction;

    [Space(15)]
    [Range(0, 3)]
    public List<int> launchPositions = new List<int>();

    [Tooltip("Single can target any of the listed landing positions, Multiple will target all landing positions")]
    [Space(15)]
    public TargetingType targetingType;

    [Space(15)]
    [Range(0, 3)]
    public List<int> landingPositions = new List<int>();

    [Space(15)]
    public List<StatusEffectData> effects = new List<StatusEffectData>();

    [Space(15)]
    public string abilityDescription;

    [Space(15)]
    [Range(0, 10)]
    public int cooldownLength;

    [Space(15)]
    public int baseAbilityPower;

    /// <summary>
    /// Not sure this is needed anymore
    /// </summary>
    [Space(15)]
    public List<AbilityType> abilityTypes = new List<AbilityType>();

    [Space(15)]
    public bool onlyTargetUnconscious = false;

    [Space(15)]
    public bool canOnlyTargetSelf = false;
}
