[System.Serializable]
public class StatModifier
{
    public string modifierName = "";
    public int magicPowerMod = 0;
    public int physicalPowerMod = 0;
    public int speedMod = 0;
    public int maxHealthMod = 0;
    public PositiveOrNegativeMod positiveOrNegative;

    public StatModifier() { }

    public override string ToString()
    {
        return $"Modifier Name: {modifierName}, "
            + $"Magic Power Mod: {magicPowerMod}, "
            + $"Physical Power Mod: {physicalPowerMod}, "
            + $"Speed Mod: {speedMod}, "
            + $"Max Health Mod: {maxHealthMod}, "
            + $"Positive or Negative: {positiveOrNegative}";
    }
}

[System.Serializable]
public enum PositiveOrNegativeMod
{
    Positive,
    Negative
}
