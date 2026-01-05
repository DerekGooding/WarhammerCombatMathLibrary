using System.Diagnostics;
using WarhammerCombatMathLibrary.Data;

namespace WarhammerCombatMathLibrary.CombatMath;

/// <summary>
/// Represents a binomial distribution of trials and successes.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Returns the attacker's probability of succeeding any single hit roll.
    /// Includes modifiers for any abilities that affect hit rolls.
    /// </summary>
    /// <param name="attacker">The attacker data object</param>
    /// <param name="defender">The defender data object (optional, used for hit modifiers)</param>
    /// <returns>A double value containing the probability of succeeding on any single hit roll.</returns>
    public static double GetProbabilityOfHit(AttackerDTO? attacker, DefenderDTO? defender = null)
    {
        // Validate inputs
        if (attacker == null)
        {
            Debug.WriteLine("GetProbabilityOfHit() | Attacker is null, returning 0 ...");
            return 0;
        }

        // If weapon has torrent, no other modifiers are added
        if (attacker.WeaponHasTorrent)
        {
            return 1;
        }

        // Calculate combined hit modifier
        var combinedHitModifier = GetCombinedHitModifier(attacker, defender);

        // Calculate base hit probability with hit modifier
        var baseHitProbability = GetBaseProbabilityOfHit(attacker, combinedHitModifier);

        // Calculate modifiers
        double totalHitModifiers = 0;

        // Full rerolls overrides rerolls of 1
        if (attacker.WeaponHasRerollHitRolls)
        {
            totalHitModifiers += GetHitModifier_RerollHits(baseHitProbability);
        }
        else if (attacker.WeaponHasRerollHitRollsOf1)
        {
            totalHitModifiers += GetHitModifier_RerollHitsOf1(baseHitProbability);
        }

        return (double)(baseHitProbability + totalHitModifiers);
    }

    /// <summary>
    /// Returns the mean of the attacker's hit roll distribution.
    /// </summary>
    /// <param name="attacker">The attacker data object</param>
    /// <returns>A double value containing the average number of successful hit rolls</returns>
    public static double GetMeanHits(AttackerDTO? attacker)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetMeanHits() | Attacker is null, returning 0 ...");
            return 0;
        }

        var averageNumberOfAttacks = GetAverageAttacks(attacker);
        var probabilityOfHit = GetProbabilityOfHit(attacker);
        return Statistics.GetMeanOfDistribution(averageNumberOfAttacks, probabilityOfHit);
    }

    /// <summary>
    /// Gets the discrete expected number of successful hit rolls, based on the average probability.
    /// </summary>
    /// <param name="attacker">The attacker data object</param>
    /// <returns></returns>
    public static int GetExpectedHits(AttackerDTO? attacker) => (int)Math.Floor(GetMeanHits(attacker));

    /// <summary>
    /// Returns the standard deviation of the attacker's hit roll distribution.
    /// TODO: We may want to add a special case for Torrent attacks. Not sure at this point.
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns>A double value containing the standard deviation of successful hits</returns>
    public static double GetStandardDeviationHits(AttackerDTO? attacker)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetStandardDeviationHits() | Attacker is null. Returning 0 ...");
            return 0;
        }

        var averageAttacks = GetAverageAttacks(attacker);
        var varianceAttacks = Statistics.GetVarianceOfResults(attacker.WeaponNumberOfAttackDice, (int)attacker.WeaponAttackDiceType);
        var probabilityOfHit = GetProbabilityOfHit(attacker);

        // If the attacker has Torrent, all attacks will hit.
        // Any variance comes from the number of attacks.
        return attacker.WeaponHasTorrent
            ? Math.Sqrt(varianceAttacks)
            : Statistics.GetCombinedStandardDeviationOfDistribution(averageAttacks, varianceAttacks, probabilityOfHit);
    }

    /// <summary>
    /// Returns a distribution of hit roll results.
    /// </summary>
    /// <param name="attacker">The attacker data object</param>
    /// <param name="distributionType">The type of distribution to create. Defaults to a Binomial distribution.</param>
    /// <returns>A List of BinomomialOutcome data objects, representing a distribution of successful outcomes.</returns>
    public static List<BinomialOutcome> GetDistributionHits(AttackerDTO? attacker, DistributionTypes distributionType = DistributionTypes.Binomial)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetBinomialDistributionOfHits() | Attacker is null. Returning empty list ...");
            return [];
        }

        var minimumAttacks = GetMinimumAttacks(attacker);
        var maximumAttacks = GetMaximumAttacks(attacker);
        var probabilityOfHit = GetProbabilityOfHit(attacker);

        return distributionType switch
        {
            DistributionTypes.Binomial => Statistics.GetBinomialDistribution(minimumAttacks, maximumAttacks, probabilityOfHit),
            DistributionTypes.Cumulative => Statistics.GetCumulativeDistribution(minimumAttacks, maximumAttacks, probabilityOfHit),
            DistributionTypes.Survivor => Statistics.GetSurvivorDistribution(minimumAttacks, maximumAttacks, probabilityOfHit),
            _ => Statistics.GetBinomialDistribution(minimumAttacks, maximumAttacks, probabilityOfHit),
        };
    }

    /// <summary>
    /// Returns the success threshold for wounding the defender.
    /// </summary>
    /// <param name="attackerWeaponStrength"></param>
    /// <param name="defenderToughness"></param>
    /// <returns>An integer value containing the success threshold of the wound roll</returns>
    public static int GetSuccessThresholdOfWound(int attackerWeaponStrength, int defenderToughness)
    {
        if (attackerWeaponStrength <= 0)
        {
            Debug.WriteLine("GetSuccessThresholdOfWound() | Attacker strength is less than or equal to 0. Returning 6+ ...");
            return 6;
        }

        if (defenderToughness <= 0)
        {
            Debug.WriteLine("GetSuccessThresholdOfWound() | Defender toughness is less than or equal to 0. Returning 2+ ...");
            return 2;
        }

        // The attacker's weapon Strength is greater than or equal to double the defender's Toughness.
        return attackerWeaponStrength >= 2 * defenderToughness ? 2
        // The attacker's weapon Strength is greater than, but less than double, the defender's Toughness.
        : attackerWeaponStrength > defenderToughness ? 3
        // The attacker's weapon Strength is equal to the defender's Toughness.
        : attackerWeaponStrength == defenderToughness ? 4
        // The attacker's weapon Strength is less than, but more than half, the defender's Toughness.
        : attackerWeaponStrength > defenderToughness / 2 ? 5
        // The attacker's weapon Strength is less than or equal to half the defender's Toughness.
        : 6;
    }

    /// <summary>
    /// Returns the probability of succeeding in both a hit and a wound roll for any one attack.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns>A double value containing the probability of hitting and wounding with a single attack</returns>
    public static double GetProbabilityOfHitAndWound(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetProbabilityOfHitAndWound() | Attacker is null. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetProbabilityOfHitAndWound() | Defender is null. Returning 0 ...");
            return 0;
        }

        // Calculate hit and wound roll probabilities
        var baseHitProbability = GetProbabilityOfHit(attacker);
        var criticalHitProbability = GetProbabilityOfCriticalHit(attacker);
        var normalHitProbability = baseHitProbability - criticalHitProbability;
        var baseWoundProbability = GetBaseProbabilityOfWound(attacker, defender);

        // Lethal Hits will bypass the wound roll.
        // If the attacker has Lethal Hits, we have to use the normal hit probability
        // to avoid double-counting the critical hit probability
        var hitProbability = attacker.WeaponHasLethalHits && !attacker.WeaponHasTorrent ? normalHitProbability : baseHitProbability;

        // Calculate modifiers
        double totalWoundModifiers = 0;

        if (attacker.WeaponHasLethalHits && !attacker.WeaponHasTorrent)
        {
            totalWoundModifiers += GetWoundModifier_LethalHits(criticalHitProbability);
        }

        if (attacker.WeaponHasSustainedHits && !attacker.WeaponHasTorrent)
        {
            totalWoundModifiers += GetWoundModifier_SustainedHits(criticalHitProbability, attacker.WeaponSustainedHitsMultiplier, baseWoundProbability);
        }

        // Reroll wounds overrides reroll wounds of 1
        if (attacker.WeaponHasRerollWoundRolls)
        {
            totalWoundModifiers += GetWoundModifier_RerollWounds(hitProbability, baseWoundProbability);
        }
        else if (attacker.WeaponHasRerollWoundRollsOf1)
        {
            totalWoundModifiers += GetWoundModifier_RerollWoundsOf1(hitProbability, baseWoundProbability);
        }

        // Calculate total wound probability
        var totalWoundProbability = baseWoundProbability + totalWoundModifiers;

        // Calculate combined hit and wound probability
        return hitProbability * totalWoundProbability;
    }

    /// <summary>
    /// Returns the mean of the attacker's wound roll distribution.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static double GetMeanWounds(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetMeanWounds() | Attacker is null. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetMeanWounds() | Defender is null. Returning 0 ...");
            return 0;
        }

        var averageAttacks = GetAverageAttacks(attacker);
        var probabilityOfHitAndWound = GetProbabilityOfHitAndWound(attacker, defender);
        return Statistics.GetMeanOfDistribution(averageAttacks, probabilityOfHitAndWound);
    }

    /// <summary>
    /// Gets the discrete expected number of successful wound rolls, based on the average probability.
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    public static int GetExpectedWounds(AttackerDTO? attacker, DefenderDTO? defender) => (int)Math.Floor(GetMeanWounds(attacker, defender));

    /// <summary>
    /// Returns the standard deviation of the attacker's wound roll distribution.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static double GetStandardDeviationWounds(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetStandardDeviationWounds() | Attacker is null. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetStandardDeviationWounds() | Defender is null. Returning 0 ...");
            return 0;
        }

        var averageAttacks = GetAverageAttacks(attacker);
        var varianceAttacks = Statistics.GetVarianceOfResults(attacker.WeaponNumberOfAttackDice, (int)attacker.WeaponAttackDiceType);
        var probabilityOfHitAndWound = GetProbabilityOfHitAndWound(attacker, defender);
        return Statistics.GetCombinedStandardDeviationOfDistribution(averageAttacks, varianceAttacks, probabilityOfHitAndWound);
    }

    /// <summary>
    /// Returns a distribution of hit and wound roll results.
    /// </summary>
    /// <param name="attacker">The attacker data object</param>
    /// <param name="defender">The defender data object</param>
    /// <param name="distributionType">The type of distribution to create. Defaults to a Binomial distribution.</param>
    /// <returns>A List of BinomomialOutcome data objects, representing a distribution of successful outcomes.</returns>
    public static List<BinomialOutcome> GetDistributionWounds(AttackerDTO? attacker, DefenderDTO? defender, DistributionTypes distributionType = DistributionTypes.Binomial)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetBinomialDistributionWounds() | Attacker is null. Returning empty list ...");
            return [];
        }

        if (defender == null)
        {
            Debug.WriteLine("GetBinomialDistributionWounds() | Defender is null. Returning empty list ...");
            return [];
        }

        var minimumAttacks = GetMinimumAttacks(attacker);
        var maximumAttacks = GetMaximumAttacks(attacker);
        var probability = GetProbabilityOfHitAndWound(attacker, defender);

        return distributionType switch
        {
            DistributionTypes.Binomial => Statistics.GetBinomialDistribution(minimumAttacks, maximumAttacks, probability),
            DistributionTypes.Cumulative => Statistics.GetCumulativeDistribution(minimumAttacks, maximumAttacks, probability),
            DistributionTypes.Survivor => Statistics.GetSurvivorDistribution(minimumAttacks, maximumAttacks, probability),
            _ => Statistics.GetBinomialDistribution(minimumAttacks, maximumAttacks, probability),
        };
    }

    /// <summary>
    /// Returns the adjusted armor save of the defender after applying the attacker's armor pierce.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static int GetAdjustedArmorSaveThreshold(AttackerDTO attacker, DefenderDTO defender)
    {
        if ((defender.ArmorSave <= 0 && defender.InvulnerableSave <= 0)
            || (defender.ArmorSave <= 0 && defender.InvulnerableSave >= 7)
            || (defender.ArmorSave >= 7 && defender.InvulnerableSave <= 0)
            || (defender.ArmorSave >= 7 && defender.InvulnerableSave >= 7))
        {
            Debug.WriteLine("GetAdjustedArmorSaveThreshold() | Defender has invalid armor save and invulnverable save values. Returning adjusted save value of 7+ ...");
            return 6;
        }

        // If the defender has an invulnerable save, and the invulnerable save is lower than the regular save after applying armor pierce,
        // then use the invulnerable save.
        // Compare against minimum armor save to guard against negative armor pierce values.
        const int minimumArmorSave = 2;
        var effectiveInvulnerableSave = defender.InvulnerableSave <= 0 ? 7 : defender.InvulnerableSave;
        var piercedArmorSaveThreshold = defender.ArmorSave + attacker.WeaponArmorPierce;
        var adjustedArmorSave = Math.Min(piercedArmorSaveThreshold, effectiveInvulnerableSave);
        return Math.Max(minimumArmorSave, adjustedArmorSave);
    }

    /// <summary>
    /// Returns the probability of the attacker passing their hit and wound roll, and the defender failing their save, for any one attack.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static double GetProbabilityOfHitAndWoundAndFailedSave(AttackerDTO? attacker, DefenderDTO? defender)
    {
        // Validate inputs
        if (attacker == null)
        {
            Debug.WriteLine("GetProbabilityOfFailedSave() | Attacker is null, returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetProbabilityOfFailedSave() | Defender is null, returning 0 ...");
            return 0;
        }

        // Base probabilities
        var baseHitProbability = GetProbabilityOfHit(attacker);
        var criticalHitProbability = GetProbabilityOfCriticalHit(attacker);
        var normalHitProbability = baseHitProbability - criticalHitProbability;
        var baseFailedSaveProbability = GetBaseProbabilityOfFailedSave(attacker, defender);
        var hitAndWoundProbability = GetProbabilityOfHitAndWound(attacker, defender);

        // Calculate Devastating Wounds (only from normal hits that roll critical wounds)
        double totalFailedSaveModifiers = 0;

        if (attacker.WeaponHasDevastatingWounds)
        {
            // Adjust hit probability if Lethal Hits are active
            var hitProbability = attacker.WeaponHasLethalHits && !attacker.WeaponHasTorrent ? normalHitProbability : baseHitProbability;
            totalFailedSaveModifiers += GetFailedSaveModifier_DevastatingWounds(hitProbability, GetProbabilityOfCriticalWound(attacker));
        }

        // Calculate total failed save probability
        var totalFailedSaveProbability = baseFailedSaveProbability + totalFailedSaveModifiers;
        return (double)(hitAndWoundProbability * baseFailedSaveProbability);
    }

    /// <summary>
    /// Returns the mean of the failed save roll distribution.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static double GetMeanFailedSaves(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetMeanFailedSaves() | Attacker is null, returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetMeanFailedSaves() | Defender is null, returning 0 ...");
            return 0;
        }

        var averageAttacks = GetAverageAttacks(attacker);
        var probabilityOfFailedSave = GetProbabilityOfHitAndWoundAndFailedSave(attacker, defender);
        return Statistics.GetMeanOfDistribution(averageAttacks, probabilityOfFailedSave);
    }

    /// <summary>
    /// Gets the discrete expected number of failed save rolls, based on the average probability.
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    public static int GetExpectedFailedSaves(AttackerDTO? attacker, DefenderDTO? defender) => (int)Math.Floor(GetMeanFailedSaves(attacker, defender));

    /// <summary>
    /// Returns the standard deviation of the failed save roll distribution.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static double GetStandardDeviationFailedSaves(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetStandardDeviationFailedSaves() | Attacker is null, returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetStandardDeviationFailedSaves() | Defender is null, returning 0 ...");
            return 0;
        }

        var averageAttacks = GetAverageAttacks(attacker);
        var varianceAttacks = Statistics.GetVarianceOfResults(attacker.WeaponNumberOfAttackDice, (int)attacker.WeaponAttackDiceType);
        var probabilityOfHitAndWoundAndFailedSave = GetProbabilityOfHitAndWoundAndFailedSave(attacker, defender);
        return Statistics.GetCombinedStandardDeviationOfDistribution(averageAttacks, varianceAttacks, probabilityOfHitAndWoundAndFailedSave);
    }

    /// <summary>
    /// Returns a distribution of hit and wound and failed save roll results.
    /// </summary>
    /// <param name="attacker">The attacker data object</param>
    /// <param name="defender">The defender data object</param>
    /// <param name="distributionType">The type of distribution to create. Defaults to a Binomial distribution.</param>
    /// <returns>A List of BinomomialOutcome data objects, representing a distribution of successful outcomes.</returns>
    public static List<BinomialOutcome> GetDistributionFailedSaves(AttackerDTO? attacker, DefenderDTO? defender, DistributionTypes distributionType = DistributionTypes.Binomial)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetBinomialDistributionFailedSaves() | Attacker is null. Returning empty list ...");
            return [];
        }

        if (defender == null)
        {
            Debug.WriteLine("GetBinomialDistributionFailedSaves() | Defender is null. Returning empty list ...");
            return [];
        }

        var minimumAttacks = GetMinimumAttacks(attacker);
        var maximumAttacks = GetMaximumAttacks(attacker);
        var probability = GetProbabilityOfHitAndWoundAndFailedSave(attacker, defender);

        return distributionType switch
        {
            DistributionTypes.Binomial => Statistics.GetBinomialDistribution(minimumAttacks, maximumAttacks, probability),
            DistributionTypes.Cumulative => Statistics.GetCumulativeDistribution(minimumAttacks, maximumAttacks, probability),
            DistributionTypes.Survivor => Statistics.GetSurvivorDistribution(minimumAttacks, maximumAttacks, probability),
            _ => Statistics.GetBinomialDistribution(minimumAttacks, maximumAttacks, probability),
        };
    }

    /// <summary>
    /// Gets the average amount of damage done by the attacker after all hit, wound, and save rolls have been completed.
    /// This does not take into account feel no pain rolls or damage reduction abilities.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static double GetMeanDamage(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetMeanDamageNet() | Attacker is null. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetMeanDamageNet() | Defender is null. Returning 0 ...");
            return 0;
        }

        var meanFailedSaves = GetMeanFailedSaves(attacker, defender);
        var averageDamagePerAttack = GetAverageDamagePerAttack(attacker);
        return meanFailedSaves * averageDamagePerAttack;
    }

    /// <summary>
    /// Gets the discrete expected total amount of damage after all hit, wound, and save rolls have been completed.
    /// This does not take into account feel no pain rolls or damage reduction abilities.
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    public static int GetExpectedDamage(AttackerDTO? attacker, DefenderDTO? defender) => (int)Math.Floor(GetMeanDamage(attacker, defender));

    /// <summary>
    /// Gets the standard deviation of damage done after all hit, wound, and save rolls have been completed.
    /// This does not take into account feel no pain rolls or damage reduction abilities.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static double GetStandardDeviationDamage(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetStandardDeviationDamageNet() | Attacker is null. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetStandardDeviationDamageNet() | Defender is null. Returning 0 ...");
            return 0;
        }

        var averageAttacks = GetAverageAttacks(attacker);
        var varianceAttacks = Statistics.GetVarianceOfResults(attacker.WeaponNumberOfAttackDice, (int)attacker.WeaponAttackDiceType);
        var probabilityOfHitAndWoundAndFailedSave = GetProbabilityOfHitAndWoundAndFailedSave(attacker, defender);
        var averageDamagePerAttack = GetAverageDamagePerAttack(attacker);
        return Statistics.GetCombinedStandardDeviationOfDistribution(averageAttacks, varianceAttacks, probabilityOfHitAndWoundAndFailedSave) * averageDamagePerAttack;
    }

    /// <summary>
    /// Get the mean number of defending models that will be destroyed by the attack.
    /// This takes into account any feel no pain rolls and damage reduction abilities.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns>A double value containing the mean number of models destroyed by the attack.</returns>
    public static double GetMeanDestroyedModels(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetMeanDestroyedModels() | Attacker is null. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetMeanDestroyedModels() | Defender is null. Returning 0 ...");
            return 0;
        }

        var meanFailedSaves = GetMeanFailedSaves(attacker, defender);
        var averageDamagePerAttack = GetAverageDamagePerAttack(attacker);
        var adjustedDamagePerAttack = GetAverageAdjustedDamagePerAttack(averageDamagePerAttack, defender);
        return GetModelsDestroyed(meanFailedSaves, adjustedDamagePerAttack, defender);
    }

    /// <summary>
    /// Get the expected number of defending models that will be destroyed by the attack.
    /// This takes into account any feel no pain rolls and damage reduction abilities.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns>An integer value containing the expected number of destroyed models</returns>
    public static int GetExpectedDestroyedModels(AttackerDTO? attacker, DefenderDTO? defender) => (int)Math.Floor(GetMeanDestroyedModels(attacker, defender));

    /// <summary>
    /// Gets the standard deviation of destroyed models.
    /// This takes into account any feel no pain rolls and damage reduction abilities.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns>An integer value containing the standard deviation of destroyed models.</returns>
    public static double GetStandardDeviationDestroyedModels(AttackerDTO? attacker, DefenderDTO? defender)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetStandardDeviationDestroyedModels() | Attacker is null. Returning 0 ...");
            return 0;
        }

        if (defender == null)
        {
            Debug.WriteLine("GetStandardDeviationDestroyedModels() | Defender is null. Returning 0 ...");
            return 0;
        }

        // Calculate the standard deviation of failed saves
        var averageAttacks = GetAverageAttacks(attacker);
        var varianceAttacks = Statistics.GetVarianceOfResults(attacker.WeaponNumberOfAttackDice, (int)attacker.WeaponAttackDiceType);
        var probabilityOfHitAndWoundAndFailedSave = GetProbabilityOfHitAndWoundAndFailedSave(attacker, defender);
        var standardDeviationSuccessfulAttacks = Statistics.GetCombinedStandardDeviationOfDistribution(averageAttacks, varianceAttacks, probabilityOfHitAndWoundAndFailedSave);

        // Calculate the average amount of adjusted damage done per failed save
        var averageDamagePerAttack = GetAverageDamagePerAttack(attacker);
        var adjustedDamagePerAttack = GetAverageAdjustedDamagePerAttack(averageDamagePerAttack, defender);

        // Calculate how many models would be destroyed
        return GetModelsDestroyed(standardDeviationSuccessfulAttacks, adjustedDamagePerAttack, defender);
    }

    /// <summary>
    /// Returns a distribution of destroyed models from an attack.
    /// </summary>
    /// <param name="attacker">The attacker data object</param>
    /// <param name="defender">The defender data object</param>
    /// <param name="distributionType">The type of distribution to create. Defaults to a Binomial distribution.</param>
    /// <returns>A List of BinomomialOutcome data objects, representing a distribution of destroyed models.</returns>
    public static List<BinomialOutcome> GetDistributionDestroyedModels(AttackerDTO? attacker, DefenderDTO? defender, DistributionTypes distributionType = DistributionTypes.Binomial)
    {
        if (attacker == null)
        {
            Debug.WriteLine("GetBinomialDistributionDestroyedModels() | Attacker is null. Returning empty list ...");
            return [];
        }

        if (defender == null)
        {
            Debug.WriteLine("GetBinomialDistributionDestroyedModels() | Defender is null. Returning empty list ...");
            return [];
        }

        // Get probability of a successful attack
        var probability = GetProbabilityOfHitAndWoundAndFailedSave(attacker, defender);

        // Get upper and lower bounds for the number of trials
        var minimumAttacks = GetMinimumAttacks(attacker);
        var maximumAttacks = GetMaximumAttacks(attacker);

        // Get lower bound of group success count
        var maximumDamagePerAttack = GetMaximumDamagePerAttack(attacker);
        var maximumAdjustedDamagePerAttack = GetMaximumAdjustedDamagePerAttack(maximumDamagePerAttack, defender);
        var minGroupSuccessCount = GetAttacksRequiredToDestroyOneModel(maximumAdjustedDamagePerAttack, defender);

        // Get upper bound of group success count
        var minimumDamagePerAttack = GetMinimumDamagePerAttack(attacker);
        var minimumAdjustedDamagePerAttack = GetMinimumAdjustedDamagePerAttack(minimumDamagePerAttack, defender);
        var maxAttacksRequiredToDestroyOneModel = GetAttacksRequiredToDestroyOneModel(minimumAdjustedDamagePerAttack, defender);
        var maxGroupSuccessCount = maxAttacksRequiredToDestroyOneModel == 0 ? maximumAttacks + 1 : maxAttacksRequiredToDestroyOneModel;

        // Get distribution
        return distributionType switch
        {
            DistributionTypes.Binomial => Statistics.GetBinomialDistribution(minimumAttacks, maximumAttacks, probability, minGroupSuccessCount, maxGroupSuccessCount),
            DistributionTypes.Cumulative => Statistics.GetCumulativeDistribution(minimumAttacks, maximumAttacks, probability, minGroupSuccessCount, maxGroupSuccessCount),
            DistributionTypes.Survivor => Statistics.GetSurvivorDistribution(minimumAttacks, maximumAttacks, probability, minGroupSuccessCount, maxGroupSuccessCount),
            _ => Statistics.GetBinomialDistribution(minimumAttacks, maximumAttacks, probability, minGroupSuccessCount, maxGroupSuccessCount),
        };
    }
}
