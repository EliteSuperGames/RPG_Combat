[System.Serializable]
public enum EffectApplicationType
{
    /// <summary>
    /// Instantaneous effects are applied immediately and then removed. Things like Stun, Movement
    /// </summary>
    Instantaneous_Single_Application,

    /// <summary>
    /// Instantaneous effects are applied immediately and persist until they are cured or their duration expires. Things like Buffs and Debuffs.
    /// </summary>
    Instantaneous_Persistent,

    /// <summary>
    /// Persistent effects are applied and remain on the character until the duration expires or they are removed. Things like Damage over Time, Healing over Time.
    /// Some may have their effect applied at the start of the round, some at the end of the round. Most would be the end of the round.
    /// </summary>
    Persistent_Applied_End_Of_Round,
    Persistent_Applied_Start_Of_Round,
}
