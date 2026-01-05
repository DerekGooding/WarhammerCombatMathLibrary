using WarhammerCombatMathLibrary.Model;

namespace WarhammerCombatMathLibrary.Statistics;

internal static class Helpers
{
    internal static List<BinomialOutcome> NormalizeDistribution(List<BinomialOutcome> distribution)
    {
        if (distribution == null || distribution.Count == 0)
        {
            return distribution ?? [];
        }

        var totalProbability = distribution.Sum(outcome => outcome.Probability);

        if (Math.Abs(totalProbability) < PROBABILITY_TOLERANCE || Math.Abs(totalProbability - 1.0) < PROBABILITY_TOLERANCE)
        {
            return distribution;
        }

        return distribution.ConvertAll(outcome => new BinomialOutcome
        {
            Successes = outcome.Successes,
            Probability = outcome.Probability / totalProbability
        });
    }

    internal static List<BinomialOutcome> ApplyCumulativeFunction(List<BinomialOutcome> distribution)
    {
        var cumulativeDistribution = new List<BinomialOutcome>();
        double cumulative = 0;

        for (var i = 0; i < distribution.Count; i++)
        {
            cumulative += distribution[i].Probability;

            var probability = (i == distribution.Count - 1 && Math.Abs(cumulative - 1.0) < PROBABILITY_TOLERANCE)
                ? 1.0
                : Math.Min(cumulative, 1.0);

            cumulativeDistribution.Add(new BinomialOutcome
            {
                Successes = distribution[i].Successes,
                Probability = probability
            });
        }

        return cumulativeDistribution;
    }

    internal static List<BinomialOutcome> ApplySurvivorFunction(List<BinomialOutcome> distribution)
    {
        var survivorDistribution = new List<BinomialOutcome>();
        double cumulative = 0;

        for (var i = distribution.Count - 1; i >= 0; i--)
        {
            cumulative += distribution[i].Probability;

            // For the first element (i == 0), set to exactly 1.0 if it's within tolerance to handle floating point precision
            var probability = (i == 0 && Math.Abs(cumulative - 1.0) < PROBABILITY_TOLERANCE)
                ? 1.0
                : Math.Min(cumulative, 1.0);

            survivorDistribution.Insert(0, new BinomialOutcome
            {
                Successes = distribution[i].Successes,
                Probability = probability
            });
        }

        return survivorDistribution;
    }

    internal static List<BinomialOutcome> CreateDistribution(DistributionTypes distributionType, int numberOfTrials, double probability, int groupSuccessCount = 1)
    {
        var baseDistribution = new List<BinomialOutcome>();

        var maxK = Math.Floor((double)numberOfTrials / groupSuccessCount);

        for (var k = 0; k <= maxK; k++)
        {
            var groupedSuccesses = k * groupSuccessCount;
            var discreteProbability = ProbabilityMassFunction(numberOfTrials, groupedSuccesses, probability);

            baseDistribution.Add(new BinomialOutcome
            {
                Successes = k,
                Probability = discreteProbability
            });
        }

        if (groupSuccessCount > 1)
        {
            baseDistribution = NormalizeDistribution(baseDistribution);
        }

        return distributionType switch
        {
            DistributionTypes.Binomial => baseDistribution,
            DistributionTypes.Cumulative => ApplyCumulativeFunction(baseDistribution),
            DistributionTypes.Survivor => ApplySurvivorFunction(baseDistribution),
            _ => baseDistribution
        };
    }

    internal static List<BinomialOutcome> CreateDistribution(DistributionTypes distributionType, int numberOfTrials, double probability, int minGroupSuccessCount, int maxGroupSuccessCount)
    {
        if (minGroupSuccessCount == maxGroupSuccessCount)
        {
            return CreateDistribution(distributionType, numberOfTrials, probability, maxGroupSuccessCount);
        }

        var probabilitySums = new Dictionary<int, double>();
        var probabilityWeights = new Dictionary<int, int>();

        var maxK = Math.Floor((double)numberOfTrials / minGroupSuccessCount);

        for (var g = minGroupSuccessCount; g <= maxGroupSuccessCount; g++)
        {
            for (var k = 0; k <= maxK; k++)
            {
                var groupedSuccesses = k * g;
                var discreteProbability = ProbabilityMassFunction(numberOfTrials, groupedSuccesses, probability);

                if (!probabilitySums.ContainsKey(k))
                {
                    probabilitySums[k] = 0;
                    probabilityWeights[k] = 0;
                }

                probabilitySums[k] += discreteProbability;
                probabilityWeights[k]++;
            }
        }

        var baseDistribution = probabilitySums
             .Select(pair => new BinomialOutcome
             {
                 Successes = pair.Key,
                 Probability = pair.Value / probabilityWeights[pair.Key]
             })
             .OrderBy(outcome => outcome.Successes)
             .ToList();

        baseDistribution = NormalizeDistribution(baseDistribution);

        return distributionType switch
        {
            DistributionTypes.Binomial => baseDistribution,
            DistributionTypes.Cumulative => ApplyCumulativeFunction(baseDistribution),
            DistributionTypes.Survivor => ApplySurvivorFunction(baseDistribution),
            _ => baseDistribution
        };
    }

