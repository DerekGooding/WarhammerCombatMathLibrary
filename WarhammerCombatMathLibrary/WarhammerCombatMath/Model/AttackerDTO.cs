namespace WarhammerCombatMath.Model;

/// <summary>
/// A data transfer object representing the attacker in a combat scenario.
/// </summary>
public sealed record AttackerDTO(int NumberOfModels, List<WeaponProfile> Profiles)
{
    internal bool WeaponHasTorrent => Profiles[0].Tags.HasFlag(WeaponProfileTags.Torrent);
    internal bool WeaponHasRerollHitRolls => Profiles[0].Tags.HasFlag(WeaponProfileTags.RerollHits);
    internal bool WeaponHasRerollHitRollsOf1 => Profiles[0].Tags.HasFlag(WeaponProfileTags.RerollHits1);
    internal bool WeaponHasRerollWoundRolls => Profiles[0].Tags.HasFlag(WeaponProfileTags.RerollWounds);
    internal bool WeaponHasRerollWoundRollsOf1 => Profiles[0].Tags.HasFlag(WeaponProfileTags.RerollWounds1);
    internal bool WeaponHasLethalHits => Profiles[0].Tags.HasFlag(WeaponProfileTags.LethalHits);
    internal bool WeaponHasSustainedHits => Profiles[0].Tags.HasFlag(WeaponProfileTags.SustainedHits);
    internal bool WeaponHasDevastatingWounds => Profiles[0].Tags.HasFlag(WeaponProfileTags.DevestatingWounds);
    internal bool WeaponHasAnti => Profiles[0].Tags.HasFlag(WeaponProfileTags.Anti);
    internal int WeaponNumberOfAttackDice => Profiles[0].NumberOfAttackDice;
    internal int HitModifier => Profiles[0].HitModifier;
    internal int WoundModifier => Profiles[0].WoundModifier;
    internal int WeaponFlatAttacks => Profiles[0].FlatAttacks;
    internal int WeaponSkill => Profiles[0].WeaponSkill;
    internal int WeaponStrength => Profiles[0].Strength;
    internal int CriticalHitThreshold => Profiles[0].CriticalHitThreshold;
    internal int CriticalWoundThreshold => Profiles[0].CriticalWoundThreshold;
    internal int WeaponAntiThreshold => Profiles[0].WeaponAntiThreshold;
    internal int WeaponNumberOfDamageDice => Profiles[0].NumberOfDamageDice;
    internal int WeaponFlatDamage => Profiles[0].FlatDamage;
    internal int WeaponSustainedHitsMultiplier => Profiles[0].SustainedHitsAmount;
    internal int WeaponArmorPierce => Profiles[0].ArmorPierce;


    internal DiceType WeaponDamageDiceType => Profiles[0].DamageDiceType;
    internal DiceType WeaponAttackDiceType => Profiles[0].AttackDiceType;

    /// <inheritdoc/>
    public override string ToString() =>
        $"Attacker: [ NumberOfModels: {NumberOfModels}, Profiles: {string.Join(", ", Profiles)} ]";
}
