public class EligibleAbility
{
    public Ability Ability { get; private set; }
    public bool EligibleBasedOnLaunchPosition { get; set; }
    public bool EligibleBasedOnLandingTargets { get; set; }

    public EligibleAbility(Ability ability)
    {
        Ability = ability;
        // EligibleBasedOnLaunchPosition = false;
    }
}
