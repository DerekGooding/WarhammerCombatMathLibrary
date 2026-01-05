namespace WarhammerCombatMath.Model;

public sealed record WeaponProfile
{
    public WeaponProfileTags Tags { get; set; } = WeaponProfileTags.None;

    /// <summary>
    /// The number of attacks dice that the weapon gets as part of its attacks stat.
    /// </summary>
    public int NumberOfAttackDice { get; set; }

    /// <summary>
    /// The attack dice type used to determine the variable number of attacks.
    /// </summary>
    public DiceType AttackDiceType { get; set; }

    /// <summary>
    /// The number of flat attacks that the attacker gets.
    /// </summary>
    public int FlatAttacks { get; set; }

    /// <summary>
    /// The ballistic/weapon skill threshold value of the attacker.
    /// </summary>
    public int WeaponSkill { get; set; }

    /// <summary>
    /// The strength of the attacker's weapon.
    /// </summary>
    public int Strength { get; set; }

    /// <summary>
    /// The armor pierce value of the attacker's weapon.
    /// </summary>
    public int ArmorPierce { get; set; }

    /// <summary>
    /// The number of damage dice that the weapon gets as part of its damage stat.
    /// </summary>
    public int NumberOfDamageDice { get; set; }

    /// <summary>
    /// The damage dice type used to determine the variable amount of damage.
    /// </summary>
    public DiceType DamageDiceType { get; set; }

    /// <summary>
    /// The amount of flat damage the weapon deals.
    /// </summary>
    public int FlatDamage { get; set; }

    /// <summary>
    /// The amount of additional hits the weapon gets when Sustained Hits is triggered.
    /// </summary>
    public int SustainedHitsAmount { get; set; }

    /// <summary>
    /// The hit roll result threshold that is considered a critical hit
    /// </summary>
    public int CriticalHitThreshold { get; set; }

    /// <summary>
    /// The wound roll result threshold that is considered a critical wound
    /// </summary>
    public int CriticalWoundThreshold { get; set; }

    /// <summary>
    /// The wound roll result threshold (X+) at which Anti triggers critical wounds
    /// </summary>
    public int WeaponAntiThreshold { get; set; }

    /// <summary>
    /// Hit modifier applied to the attacker's hit rolls. Positive values make it easier to hit, negative values make it harder.
    /// Combined with defender's hit modifier, the total is capped at +/- 1.
    /// </summary>
    public int HitModifier { get; set; }

    /// <summary>
    /// Wound modifier applied to the attacker's wound rolls. Positive values make it easier to wound, negative values make it harder.
    /// Combined with defender's wound modifier, the total is capped at +/- 1.
    /// </summary>
    public int WoundModifier { get; set; }

    /// <inheritdoc/>
    public override string ToString() =>
          $"Weapon Attacks: {(NumberOfAttackDice > 0 ? $"{NumberOfAttackDice} {AttackDiceType} + {FlatAttacks}" : FlatAttacks.ToString())}, "
        + $"WeaponSkill: {WeaponSkill}, "
        + $"WeaponStrength: {Strength}, "
        + $"WeaponArmorPierce: -{ArmorPierce}, "
        + $"WeaponDamage: {(NumberOfDamageDice > 0 ? $"{NumberOfDamageDice} {DamageDiceType} + {FlatDamage}" : FlatDamage)}, "
        + $"WeaponHasTorrent: {Tags.HasFlag(WeaponProfileTags.Torrent)}, "
        + $"WeaponHasLethalHits: {Tags.HasFlag(WeaponProfileTags.LethalHits)}, "
        + $"WeaponHasSustainedHits: {Tags.HasFlag(WeaponProfileTags.SustainedHits)}, "
        + $"WeaponSustainedHitsMultiplier: {SustainedHitsAmount}, "
        + $"WeaponHasRerollHitRolls: {Tags.HasFlag(WeaponProfileTags.RerollHits)}, "
        + $"WeaponHasRerollHitRollsOf1: {Tags.HasFlag(WeaponProfileTags.RerollHits1)}, "
        + $"WeaponHasDevastatingWounds: {Tags.HasFlag(WeaponProfileTags.DevestatingWounds)}, "
        + $"WeaponHasRerollWoundRolls: {Tags.HasFlag(WeaponProfileTags.RerollWounds)}, "
        + $"WeaponHasRerollWoundRollsOf1: {Tags.HasFlag(WeaponProfileTags.RerollWounds1)}, "
        + $"WeaponHasRerollDamageRolls: {Tags.HasFlag(WeaponProfileTags.RerollDamage)}, "
        + $"WeaponHasRerollDamageRollsOf1: {Tags.HasFlag(WeaponProfileTags.RerollDamage1)}, "
        + $"CriticalHitThreshold: {CriticalHitThreshold}, "
        + $"CriticalWoundThreshold: {CriticalWoundThreshold}, "
        + $"WeaponHasAnti: {Tags.HasFlag(WeaponProfileTags.Anti)}, "
        + $"WeaponAntiThreshold: {WeaponAntiThreshold}, "
        + $"HitModifier: {HitModifier}, "
        + $"WoundModifier: {WoundModifier} ";
}
