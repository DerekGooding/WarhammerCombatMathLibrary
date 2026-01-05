using System.Diagnostics;
using System.Numerics;

namespace WarhammerCombatMath.Statistics;

/// <summary>
/// A static class that provides statstical math functions.
/// </summary>
public static class Utility
{
    internal static readonly BoundedCache<(int, int, double), double> _probabilityMassFunctionCache = new(MaxCache);

    /// <summary>
    /// Calculates the probability of success for a single trial.
    /// </summary>
    /// <param name="numberOfPossibleResults"></param>
    /// <param name="numberOfSuccessfulResults"></param>
    /// <returns>A double value containing the probability of success.</returns>
    public static double GetProbabilityOfSuccess(int numberOfPossibleResults, int numberOfSuccessfulResults)
    {
        // Validate parameters
        if (numberOfPossibleResults <= 0)
        {
            Debug.WriteLine("ProbabilityOfSuccess() | Number of possible results is less than or equal to 0. Returning 0 ...");
            return 0;
        }

        if (numberOfSuccessfulResults < 1)
        {
            Debug.WriteLine("ProbabilityOfSuccess() | Number of successful results is less than 1. Returning 0 ...");
            return 0;
        }

        if (numberOfSuccessfulResults > numberOfPossibleResults)
        {
            Debug.WriteLine("ProbabilityOfSuccess() | Number of successful results is greater than the number of possible results. Returning 1 ...");
            return 1;
        }

        // Perform calculation
        return (double)numberOfSuccessfulResults / numberOfPossibleResults;
    }

    /// <summary>
    /// Calculates the mean result from the number of possible results.
    /// </summary>
    /// <param name="numberOfPossibleResults"></param>
    /// <returns>An integer value containing the mean result.</returns>
    public static int GetMeanResult(int numberOfPossibleResults)
    {
        // Validate inputs
        if (numberOfPossibleResults <= 0)
        {
            return 0;
        }

        // Use formula for adding natural numbers from 1 to n
        var sum = (double)(numberOfPossibleResults * (numberOfPossibleResults + 1)) / 2;

        // Calcualte average
        return (int)Math.Round((double)sum / numberOfPossibleResults);

    }

    /// <summary>
    /// Determines the variance of results when a trial with a specified number of equally likely results is repeated a number of times.
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="numberOfPossibleResults"></param>
    /// <returns>A double value containing the variance of results for a repeated trial</returns>
    public static double GetVarianceOfResults(int numberOfTrials, double numberOfPossibleResults)
    {
        if (numberOfTrials <= 0 || numberOfPossibleResults <= 1)
        {
            return 0;
        }

        // Variance of a trial = (results^2 - 1) / 12
        var singleDieVariance = (Math.Pow(numberOfPossibleResults, 2) - 1) / 12.0;

        return numberOfTrials * singleDieVariance;
    }

    /// <summary>
    /// Determines the standard deviation of results when a trial with a specified number of equally likely results is repeated a number of times.
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="numberOfPossibleResults"></param>
    /// <returns>A double value containing the standard deviation of results for a repeated trial</returns>
    public static double GetStandardDeviationOfResults(int numberOfTrials, double numberOfPossibleResults) => Math.Sqrt(GetVarianceOfResults(numberOfTrials, numberOfPossibleResults));

    /// <summary>
    /// Calculates the binomial coefficient, determining the number of combinations of k elements
    /// in a population of n, independent of order.
    /// </summary>
    /// <param name="population">The population of the group or set.</param>
    /// <param name="combinationSize">The number elements in a unique combination.</param>
    /// <returns>A double value containing the binomial coefficient.</returns>
    public static BigInteger BinomialCoefficient(int population, int combinationSize)
    {
        // Validate parameters
        if (population < 0)
        {
            Debug.WriteLine("BinomialCoefficient() | Population is less than 0. Returning 0 ...");
            return 0;
        }

        if (combinationSize < 0)
        {
            Debug.WriteLine("BinomialCoefficient() | Combination size is less than 0. Returning 0 ...");
            return 0;
        }

        if (combinationSize > population)
        {
            Debug.WriteLine("BinomialCoefficient() | Combination size must be less than or equal to the total population. Returning 0 ...");
            return 0;
        }

        // Perform calculation
        var factorialTotal = Factorial(population);
        var factorialCombination = Factorial(combinationSize);
        var factorialDifference = Factorial(population - combinationSize);
        return factorialTotal / (factorialCombination * factorialDifference);
    }

