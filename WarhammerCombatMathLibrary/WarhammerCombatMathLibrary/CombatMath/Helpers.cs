using System.Diagnostics;
using WarhammerCombatMathLibrary.Data;

namespace WarhammerCombatMathLibrary.CombatMath;

internal static class Helpers
{
    internal static int GetCombinedHitModifier(AttackerDTO? attacker, DefenderDTO? defender)
    {
        var attackerModifier = attacker?.HitModifier ?? 0;
        var defenderModifier = defender?.HitModifier ?? 0;
        var combinedModifier = attackerModifier + defenderModifier;

        return Math.Clamp(combinedModifier, -1, 1);
    }

    internal static int GetCombinedWoundModifier(AttackerDTO? attacker, DefenderDTO? defender)
    {
        var attackerModifier = attacker?.WoundModifier ?? 0;
        var defenderModifier = defender?.WoundModifier ?? 0;
        var combinedModifier = attackerModifier + defenderModifier;

        return Math.Clamp(combinedModifier, -1, 1);
    }

    internal static int GetNumberOfSuccessfulResults(int successThreshold)
    {
        if (successThreshold > POSSIBLE_RESULTS_SIX_SIDED_DIE)
        {
            Debug.WriteLine($"GetNumberOfSuccessfulResults() | Success threshold is greater than {POSSIBLE_RESULTS_SIX_SIDED_DIE}, returning 0 ...");
            return 0;
        }

        if (successThreshold <= 0)
        {
            Debug.WriteLine("GetNumberOfSuccessfulResults() | Success threshold is less than or equal to 0, returning 0 ...");
            return 0;
        }

        return Math.Min(POSSIBLE_RESULTS_SIX_SIDED_DIE - (successThreshold - 1), POSSIBLE_RESULTS_SIX_SIDED_DIE - 1);
    }

