namespace WarhammerCombatMath.Model;

[Flags]
public enum WeaponProfileTags
{
    None = 0,
    Torrent = 1 << 0,
    LethalHits = 1 << 1,
    SustainedHits = 1 << 2,
    DevestatingWounds = 1 << 3,
    Anti = 1 << 4,
    RerollHits = 1 << 5,
    RerollHits1 = 1 << 6,
    RerollWounds = 1 << 7,
    RerollWounds1 = 1 << 8,
    RerollDamage = 1 << 9,
    RerollDamage1 = 1 << 10,
}