    /// <summary>
    /// Calculates the probability for the success of a given number of trials
    /// using a specified probability of success for a single trial.
    /// </summary>
    /// <param name="probability">Probability of success for a single trial.</param>
    /// <param name="numberOfSuccesses">Number of trials.</param>
    /// <returns>A double value containing the probability that all trials will be successful.</returns>
    public static double ProbabilityOfMultipleSuccesses(double probability, int numberOfSuccesses)
    {
        // Validate parameters
        if (probability <= 0)
        {
            Debug.WriteLine("ProbabilityOfMultipleSuccesses() | Probability is less than or equal to 0. Returning 0 ...");
            return 0;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("ProbabilityOfMultipleSuccesses() | Probability is greater than or equal to 1. Returning 1 ...");
            return 1;
        }

        if (numberOfSuccesses < 0)
        {
            Debug.WriteLine("ProbabilityOfMultipleSuccesses() | Number of successes is less than 0. Returning 0 ...");
            return 0;
        }

        // Perform calculation
        return Math.Pow(probability, numberOfSuccesses);
    }

    /// <summary>
    /// Calculates the probability mass function for a given number of successes
    /// when a given number of dice are rolled with a given success threshold.
    /// The function can be broken down as a combination of the following:
    /// - The binomial coefficient, describing the number of unique combinations of results that can contain the desired number of successes.
    /// - The probability of finding the exact specified number of successful results.
    /// - The probability of finding the remaining results to be failures.
    /// </summary>
    /// <param name="numberOfTrials">The total number of dice.</param>
    /// <param name="numberOfSuccesses">The number of successes.</param>
    /// <param name="probability">The probability of success for a single die roll.</param>
    /// <returns></returns>
    public static double ProbabilityMassFunction(int numberOfTrials, int numberOfSuccesses, double probability)
    {
        // Create the cache key for this calculation
        var key = (numberOfTrials, numberOfSuccesses, probability);

        // If this calculation has already been done, retrieve it from the cache
        if (_probabilityMassFunctionCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        // Validate inputs
        if (numberOfTrials < 1 || numberOfSuccesses < 0 || numberOfSuccesses > numberOfTrials || probability <= 0)
        {
            return 0;
        }

        if (probability >= 1)
        {
            return numberOfSuccesses == numberOfTrials ? 1 : 0;
        }

        var binomialCoefficient = BinomialCoefficient(numberOfTrials, numberOfSuccesses);
        var successProbability = ProbabilityOfMultipleSuccesses(probability, numberOfSuccesses);
        var failureProbability = ProbabilityOfMultipleSuccesses(1 - probability, numberOfTrials - numberOfSuccesses);
        var result = (double)binomialCoefficient * successProbability * failureProbability;

        // Cache this result
        _probabilityMassFunctionCache.Add(key, result);

        return result;
    }

    /// <summary>
    /// Calculates the binomial distribution of trial data.
    /// Optionally calculates the binomial distribution of trial data, assuming that a group of a given number of trial successes is considered a single success
    /// in the context of the distribution.
    /// Example: If 'groupSuccessCount' = 2, then the total number of trials is divided by 2, and any combination of 2 successful trials is considered a single success
    /// in the context of the distribution.
    /// </summary>
    /// <param name="numberOfTrials">The number of trials in the process.</param>
    /// <param name="probability">The probability of success for a single trial.</param>
    /// <param name="groupSuccessCount">The number of grouped trial successes that count as a success. Default is 1 (one trial success equals one binomial success)</param>
    /// <returns>A binomial distribution of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetBinomialDistribution(int numberOfTrials, double probability, int groupSuccessCount = 1)
    {
        // Validate parameters
        if (numberOfTrials <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Probability is less than or equal to 0.");

            // The probability of 0 successes should be 1, all other probabilities should be 0.
            var adjustedDistribution = new List<BinomialOutcome>
            {
                new(0, 1)
            };

            for (var k = 1; k <= numberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            return adjustedDistribution;
        }

        if (groupSuccessCount <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (groupSuccessCount > numberOfTrials)
        {
            Debug.WriteLine("BinomialDistribution() | Group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Calculate distribution
        return CreateDistribution(DistributionTypes.Binomial, numberOfTrials, probability, groupSuccessCount);
    }

    /// <summary>
    /// Calculates the combined binomial distribution of trial data using a variable group success count.
    /// </summary>
    /// <param name="numberOfTrials">The number of trials in the process.</param>
    /// <param name="probability">The probability of success for a single trial.</param>
    /// <param name="minGroupSuccessCount">The minimum number of grouped trial successes that count as a success.</param>
    /// <param name="maxGroupSuccessCount">The maximum number of grouped trial successes that count as a success.</param>
    /// <returns>A binomial distribution of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetBinomialDistribution(int numberOfTrials, double probability, int minGroupSuccessCount, int maxGroupSuccessCount)
    {
        // Validate parameters
        if (numberOfTrials <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Probability is less than or equal to 0.");

            // The probability of 0 successes should be 1, all other probabilities should be 0.
            var adjustedDistribution = new List<BinomialOutcome>
            {
                new(0, 1)
            };

            for (var k = 1; k <= numberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            return adjustedDistribution;
        }

        if (minGroupSuccessCount <= 0)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Minimum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxGroupSuccessCount <= 0)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Maximum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxGroupSuccessCount)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Minimum group success count is greater than maximum group success count.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > numberOfTrials)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Minimum group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Binomial, numberOfTrials, probability, minGroupSuccessCount, maxGroupSuccessCount);
    }

    /// <summary>
    /// Calculates the combined binomial distribution of trial data using a variable number of trials.
    /// Optionally calculates the binomial distribution of trial data, assuming that a group of a given number of trial successes is considered a single success
    /// in the context of the distribution.
    /// Example: If 'groupSuccessCount' = 2, then the total number of trials is divided by 2, and any combination of 2 successful trials is considered a single success
    /// in the context of the distribution.
    /// </summary>
    /// <param name="minNumberOfTrials">The minimum nunmber of trials in the process.</param>
    /// <param name="maxNumberOfTrials">The maximum number of trials in the process.</param>
    /// <param name="probability">The probability of success for a single trial.</param>
    /// <param name="groupSuccessCount">The number of grouped trial successes that count as a success. Default is 1 (one trial success equals one binomial success)</param>
    /// <returns>A binomial distribution of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetBinomialDistribution(int minNumberOfTrials, int maxNumberOfTrials, double probability, int groupSuccessCount = 1)
    {
        // Validate parameters
        if (minNumberOfTrials <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Min number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxNumberOfTrials <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Max number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Probability is less than or equal to 0.");

            // The probability of 0 successes should be 1, all other probabilities should be 0.
            var adjustedDistribution = new List<BinomialOutcome>
            {
                new(0, 1)
            };

            for (var k = 1; k <= maxNumberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            return adjustedDistribution;
        }

        if (groupSuccessCount <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (groupSuccessCount > maxNumberOfTrials)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Binomial, minNumberOfTrials, maxNumberOfTrials, probability, groupSuccessCount);
    }

    /// <summary>
    /// Calculates the combined binomial distribution of trial data using a variable number of trials and a variable group success count.
    /// </summary>
    /// <param name="minNumberOfTrials">The minimum nunmber of trials in the process.</param>
    /// <param name="maxNumberOfTrials">The maximum number of trials in the process.</param>
    /// <param name="probability">The probability of success for a single trial.</param>
    /// <param name="minGroupSuccessCount">The minimum number of grouped trial successes that count as a success.</param>
    /// <param name="maxGroupSuccessCount">The maximum number of grouped trial successes that count as a success.</param>
    /// <returns>A binomial distribution of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetBinomialDistribution(int minNumberOfTrials, int maxNumberOfTrials, double probability, int minGroupSuccessCount, int maxGroupSuccessCount)
    {
        // Validate parameters
        if (minNumberOfTrials <= 0)
        {
            Debug.WriteLine("CreateDistribution() | Min number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxNumberOfTrials <= 0)
        {
            Debug.WriteLine("CreateDistribution() | Max number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minNumberOfTrials > maxNumberOfTrials)
        {
            Debug.WriteLine("CreateDistribution() | Min number of trials is greater than max number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("BinomialDistribution() | Probability is less than or equal to 0.");

            // The probability of 0 successes should be 1, all other probabilities should be 0.
            var adjustedDistribution = new List<BinomialOutcome>
            {
                new(0, 1)
            };

            for (var k = 1; k <= maxNumberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            return adjustedDistribution;
        }

        if (minGroupSuccessCount <= 0)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Minimum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxGroupSuccessCount <= 0)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Maximum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxGroupSuccessCount)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Minimum group success count is greater than maximum group success count.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxNumberOfTrials)
        {
            Debug.WriteLine("BinomialDistributionVariableTrials() | Minimum group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Binomial, minNumberOfTrials, maxNumberOfTrials, probability, minGroupSuccessCount, maxGroupSuccessCount);
    }


    /// <summary>
    /// Calculates the lower cumulative distribution P(X≤k) of trial data.
    /// Optionally calculates the lower cumulative distribution P(X≤k) of trial data, assuming that a group of a given number of trial successes is considered a single success
    /// in the context of the distribution.
    /// Example: If 'groupSuccessCount' = 2, then the total number of trials is divided by 2, and any combination of 2 successful trials is considered a single success
    /// in the context of the distribution.
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="probability"></param>
    /// <param name="groupSuccessCount">The number of grouped trial successes that count as a success. Default is 1 (one trial success equals one binomial success)</param>
    /// <returns>A cumulative distribution P(X≤k) of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetCumulativeDistribution(int numberOfTrials, double probability, int groupSuccessCount = 1)
    {
        // Validate parameters
        if (numberOfTrials <= 0)
        {
            Debug.WriteLine("CumulativeDistribution() | Number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("CumulativeDistribution() | Probability is less than or equal to 0.");

            // The probability of 0 successes should be 1, all other probabilities should be 0.
            // So the lower cumulative probability for all entries should be 1.
            var adjustedDistribution = new List<BinomialOutcome>
            {
                new(0, 1)
            };

            for (var k = 1; k <= numberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 1));
            }

            return adjustedDistribution;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("CumulativeDistribution() | Probability is greater than or equal to 1.");

            // All probabilities should be 0, except the probability of all successes should be 1.
            var adjustedDistribution = new List<BinomialOutcome>();

            for (var k = 0; k <= numberOfTrials - 1; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            adjustedDistribution.Add(new BinomialOutcome(numberOfTrials, 1));

            return adjustedDistribution;
        }

        if (groupSuccessCount <= 0)
        {
            Debug.WriteLine("CumulativeDistribution() | Group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (groupSuccessCount > numberOfTrials)
        {
            Debug.WriteLine("CumulativeDistribution() | Group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Cumulative, numberOfTrials, probability, groupSuccessCount);
    }

    /// <summary>
    /// Calculates the lower cumulative distribution P(X≤k) of trial data using a variable group success count
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="probability"></param>
    /// <param name="minGroupSuccessCount">The minimum number of grouped trial successes that count as a success.</param>
    /// <param name="maxGroupSuccessCount">The maximum number of grouped trial successes that count as a success.</param>
    /// <returns>A cumulative distribution P(X≤k) of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetCumulativeDistribution(int numberOfTrials, double probability, int minGroupSuccessCount, int maxGroupSuccessCount)
    {
        // Validate parameters
        if (numberOfTrials <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Probability is less than or equal to 0.");

            // The probability of 0 successes should be 1, all other probabilities should be 0.
            // So the lower cumulative probability for all entries should be 1.
            var adjustedDistribution = new List<BinomialOutcome>
            {
                new(0, 1)
            };

            for (var k = 1; k <= numberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 1));
            }

            return adjustedDistribution;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Probability is greater than or equal to 1.");

            // All probabilities should be 0, except the probability of all successes should be 1.
            var adjustedDistribution = new List<BinomialOutcome>();

            for (var k = 0; k <= numberOfTrials - 1; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            adjustedDistribution.Add(new BinomialOutcome(numberOfTrials, 1));

            return adjustedDistribution;
        }

        if (minGroupSuccessCount <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Minimum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxGroupSuccessCount <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Maximum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxGroupSuccessCount)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Minimum group success count is greater than maximum group success count.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > numberOfTrials)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Minimum group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Cumulative, numberOfTrials, probability, minGroupSuccessCount, maxGroupSuccessCount);
    }

    /// <summary>
    /// Calculates the combined lower cumulative distribution P(X≤k) of trial data using a variable number of trials.
    /// Optionally calculates the lower cumulative distribution P(X≤k) of trial data, assuming that a group of a given number of trial successes is considered a single success
    /// in the context of the distribution.
    /// Example: If 'groupSuccessCount' = 2, then the total number of trials is divided by 2, and any combination of 2 successful trials is considered a single success
    /// in the context of the distribution.
    /// </summary>
    /// <param name="minNumberOfTrials">The minimum nunmber of trials in the process.</param>
    /// <param name="maxNumberOfTrials">The maximum number of trials in the process.</param>
    /// <param name="probability">The probability of success for a single trial.</param>
    /// <param name="groupSuccessCount">The number of grouped trial successes that count as a success. Default is 1 (one trial success equals one binomial success)</param>
    /// <returns>A cumulative distribution P(X≤k) of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetCumulativeDistribution(int minNumberOfTrials, int maxNumberOfTrials, double probability, int groupSuccessCount = 1)
    {
        // Validate parameters
        if (minNumberOfTrials <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Minimum number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxNumberOfTrials <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Maximum number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minNumberOfTrials > maxNumberOfTrials)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Min number of trials is greater than max number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Probability is less than or equal to 0.");

            // The probability of 0 successes should be 1, all other probabilities should be 0.
            // So the lower cumulative probability for all entries should be 1.
            var adjustedDistribution = new List<BinomialOutcome>
            {
                new(0, 1)
            };

            for (var k = 1; k <= maxNumberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 1));
            }

            return adjustedDistribution;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Probability is greater than or equal to 1.");

            // All probabilities should be 0, except the probability of all successes should be 1.
            var adjustedDistribution = new List<BinomialOutcome>();

            for (var k = 0; k <= maxNumberOfTrials - 1; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            adjustedDistribution.Add(new BinomialOutcome(maxNumberOfTrials, 1));

            return adjustedDistribution;
        }

        if (groupSuccessCount <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (groupSuccessCount > maxNumberOfTrials)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Cumulative, minNumberOfTrials, maxNumberOfTrials, probability, groupSuccessCount);
    }

    /// <summary>
    /// Calculates the combined lower cumulative distribution P(X≤k) of trial data using a variable number of trials and a variable group success count.
    /// </summary>
    /// <param name="minNumberOfTrials">The minimum nunmber of trials in the process.</param>
    /// <param name="maxNumberOfTrials">The maximum number of trials in the process.</param>
    /// <param name="probability">The probability of success for a single trial.</param>
    /// <param name="minGroupSuccessCount">The minimum number of grouped trial successes that count as a success.</param>
    /// <param name="maxGroupSuccessCount">The maximum number of grouped trial successes that count as a success.</param>
    /// <returns>A cumulative distribution P(X≤k) of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetCumulativeDistribution(int minNumberOfTrials, int maxNumberOfTrials, double probability, int minGroupSuccessCount, int maxGroupSuccessCount)
    {
        // Validate parameters
        if (minNumberOfTrials <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Minimum number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxNumberOfTrials <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Maximum number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minNumberOfTrials > maxNumberOfTrials)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Min number of trials is greater than max number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Probability is less than or equal to 0.");

            // The probability of 0 successes should be 1, all other probabilities should be 0.
            // So the lower cumulative probability for all entries should be 1.
            var adjustedDistribution = new List<BinomialOutcome>
            {
                new(0, 1)
            };

            for (var k = 1; k <= maxNumberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 1));
            }

            return adjustedDistribution;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Probability is greater than or equal to 1.");

            // All probabilities should be 0, except the probability of all successes should be 1.
            var adjustedDistribution = new List<BinomialOutcome>();

            for (var k = 0; k <= maxNumberOfTrials - 1; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            adjustedDistribution.Add(new BinomialOutcome(maxNumberOfTrials, 1));

            return adjustedDistribution;
        }

        if (minGroupSuccessCount <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Minimum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxGroupSuccessCount <= 0)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Maximum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxGroupSuccessCount)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Minimum group success count is greater than maximum group success count.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxNumberOfTrials)
        {
            Debug.WriteLine("LowerCumulativeDistribution() | Minimum group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Cumulative, minNumberOfTrials, maxNumberOfTrials, probability, minGroupSuccessCount, maxGroupSuccessCount);
    }

    /// <summary>
    /// Gets the survivor function distribution P(X≥k) of trial data.
    /// Optionally calculates the upper cumulative distribution P(X≥k) of trial data, assuming that a group of a given number of trial successes is considered a single
    /// success in the context of the distribution.
    /// Example: If 'groupSuccessCount' = 2, then the total number of trials is divided by 2, and any combination of 2 successful trials is considered a single success
    /// in the context of the distribution.
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="probability"></param>
    /// <param name="groupSuccessCount">The number of grouped trial successes that count as a success. Default is 1 (one trial success equals one binomial success)</param>
    /// <returns>A survivor function distribution P(X≥k) of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetSurvivorDistribution(int numberOfTrials, double probability, int groupSuccessCount = 1)
    {
        // Validate parameters
        if (numberOfTrials < 1)
        {
            Debug.WriteLine("SurvivorDistribution() | Number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Probability is less than or equal to 0.");

            // The probability of getting greater than 0 is 0%
            var adjustedDistribution = new List<BinomialOutcome>
            {
                // If including k=0, the probability should be 1.
                new(0, 1)
            };

            // Probabilities for all other values of k should be 0.
            for (var k = 1; k <= numberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            return adjustedDistribution;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("SurvivorDistribution() | Probability is greater than or equal to 1.");

            // All probabilities should be 0, except the probability of all successes should be 1.
            // So the upper cumulative distribution should all be ones.
            var adjustedDistribution = new List<BinomialOutcome>();

            for (var k = 0; k <= numberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 1));
            }

            return adjustedDistribution;
        }

        if (groupSuccessCount < 1)
        {
            Debug.WriteLine("SurvivorDistribution() | Group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (groupSuccessCount > numberOfTrials)
        {
            Debug.WriteLine("SurvivorDistribution() | Group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Survivor, numberOfTrials, probability, groupSuccessCount);
    }

    /// <summary>
    /// Gets the survivor function distribution P(X≥k) of trial data using a variable group success count
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="probability"></param>
    /// <param name="minGroupSuccessCount">The minimum number of grouped trial successes that count as a success.</param>
    /// <param name="maxGroupSuccessCount">The maximum number of grouped trial successes that count as a success.</param>
    /// <returns>A survivor function distribution P(X≥k) of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetSurvivorDistribution(int numberOfTrials, double probability, int minGroupSuccessCount, int maxGroupSuccessCount)
    {
        // Validate parameters
        if (numberOfTrials <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Probability is less than or equal to 0.");

            // The probability of getting greater than 0 is 0%
            var adjustedDistribution = new List<BinomialOutcome>
            {
                // If including k=0, the probability should be 1.
                new(0, 1)
            };

            // Probabilities for all other values of k should be 0.
            for (var k = 1; k <= numberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            return adjustedDistribution;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("SurvivorDistribution() | Probability is greater than or equal to 1.");

            // All probabilities should be 0, except the probability of all successes should be 1.
            // So the upper cumulative distribution should all be ones.
            var adjustedDistribution = new List<BinomialOutcome>();

            for (var k = 0; k <= numberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 1));
            }

            return adjustedDistribution;
        }

        if (minGroupSuccessCount <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Minimum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxGroupSuccessCount <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Maximum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxGroupSuccessCount)
        {
            Debug.WriteLine("SurvivorDistribution() | Minimum group success count is greater than maximum group success count.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > numberOfTrials)
        {
            Debug.WriteLine("SurvivorDistribution() | Minimum group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Survivor, numberOfTrials, probability, minGroupSuccessCount, maxGroupSuccessCount);
    }

    /// <summary>
    /// Gets the combined survivor function distribution P(X≥k) of trial data using a variable number of trials.
    /// Optionally calculates the upper cumulative distribution P(X≥k) of trial data, assuming that a group of a given number of trial successes is considered a single
    /// success in the context of the distribution.
    /// Example: If 'groupSuccessCount' = 2, then the total number of trials is divided by 2, and any combination of 2 successful trials is considered a single success
    /// in the context of the distribution.
    /// </summary>
    /// <param name="minNumberOfTrials">The minimum nunmber of trials in the process.</param>
    /// <param name="maxNumberOfTrials">The maximum number of trials in the process.</param>
    /// <param name="probability">The probability of success for a single trial.</param>
    /// <param name="groupSuccessCount">The number of grouped trial successes that count as a success. Default is 1 (one trial success equals one binomial success)</param>
    /// <returns>A survivor function distribution P(X≥k) of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetSurvivorDistribution(int minNumberOfTrials, int maxNumberOfTrials, double probability, int groupSuccessCount = 1)
    {
        // Validate parameters
        if (minNumberOfTrials <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Minimum number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxNumberOfTrials <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Maximum number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minNumberOfTrials > maxNumberOfTrials)
        {
            Debug.WriteLine("SurvivorDistribution() | Min number of trials is greater than max number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Probability is less than or equal to 0.");

            // The probability of getting greater than 0 is 0%
            var adjustedDistribution = new List<BinomialOutcome>
            {
                // If including k=0, the probability should be 1.
                new(0, 1)
            };

            // Probabilities for all other values of k should be 0.
            for (var k = 1; k <= maxNumberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            return adjustedDistribution;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("SurvivorDistribution() | Probability is greater than or equal to 1.");

            // All probabilities should be 0, except the probability of all successes should be 1.
            // So the upper cumulative distribution should all be ones.
            var adjustedDistribution = new List<BinomialOutcome>();

            for (var k = 0; k <= maxNumberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 1));
            }

            return adjustedDistribution;
        }

        if (groupSuccessCount < 1)
        {
            Debug.WriteLine("SurvivorDistribution() | Group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (groupSuccessCount > maxNumberOfTrials)
        {
            Debug.WriteLine("SurvivorDistribution() | Group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Survivor, minNumberOfTrials, maxNumberOfTrials, probability, groupSuccessCount);
    }

    /// <summary>
    /// Gets the combined survivor function distribution P(X≥k) of trial data using a variable number of trials and a variable group success count.
    /// </summary>
    /// <param name="minNumberOfTrials">The minimum nunmber of trials in the process.</param>
    /// <param name="maxNumberOfTrials">The maximum number of trials in the process.</param>
    /// <param name="probability">The probability of success for a single trial.</param>
    /// <param name="minGroupSuccessCount">The minimum number of grouped trial successes that count as a success.</param>
    /// <param name="maxGroupSuccessCount">The maximum number of grouped trial successes that count as a success.</param>
    /// <returns>A survivor function distribution P(X≥k) of trial results and their respective probabilities.</returns>
    public static List<BinomialOutcome> GetSurvivorDistribution(int minNumberOfTrials, int maxNumberOfTrials, double probability, int minGroupSuccessCount, int maxGroupSuccessCount)
    {
        // Validate parameters
        if (minNumberOfTrials <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Minimum number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxNumberOfTrials <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Maximum number of trials is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minNumberOfTrials > maxNumberOfTrials)
        {
            Debug.WriteLine("SurvivorDistribution() | Min number of trials is greater than max number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        if (probability <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Probability is less than or equal to 0.");

            // The probability of getting greater than 0 is 0%
            var adjustedDistribution = new List<BinomialOutcome>
            {
                // If including k=0, the probability should be 1.
                new(0, 1)
            };

            // Probabilities for all other values of k should be 0.
            for (var k = 1; k <= maxNumberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 0));
            }

            return adjustedDistribution;
        }

        if (probability >= 1)
        {
            Debug.WriteLine("SurvivorDistribution() | Probability is greater than or equal to 1.");

            // All probabilities should be 0, except the probability of all successes should be 1.
            // So the upper cumulative distribution should all be ones.
            var adjustedDistribution = new List<BinomialOutcome>();

            for (var k = 0; k <= maxNumberOfTrials; k++)
            {
                adjustedDistribution.Add(new BinomialOutcome(k, 1));
            }

            return adjustedDistribution;
        }

        if (minGroupSuccessCount <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Minimum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (maxGroupSuccessCount <= 0)
        {
            Debug.WriteLine("SurvivorDistribution() | Maximum group success count is less than 1.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxGroupSuccessCount)
        {
            Debug.WriteLine("SurvivorDistribution() | Minimum group success count is greater than maximum group success count.");
            return [new BinomialOutcome(0, 1)];
        }

        if (minGroupSuccessCount > maxNumberOfTrials)
        {
            Debug.WriteLine("SurvivorDistribution() | Minimum group success count is greater than the total number of trials.");
            return [new BinomialOutcome(0, 1)];
        }

        // Create distribution
        return CreateDistribution(DistributionTypes.Survivor, minNumberOfTrials, maxNumberOfTrials, probability, minGroupSuccessCount, maxGroupSuccessCount);
    }

    /// <summary>
    /// Calculates the mean value of a probability distribution.
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="probability"></param>
    /// <returns></returns>
    public static double GetMeanOfDistribution(int numberOfTrials, double probability)
    {
        if (numberOfTrials < 1)
        {
            Debug.WriteLine("GetMean() | Number of trials is less than 1. Returning 0 ...");
            return 0;
        }

        if (probability <= 0)
        {
            Debug.WriteLine("GetMean() | Probability is less or equal to 0. Returning 0 ...");
            return 0;
        }

        return numberOfTrials * probability;
    }

    /// <summary>
    /// Calculates the variance of a distribution
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="probability"></param>
    /// <returns>A double value containing the variance of a distribution</returns>
    public static double GetVarianceOfDistribution(int numberOfTrials, double probability)
    {
        if (numberOfTrials < 0)
        {
            Debug.WriteLine("GetStandardDeviation() | Number of trials is less than 0. Returning 0 ...");
            return 0;
        }

        if (probability < 0)
        {
            Debug.WriteLine("GetStandardDeviation() | Probability is less than 0. Returning 0 ...");
            return 0;
        }

        if (probability == 1)
        {
            Debug.WriteLine("GetStandardDeviation() | Probability is 1. Returning 0 ...");
            return 0;
        }

        return numberOfTrials * probability * (1 - probability);
    }

    /// <summary>
    /// Calculates the standard deviation of a distribution.
    /// </summary>
    /// <param name="numberOfTrials"></param>
    /// <param name="probability"></param>
    /// <returns>A double value containing hte standard deviation of a distribution</returns>
    public static double GetStandardDeviationOfDistribution(int numberOfTrials, double probability)
        => Math.Sqrt(GetVarianceOfDistribution(numberOfTrials, probability));

    /// <summary>
    /// Calculates the variance in the number of successes when the number of trials is also variable
    /// Var(X) = E[N] * p * (1 - p) + Var(N) * p^2
    /// </summary>
    /// <param name="expectedNumberOfTrials"></param>
    /// <param name="varianceOfNumberOfTrials"></param>
    /// <param name="probability"></param>
    /// <returns>A double value contining the combined variance of successes</returns>
    public static double GetCombinedVarianceOfDistribution(
        int expectedNumberOfTrials,
        double varianceOfNumberOfTrials,
        double probability)
        => (expectedNumberOfTrials * probability * (1 - probability)) + (varianceOfNumberOfTrials * Math.Pow(probability, 2));

    /// <summary>
    /// Calculates the standard deviation of the number of successes when the number of trials is also variable
    /// </summary>
    /// <param name="expectedNumberOfTrials"></param>
    /// <param name="varianceOfNumberOfTrials"></param>
    /// <param name="probability"></param>
    /// <returns>A double value contining the combined standard deviation of successes</returns>
    public static double GetCombinedStandardDeviationOfDistribution(
        int expectedNumberOfTrials,
        double varianceOfNumberOfTrials,
        double probability)
        => Math.Sqrt(GetCombinedVarianceOfDistribution(
            expectedNumberOfTrials,
            varianceOfNumberOfTrials,
            probability));

    /// <summary>
    /// Calculates the factorial of a positive integer.
    /// Factorials are denoted by the syntax "n!".
    /// </summary>
    /// <param name="number">
    /// The integer value to perform the factorial calculation on.
    /// The method will use the absolute value of the integer, in case the user passes in a negative value.
    /// </param>
    /// <returns>An int containing the factorial of the passed in value.</returns>
    public static BigInteger Factorial(int number)
    {
        // Throw error if given a negative number
        ArgumentOutOfRangeException.ThrowIfNegative(number);

        // Return 1 if given 0
        if (number == 0)
        {
            return 1;
        }

        // Multiply together all integers from 1 to the passed in number.
        BigInteger result = 1;

        for (var i = 1; i <= number; i++)
        {
            result *= i;
        }

        return result;
    }
}