    internal static int GetAverageAttacks(AttackerDTO attacker)
    {
        if (attacker.NumberOfModels < 1)
        {
            Debug.WriteLine("GetAverageAttacks() | Number of models is less than 1, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfAttackDice <= 0 && attacker.WeaponFlatAttacks <= 0)
        {
            Debug.WriteLine("GetAverageAttacks() | Attacker has no attacks value, returning 0 ...");
            return 0;
        }

        var averageAttackDieResult = Statistics.GetMeanResult((int)attacker.WeaponAttackDiceType);
        var averageVariableAttacksPerModel = attacker.WeaponNumberOfAttackDice * averageAttackDieResult;
        var totalAverageAttacksPerModel = averageVariableAttacksPerModel + attacker.WeaponFlatAttacks;
        return totalAverageAttacksPerModel * attacker.NumberOfModels;
    }

    internal static int GetMinimumAttacks(AttackerDTO attacker)
    {
        if (attacker.NumberOfModels < 1)
        {
            Debug.WriteLine("GetMinimumAttacks() | Number of models is less than 1, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfAttackDice <= 0 && attacker.WeaponFlatAttacks <= 0)
        {
            Debug.WriteLine("GetMinimumAttacks() | Attacker has no attacks value, returning 0 ...");
            return 0;
        }

        var minimumAttacksPerModel = attacker.WeaponNumberOfAttackDice + attacker.WeaponFlatAttacks;
        return minimumAttacksPerModel * attacker.NumberOfModels;
    }

    internal static int GetMaximumAttacks(AttackerDTO attacker)
    {
        if (attacker.NumberOfModels < 1)
        {
            Debug.WriteLine("GetMaximumAttacks() | Number of models is less than 1, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfAttackDice <= 0 && attacker.WeaponFlatAttacks <= 0)
        {
            Debug.WriteLine("GetMaximumAttacks() | Attacker has no attacks value, returning 0 ...");
            return 0;
        }

        var maximumAttackRollValue = (int)attacker.WeaponAttackDiceType;
        var maximumVariableAttacksPerModel = (attacker.WeaponNumberOfAttackDice * maximumAttackRollValue);
        var totalMaximumAttacksPerModel = maximumVariableAttacksPerModel + attacker.WeaponFlatAttacks;
        return totalMaximumAttacksPerModel * attacker.NumberOfModels;
    }

    internal static double GetBaseProbabilityOfHit(AttackerDTO attacker, int hitModifier)
    {
        if (attacker.WeaponSkill <= 0)
        {
            Debug.WriteLine("GetBaseProbabilityOfHit() | Attacker weapon skill is less than or equal to 0, returning 0 ...");
            return 0;
        }

        var adjustedWeaponSkill = attacker.WeaponSkill - hitModifier;
        var hitSuccessThreshold = adjustedWeaponSkill == AUTOMATIC_FAIL_RESULT ? AUTOMATIC_FAIL_RESULT + 1 : adjustedWeaponSkill;
        return Statistics.GetProbabilityOfSuccess(POSSIBLE_RESULTS_SIX_SIDED_DIE, GetNumberOfSuccessfulResults(hitSuccessThreshold));
    }

    internal static double GetProbabilityOfCriticalHit(AttackerDTO attacker)
    {
        if (attacker.CriticalHitThreshold is <= 1 or >= 7)
        {
            return Statistics.GetProbabilityOfSuccess(POSSIBLE_RESULTS_SIX_SIDED_DIE, 1);
        }

        var adjustedCriticalHitThreshold = attacker.CriticalHitThreshold == AUTOMATIC_FAIL_RESULT ? AUTOMATIC_FAIL_RESULT + 1 : attacker.CriticalHitThreshold;
        return Statistics.GetProbabilityOfSuccess(POSSIBLE_RESULTS_SIX_SIDED_DIE, GetNumberOfSuccessfulResults(adjustedCriticalHitThreshold));
    }
    internal static double GetHitModifier_RerollHits(double probabilityOfHit) => (1 - probabilityOfHit) * probabilityOfHit;

    internal static double GetHitModifier_RerollHitsOf1(double probabilityOfHit) => (1.0 / POSSIBLE_RESULTS_SIX_SIDED_DIE) * probabilityOfHit;

    internal static double GetBaseProbabilityOfWound(AttackerDTO attacker, DefenderDTO defender)
    {
        var combinedWoundModifier = GetCombinedWoundModifier(attacker, defender);
        var normalWoundThreshold = GetSuccessThresholdOfWound(attacker.WeaponStrength, defender.Toughness);
        var adjustedNormalThreshold = normalWoundThreshold - combinedWoundModifier;

        int finalWoundThreshold;

        if (attacker.WeaponHasAnti && IsValidThreshold(attacker.WeaponAntiThreshold))
        {
            finalWoundThreshold = Math.Min(adjustedNormalThreshold, attacker.WeaponAntiThreshold);
        }
        else
        {
            finalWoundThreshold = adjustedNormalThreshold;
        }

        var validatedThreshold = finalWoundThreshold == AUTOMATIC_FAIL_RESULT ? AUTOMATIC_FAIL_RESULT + 1 : finalWoundThreshold;
        return Statistics.GetProbabilityOfSuccess(POSSIBLE_RESULTS_SIX_SIDED_DIE, GetNumberOfSuccessfulResults(validatedThreshold));
    }

    internal static bool IsValidThreshold(int threshold) => threshold is >= 2 and <= 6;

    internal static double GetProbabilityOfCriticalWound(AttackerDTO attacker)
    {
        var hasValidAnti = attacker.WeaponHasAnti && IsValidThreshold(attacker.WeaponAntiThreshold);
        var hasValidCriticalWound = IsValidThreshold(attacker.CriticalWoundThreshold);

        int effectiveCriticalWoundThreshold;
        if (hasValidAnti && hasValidCriticalWound)
        {
            effectiveCriticalWoundThreshold = Math.Min(attacker.CriticalWoundThreshold, attacker.WeaponAntiThreshold);
        }
        else if (hasValidAnti)
        {
            effectiveCriticalWoundThreshold = attacker.WeaponAntiThreshold;
        }
        else if (hasValidCriticalWound)
        {
            effectiveCriticalWoundThreshold = attacker.CriticalWoundThreshold;
        }
        else
        {
            effectiveCriticalWoundThreshold = 6;
        }

        var adjustedCriticalWoundThreshold = effectiveCriticalWoundThreshold == AUTOMATIC_FAIL_RESULT ? AUTOMATIC_FAIL_RESULT + 1 : effectiveCriticalWoundThreshold;
        return Statistics.GetProbabilityOfSuccess(POSSIBLE_RESULTS_SIX_SIDED_DIE, GetNumberOfSuccessfulResults(adjustedCriticalWoundThreshold));
    }

    internal static double GetWoundModifier_LethalHits(double probabilityOfCriticalHit) => probabilityOfCriticalHit;

    internal static double GetWoundModifier_SustainedHits(
        double probabilityOfCriticalHit,
        int sustainedHitsMultiplier,
        double probabilityOfWound)
        => sustainedHitsMultiplier * probabilityOfCriticalHit * probabilityOfWound;

    internal static double GetWoundModifier_RerollWounds(double probabilityOfHit, double probabilityOfWound)
        => probabilityOfHit * (1 - probabilityOfWound) * probabilityOfWound;

    internal static double GetWoundModifier_RerollWoundsOf1(double probabilityOfHit, double probabilityOfWound)
        => probabilityOfHit * (1.0 / POSSIBLE_RESULTS_SIX_SIDED_DIE) * probabilityOfWound;

    internal static double GetBaseProbabilityOfFailedSave(AttackerDTO attacker, DefenderDTO defender)
    {
        var adjustedArmorSaveThreshold = GetAdjustedArmorSaveThreshold(attacker, defender);
        var probabilityOfSuccessfulSave = Statistics.GetProbabilityOfSuccess(POSSIBLE_RESULTS_SIX_SIDED_DIE, GetNumberOfSuccessfulResults(adjustedArmorSaveThreshold));
        return (double)(1 - probabilityOfSuccessfulSave);
    }

    internal static double GetFailedSaveModifier_DevastatingWounds(double probabilityOfHit, double probabilityOfCriticalWound) => probabilityOfHit * probabilityOfCriticalWound;

    internal static int GetAverageDamagePerAttack(AttackerDTO? attacker)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetAverageDamagePerAttack() | Attacker is null, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfAttackDice <= 0 && attacker.WeaponFlatAttacks <= 0)
        {
            Debug.WriteLine("GetAverageDamagePerAttack() | Attacker has no attacks value, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfDamageDice <= 0 && attacker.WeaponFlatDamage <= 0)
        {
            Debug.WriteLine("GetAverageDamagePerAttack() | Attacker has no damage value, returning 0 ...");
            return 0;
        }

        var numberOfDamageDieRolls = attacker.WeaponNumberOfDamageDice;
        var averageDamagePerDieRoll = Statistics.GetMeanResult((int)attacker.WeaponDamageDiceType);
        var flatDamage = attacker.WeaponFlatDamage;
        return (numberOfDamageDieRolls * averageDamagePerDieRoll) + flatDamage;
    }

    internal static int GetMinimumDamagePerAttack(AttackerDTO? attacker)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetMinimumDamagePerAttack() | Attacker is null, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfAttackDice <= 0 && attacker.WeaponFlatAttacks <= 0)
        {
            Debug.WriteLine("GetMinimumDamagePerAttack() | Attacker has no attacks value, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfDamageDice <= 0 && attacker.WeaponFlatDamage <= 0)
        {
            Debug.WriteLine("GetMinimumDamagePerAttack() | Attacker has no damage value, returning 0 ...");
            return 0;
        }

        var numberOfDamageDieRolls = attacker.WeaponNumberOfDamageDice;
        const int minimumDamagePerDieRoll = 1;
        var flatDamage = attacker.WeaponFlatDamage;
        return (numberOfDamageDieRolls * minimumDamagePerDieRoll) + flatDamage;
    }

    internal static int GetMaximumDamagePerAttack(AttackerDTO? attacker)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetMaximumDamagePerAttack() | Attacker is null, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfAttackDice <= 0 && attacker.WeaponFlatAttacks <= 0)
        {
            Debug.WriteLine("GetMaximumDamagePerAttack() | Attacker has no attacks value, returning 0 ...");
            return 0;
        }

        if (attacker.WeaponNumberOfDamageDice <= 0 && attacker.WeaponFlatDamage <= 0)
        {
            Debug.WriteLine("GetMaximumDamagePerAttack() | Attacker has no damage value, returning 0 ...");
            return 0;
        }

        var numberOfDamageDieRolls = attacker.WeaponNumberOfDamageDice;
        var maximumDamagePerDieRoll = (int)attacker.WeaponDamageDiceType;
        var flatDamage = attacker.WeaponFlatDamage;
        return (numberOfDamageDieRolls * maximumDamagePerDieRoll) + flatDamage;
    }

    internal static double GetAverageAdjustedDamagePerAttack(int damagePerAttack, DefenderDTO? defender)
    {
        if (damagePerAttack <= 0)
        {
            Debug.WriteLine("GetAverageAdjustedDamagePerAttack() | Input damage is less than or equal to 0. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetAverageAdjustedDamagePerAttack() | Defender is null. Returning 0 ...");
            return 0;
        }

        var damageAfterReduction = Math.Max(0, damagePerAttack - defender.DamageReduction);

        var feelNoPainSuccessProbability = Statistics.GetProbabilityOfSuccess(POSSIBLE_RESULTS_SIX_SIDED_DIE, GetNumberOfSuccessfulResults(defender.FeelNoPain));
        return damageAfterReduction * (1 - feelNoPainSuccessProbability);
    }

    internal static int GetMinimumAdjustedDamagePerAttack(int damagePerAttack, DefenderDTO? defender)
    {
        if (damagePerAttack <= 0)
        {
            Debug.WriteLine("GetMinimumAdjustedDamagePerAttack() | Input damage is less than or equal to 0. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetMinimumAdjustedDamagePerAttack() | Defender is null. Returning 0 ...");
            return 0;
        }

        var damageAfterReduction = Math.Max(0, damagePerAttack - defender.DamageReduction);

        if (defender.FeelNoPain is >= 2 and <= 6)
        {
            Debug.WriteLine("GetMinimumAdjustedDamagePerAttack() | Defender has feel no pains. Returning 0 ...");
            return 0;
        }

        return damageAfterReduction;
    }

    internal static int GetMaximumAdjustedDamagePerAttack(int damagePerAttack, DefenderDTO? defender)
    {
        if (damagePerAttack <= 0)
        {
            Debug.WriteLine("GetMaximumAdjustedDamagePerAttack() | Input damage is less than or equal to 0. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetMaximumAdjustedDamagePerAttack() | Defender is null. Returning 0 ...");
            return 0;
        }

        return Math.Max(0, damagePerAttack - defender.DamageReduction);
    }

    internal static double GetModelsDestroyed(double numberOfAttacks, double damagePerAttack, DefenderDTO? defender)
    {
        if (numberOfAttacks <= 0)
        {
            Debug.WriteLine("GetModelsDestroyed() | Number of attacks is less than or equal to 0. Returning 0 ...");
            return 0;
        }

        if (damagePerAttack <= 0)
        {
            Debug.WriteLine("GetModelsDestroyed() | Attacker's weapon damage is less than or equal to 0. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetModelsDestroyed() | Defender is null. Returning 0 ...");
            return 0;
        }

        var totalDamage = damagePerAttack * numberOfAttacks;
        var damageThreshold = Math.Max(defender.Wounds, damagePerAttack);
        var modelsDestroyed = totalDamage / damageThreshold;
        return Math.Min(modelsDestroyed, defender.NumberOfModels);
    }

    internal static int GetAttacksRequiredToDestroyOneModel(int damagePerAttack, DefenderDTO? defender)
    {
        if (defender == null)
        {
            Debug.WriteLine("GetAttacksRequiredToDestroyOneModel() | Defender is null. Returning 0 ...");
            return 0;
        }

        if (damagePerAttack <= 0)
        {
            Debug.WriteLine("GetAttacksRequiredToDestroyOneModel() | Attacker weapon damage is less than or equal to 0. Returning 0 ...");
            return 0;
        }

        return (int)Math.Ceiling((double)defender.Wounds / damagePerAttack);
    }

}