    internal static List<BinomialOutcome> CreateDistribution(DistributionTypes distributionType, int minNumberOfTrials, int maxNumberOfTrials, double probability, int groupSuccessCount = 1)
    {
        if (minNumberOfTrials == maxNumberOfTrials)
        {
            return CreateDistribution(distributionType, maxNumberOfTrials, probability, groupSuccessCount);
        }

        var probabilitySums = new Dictionary<int, double>();

        var maxK = Math.Floor((double)maxNumberOfTrials / groupSuccessCount);

        for (var k = 0; k <= maxK; k++)
        {
            var groupedSuccesses = k * groupSuccessCount;
            double combinedProbability = 0;

            for (var n = 1; n <= maxNumberOfTrials; n++)
            {
                var discreteProbability = ProbabilityMassFunction(n, groupedSuccesses, probability);
                combinedProbability += discreteProbability;
            }

            if (combinedProbability > 0)
            {
                combinedProbability /= maxNumberOfTrials;
            }

            if (!probabilitySums.ContainsKey(k))
            {
                probabilitySums[k] = 0;
            }

            probabilitySums[k] += combinedProbability;
        }

        var baseDistribution = probabilitySums
         .Select(pair => new BinomialOutcome
         {
             Successes = pair.Key,
             Probability = pair.Value
         })
         .OrderBy(outcome => outcome.Successes)
         .ToList();

        baseDistribution = NormalizeDistribution(baseDistribution);

        return distributionType switch
        {
            DistributionTypes.Binomial => baseDistribution,
            DistributionTypes.Cumulative => ApplyCumulativeFunction(baseDistribution),
            DistributionTypes.Survivor => ApplySurvivorFunction(baseDistribution),
            _ => baseDistribution
        };
    }

    internal static List<BinomialOutcome> CreateDistribution(DistributionTypes distributionType, int minNumberOfTrials, int maxNumberOfTrials, double probability, int minGroupSuccessCount, int maxGroupSuccessCount)
    {
        if (minNumberOfTrials == maxNumberOfTrials)
        {
            return CreateDistribution(distributionType, maxNumberOfTrials, probability, minGroupSuccessCount, maxGroupSuccessCount);
        }

        if (minGroupSuccessCount == maxGroupSuccessCount)
        {
            return CreateDistribution(distributionType, minNumberOfTrials, maxNumberOfTrials, probability, maxGroupSuccessCount);
        }

        var probabilitySums = new Dictionary<int, double>();
        var probabilityWeights = new Dictionary<int, int>();

        var maxK = Math.Floor((double)maxNumberOfTrials / minGroupSuccessCount);

        for (var g = minGroupSuccessCount; g <= maxGroupSuccessCount; g++)
        {
            for (var k = 0; k <= maxK; k++)
            {
                var groupedSuccesses = k * g;
                double combinedProbability = 0;

                for (var n = 1; n <= maxNumberOfTrials; n++)
                {
                    var discreteProbability = ProbabilityMassFunction(n, groupedSuccesses, probability);
                    combinedProbability += discreteProbability;
                }

                if (combinedProbability > 0)
                {
                    combinedProbability /= maxNumberOfTrials;
                }

                if (!probabilitySums.ContainsKey(k))
                {
                    probabilitySums[k] = 0;
                    probabilityWeights[k] = 0;
                }

                probabilitySums[k] += combinedProbability;
                probabilityWeights[k]++;
            }
        }

        var baseDistribution = probabilitySums
         .Select(pair => new BinomialOutcome
         {
             Successes = pair.Key,
             Probability = pair.Value / probabilityWeights[pair.Key]
         })
         .OrderBy(outcome => outcome.Successes)
         .ToList();

        baseDistribution = NormalizeDistribution(baseDistribution);

        return distributionType switch
        {
            DistributionTypes.Binomial => baseDistribution,
            DistributionTypes.Cumulative => ApplyCumulativeFunction(baseDistribution),
            DistributionTypes.Survivor => ApplySurvivorFunction(baseDistribution),
            _ => baseDistribution
        };
    }
}
