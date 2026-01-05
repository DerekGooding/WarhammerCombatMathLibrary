using System.Diagnostics;
using System.Numerics;

namespace WarhammerCombatMath.Tests;

/// <summary>
/// Tests the Statistics class.
/// </summary>
[TestClass]
public sealed class Statistics_Test
{
    #region Unit Tests - ProbabilityOfSuccess()

    /// <summary>
    /// Tests the case where the numberOfPossibleResults argument is out of range.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfSuccess_NumberOfPossibleResultsLessThanOrEqualTo0() => Assert.AreEqual(0, GetProbabilityOfSuccess(-1, 0));

    /// <summary>
    /// Tests the case where the numberOfSuccessfulResults argument is out of range.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfSuccess_NumberOfSuccessfulResultsLessThanOrEqualTo0() => Assert.AreEqual(0, GetProbabilityOfSuccess(1, -1));

    /// <summary>
    /// Tests the case where the numberOfSuccessfulResults argument is greater than the numberOfPossibleResults argument.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfSuccess_SuccessfulResultsGreaterThanPossibleResults() => Assert.AreEqual(1, GetProbabilityOfSuccess(1, 2));

    /// <summary>
    /// Tests the ProbabilityOfSuccess() method with given parameters.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfSuccess_TestParams1() => Assert.AreEqual(0.5, GetProbabilityOfSuccess(2, 1));

    /// <summary>
    /// Tests the ProbabilityOfSuccess() method with given parameters.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfSuccess_TestParams2() => Assert.AreEqual(0.25, GetProbabilityOfSuccess(4, 1));

    /// <summary>
    /// Tests the ProbabilityOfSuccess() method with given parameters.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfSuccess_TestParams3() => Assert.AreEqual(0.7, GetProbabilityOfSuccess(10, 7));

    #endregion

    #region Unit Tests - AverageResult()

    /// <summary>
    /// Tests the AverageResult() method when the input parameter is less than or equal to 0.
    /// </summary>
    [TestMethod]
    public void AverageResult_PossibleResultsLessThanOrEqualTo0()
    {
        const int expected = 0;
        var actual = GetMeanResult(0);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the AverageResult() method with given params.
    /// </summary>
    [TestMethod]
    public void AverageResult_TestParams1()
    {
        const int expected = 2;
        const int numberOfResults = 3;
        var actual = GetMeanResult(numberOfResults);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the AverageResult() method with given params.
    /// </summary>
    [TestMethod]
    public void AverageResult_TestParams2()
    {
        const int expected = 4;
        const int numberOfResults = 6;
        var actual = GetMeanResult(numberOfResults);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the AverageResult() method with given params.
    /// </summary>
    [TestMethod]
    public void AverageResult_TestParams3()
    {
        const int expected = 6;
        const int numberOfResults = 10;
        var actual = GetMeanResult(numberOfResults);
        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region Unit Tests - Mean()

    /// <summary>
    /// Tests the case where the number of trials is less than 1.
    /// </summary>
    [TestMethod]
    public void Mean_NumberOfTrialsLessThan1() => Assert.AreEqual(0, GetMeanOfDistribution(0, 1));

    /// <summary>
    /// Tests the case where the probability is less than 0.
    /// </summary>
    [TestMethod]
    public void Mean_ProbabilityLessThan0() => Assert.AreEqual(0, GetMeanOfDistribution(1, -1));

    /// <summary>
    /// Tests the method with given parameters.
    /// </summary>
    [TestMethod]
    public void Mean_TestParams1() => Assert.AreEqual(0.5, GetMeanOfDistribution(1, 0.5));

    /// <summary>
    /// Tests the method with given parameters.
    /// </summary>
    [TestMethod]
    public void Mean_TestParams2() => Assert.AreEqual(5, GetMeanOfDistribution(10, 0.5));

    /// <summary>
    /// Tests the method with given parameters.
    /// </summary>
    [TestMethod]
    public void Mean_TestParams3() => Assert.AreEqual(25, GetMeanOfDistribution(100, 0.25));

    #endregion

    #region Unit Tests - StandardDeviation()

    /// <summary>
    /// Tests the case where the number of trials is less than 1.
    /// </summary>
    [TestMethod]
    public void StandardDeviation_NumberOfTrialsLessThan1() => Assert.AreEqual(0, GetStandardDeviationOfDistribution(0, 1));

    /// <summary>
    /// Tests the case where the probability is less than 0.
    /// </summary>
    [TestMethod]
    public void StandardDeviation_ProbabilityLessThan0() => Assert.AreEqual(0, GetStandardDeviationOfDistribution(1, -1));

    /// <summary>
    /// Tests the method with given parameters.
    /// </summary>
    [TestMethod]
    public void StandardDeviation_TestParams1() => Assert.AreEqual(0.5, GetMeanOfDistribution(1, 0.5));

    /// <summary>
    /// Tests the method with given parameters.
    /// </summary>
    [TestMethod]
    public void StandardDeviation_TestParams2() => Assert.AreEqual(1.58, Math.Round(GetStandardDeviationOfDistribution(10, 0.5), 2));

    /// <summary>
    /// Tests the method with given parameters.
    /// </summary>
    [TestMethod]
    public void StandardDeviation_TestParams3() => Assert.AreEqual(4.33, Math.Round(GetStandardDeviationOfDistribution(100, 0.25), 2));

    #endregion

    #region Unit Tests - BinomialCoefficient()

    /// <summary>
    /// Tests the case where the totalPopulation argument is out of range.
    /// </summary>
    [TestMethod]
    public void BinomialCoefficient_PopulationLessThan0() => Assert.AreEqual(0, BinomialCoefficient(-1, 1));

    /// <summary>
    /// Tests the case where the combinationSize argument is out of range.
    /// </summary>
    [TestMethod]
    public void BinomialCoefficient_CombinationSizeLessThan0() => Assert.AreEqual(0, BinomialCoefficient(1, -1));

    /// <summary>
    /// Tests the case where combinationSize is bigger than totalPopulation.
    /// </summary>
    [TestMethod]
    public void BinomialCoefficient_CombinationSizeBiggerThanTotalPopulation() => Assert.AreEqual(0, BinomialCoefficient(1, 2));

    /// <summary>
    /// Tests the probability mass function with given inputs.
    /// </summary>
    [TestMethod]
    public void BinomialCoefficient_TestParams1() => Assert.AreEqual(1, BinomialCoefficient(1, 1));

    /// <summary>
    /// Tests the probability mass function with given inputs.
    /// </summary>
    [TestMethod]
    public void BinomialCoefficient_TestParams2() => Assert.AreEqual(252, BinomialCoefficient(10, 5));

    /// <summary>
    /// Tests the probability mass function with given large inputs.
    /// </summary>
    [TestMethod]
    public void BinomialCoefficient_TestBigParams() => Assert.AreEqual(BigInteger.Parse("18053528883775"), BinomialCoefficient(50, 32));

    #endregion

    #region Unit Tests - ProbabilityOfMultipleSuccesses()

    /// <summary>
    /// Tests the case where the probability argument is negative.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfMultipleSuccesses_ProbabilityLessThan0() => Assert.AreEqual(0, ProbabilityOfMultipleSuccesses(-1, 1));

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfMultipleSuccesses_ProbabilityGreaterThan1() => Assert.AreEqual(1, ProbabilityOfMultipleSuccesses(2, 1));

    /// <summary>
    /// Tests the case where the numberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfMultipleSuccesses_NumberOfSuccessesLessThan0() => Assert.AreEqual(0, ProbabilityOfMultipleSuccesses(0.5, -1));

    /// <summary>
    /// Tests the probability mass function with given large inputs.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfMultipleSuccesses_TestParams1() => Assert.AreEqual(0.1, ProbabilityOfMultipleSuccesses(0.1, 1));

    /// <summary>
    /// Tests the probability mass function with given inputs.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfMultipleSuccesses_TestParams2() => Assert.AreEqual(0.25, ProbabilityOfMultipleSuccesses(0.5, 2));

    /// <summary>
    /// Tests the probability mass function with given inputs.
    /// </summary>
    [TestMethod]
    public void ProbabilityOfMultipleSuccesses_TestParams3() => Assert.AreEqual(0.59, Math.Round(ProbabilityOfMultipleSuccesses(0.9, 5), 2));

    #endregion

    #region Unit Tests - ProbabilityMassFunction()

    /// <summary>
    /// Tests the case where the numberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void ProbabilityMassFunction_NumberOfTrialsLessThan1() => Assert.AreEqual(0, ProbabilityMassFunction(0, 1, 0.5));

    /// <summary>
    /// Tests the case where the numberOfSuccesses argument is out of range.
    /// </summary>
    [TestMethod]
    public void ProbabilityMassFunction_NumberOfSuccessesLessThan0() => Assert.AreEqual(0, ProbabilityMassFunction(1, -1, 0.5));

    /// <summary>
    /// Tests the case where the number of successes is greater than the number of trials.
    /// </summary>
    [TestMethod]
    public void ProbabilityMassFunction_NumberOfSuccessesGreaterThanNumberOfTrials() => Assert.AreEqual(0, ProbabilityMassFunction(1, 2, 0.5));

    /// <summary>
    /// Tests the case where the probability argument is negative.
    /// </summary>
    [TestMethod]
    public void ProbabilityMassFunction_ProbabilityLessThan0() => Assert.AreEqual(0, ProbabilityMassFunction(1, 1, -1));

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void ProbabilityMassFunction_ProbabilityGreaterThan1() => Assert.AreEqual(1, ProbabilityMassFunction(1, 1, 2));

    /// <summary>
    /// Tests the probability mass function with given inputs.
    /// </summary>
    [TestMethod]
    public void ProbabilityMassFunction_TestParams1() => Assert.AreEqual(0.5, ProbabilityMassFunction(1, 1, 0.5));

    /// <summary>
    /// Tests the probability mass function with given inputs.
    /// </summary>
    [TestMethod]
    public void ProbabilityMassFunction_TestParams2() => Assert.AreEqual(0.0584, Math.Round(ProbabilityMassFunction(10, 5, 0.25), 4));

    /// <summary>
    /// Tests the probability mass function with given large inputs.
    /// </summary>
    [TestMethod]
    public void ProbabilityMassFunction_BigParams() => Assert.AreEqual(0.0160, Math.Round(ProbabilityMassFunction(50, 32, 0.5), 4));

    #endregion

    #region Unit Tests - BinomialDistribution()

    /// <summary>
    /// Tests the case where the numberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_NumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetBinomialDistribution(0, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case for the variable trials override where the minNumberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_VariableTrials_MinNumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetBinomialDistribution(0, 1, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case for the variable trials override where the maxNumberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_VariableTrials_MaxNumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetBinomialDistribution(1, 0, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is negative.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_ProbabilityLessThanOrEqualTo0()
    {
        var expected = new List<BinomialOutcome>()
        {
            new(0, 1),
            new(1,0),
            new(2,0),
            new(3,0)
        };
        var actual = GetBinomialDistribution(3, -1);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_ProbabilityGreaterThanOrEqualTo1()
    {
        var expected = new List<BinomialOutcome>()
        {
            new(0, 0),
            new(1, 0),
            new(2, 0),
            new(3, 1)
        };

        var actual = GetBinomialDistribution(3, 1);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_GroupSuccessCountLessThan1()
    {
        var expected = new List<BinomialOutcome> { new(0, 1) };
        var actual = GetBinomialDistribution(3, 0.5, 0);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_GroupSuccessCountGreaterThanNumberOfTrials()
    {
        var expected = new List<BinomialOutcome> { new(0, 1) };
        var actual = GetBinomialDistribution(3, 0.5, 5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the method with given large inputs.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_VariableTrials()
    {
        const int minNumberOfTrials = 1;
        const int maxNumberOfTrials = 3;
        const double probability = 0.5;
        var expected = new List<BinomialOutcome>()
        {
            new(0, 0.2917),
            new(1, 0.4583),
            new(2, 0.2083),
            new(3, 0.0417)
        };

        var actual = GetBinomialDistribution(minNumberOfTrials, maxNumberOfTrials, probability);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    #endregion

    #region Unit Tests - LowerCumulativeDistribution

    /// <summary>
    /// Tests the case where the numberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void LowerCumulativeDistribution_NumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetCumulativeDistribution(0, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case for the variable trials override where the minNumberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void LowerCumulativeDistribution_VariableTrials_MinNumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetCumulativeDistribution(0, 1, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case for the variable trials override where the maxNumberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void LowerCumulativeDistribution_VariableTrials_MaxNumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetCumulativeDistribution(1, 0, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is negative.
    /// </summary>
    [TestMethod]
    public void LowerCumulativeDistribution_ProbabilityLessThanOrEqualTo0()
    {
        var expected = new List<BinomialOutcome>()
        {
            new(0, 1),
            new(1,1),
            new(2,1),
            new(3,1)
        };
        var actual = GetCumulativeDistribution(3, -1);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void LowerCumulativeDistribution_ProbabilityGreaterThanOrEqualTo1()
    {
        var expected = new List<BinomialOutcome>()
        {
            new(0, 0),
            new(1, 0),
            new(2, 0),
            new(3, 1)
        };

        var actual = GetCumulativeDistribution(3, 1);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void LowerCumulativeDistribution_GroupSuccessCountLessThan1()
    {
        var expected = new List<BinomialOutcome> { new(0, 1) };
        var actual = GetCumulativeDistribution(3, 0.5, 0);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void LowerCumulativeDistribution_GroupSuccessCountGreaterThanNumberOfTrials()
    {
        var expected = new List<BinomialOutcome> { new(0, 1) };
        var actual = GetCumulativeDistribution(3, 0.5, 5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the method with given large inputs.
    /// </summary>
    [TestMethod]
    public void CumulativeDistribution_VariableTrials()
    {
        const int minNumberOfTrials = 1;
        const int maxNumberOfTrials = 3;
        const double probability = 0.5;
        var expected = new List<BinomialOutcome>()
        {
            new(0, 0.2917),
            new(1, 0.7500),
            new(2, 0.9583),
            new(3, 1)
        };

        var actual = GetCumulativeDistribution(minNumberOfTrials, maxNumberOfTrials, probability);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    #endregion

    #region Unit Tests - SurvivorDistribution

    /// <summary>
    /// Tests the case where the numberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_NumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetSurvivorDistribution(0, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case for the variable trials override where the minNumberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_VariableTrials_MinNumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetSurvivorDistribution(0, 1, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case for the variable trials override where the maxNumberOfTrials argument is out of range.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_VariableTrials_MaxNumberOfTrialsLessThan1()
    {
        var expected = new List<BinomialOutcome>() { new(0, 1) };
        var actual = GetSurvivorDistribution(1, 0, 0.5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is negative.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_ProbabilityLessThanOrEqualTo0()
    {
        var expected = new List<BinomialOutcome>()
        {
            new(0, 1),
            new(1, 0),
            new(2, 0),
            new(3, 0)
        };

        var actual = GetSurvivorDistribution(3, -1);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_ProbabilityGreaterThanOrEqualTo1()
    {
        var expected = new List<BinomialOutcome>()
        {
            new(0, 1),
            new(1, 1),
            new(2, 1),
            new(3, 1)
        };

        var actual = GetSurvivorDistribution(3, 1);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_GroupSuccessCountLessThan1()
    {
        var expected = new List<BinomialOutcome> { new(0, 1) };
        var actual = GetSurvivorDistribution(3, 0.5, 0);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the probability argument is greater than 1.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_GroupSuccessCountGreaterThanNumberOfTrials()
    {
        var expected = new List<BinomialOutcome> { new(0, 1) };
        var actual = GetSurvivorDistribution(3, 0.5, 5);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Tests the case where the number of trials is variable.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_VariableTrials()
    {
        const int minNumberOfTrials = 1;
        const int maxNumberOfTrials = 3;
        const double probability = 0.5;
        var expected = new List<BinomialOutcome>()
        {
            new(0, 1),
            new(1, 0.7083),
            new(2, 0.2500),
            new(3, 0.0417)
        };

        var actual = GetSurvivorDistribution(minNumberOfTrials, maxNumberOfTrials, probability);

        // Print expected
        Debug.WriteLine("Expected: ");
        foreach (var value in expected)
        {
            Debug.WriteLine(value);
        }

        // Print actual
        Debug.WriteLine("Actual: ");
        foreach (var value in actual)
        {
            Debug.WriteLine(value);
        }

        CollectionAssert.AreEqual(expected, actual);
    }

    #endregion

    #region Unit Tests - Validate Cumulative Distribution Math

    /// <summary>
    /// Tests that a simple cumulative distribution reaches exactly 1.0 at its maximum value.
    /// </summary>
    [TestMethod]
    public void CumulativeDistribution_Simple_MaxValueIsOne()
    {
        var dist = GetCumulativeDistribution(10, 0.5);
        var lastProb = dist[^1].Probability;
        Assert.AreEqual(1.0, lastProb, 0.0001, $"Last probability was {lastProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a cumulative distribution with variable trials reaches exactly 1.0 at its maximum value.
    /// </summary>
    [TestMethod]
    public void CumulativeDistribution_VariableTrials_MaxValueIsOne()
    {
        var dist = GetCumulativeDistribution(1, 10, 0.5);
        var lastProb = dist[^1].Probability;
        Assert.AreEqual(1.0, lastProb, 0.0001, $"Last probability was {lastProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a cumulative distribution with variable group success count reaches exactly 1.0 at its maximum value.
    /// </summary>
    [TestMethod]
    public void CumulativeDistribution_VariableGroupSuccess_MaxValueIsOne()
    {
        var dist = GetCumulativeDistribution(10, 0.5, 2, 3);
        var lastProb = dist[^1].Probability;
        Assert.AreEqual(1.0, lastProb, 0.0001, $"Last probability was {lastProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a cumulative distribution with both variable trials and variable group success count reaches exactly 1.0.
    /// </summary>
    [TestMethod]
    public void CumulativeDistribution_VariableTrialsAndGroup_MaxValueIsOne()
    {
        var dist = GetCumulativeDistribution(1, 10, 0.5, 2, 3);
        var lastProb = dist[^1].Probability;
        Assert.AreEqual(1.0, lastProb, 0.0001, $"Last probability was {lastProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a simple survivor distribution starts at exactly 1.0.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_Simple_FirstValueIsOne()
    {
        var dist = GetSurvivorDistribution(10, 0.5);
        var firstProb = dist[0].Probability;
        Assert.AreEqual(1.0, firstProb, 0.0001, $"First probability was {firstProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a survivor distribution with variable trials starts at exactly 1.0.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_VariableTrials_FirstValueIsOne()
    {
        var dist = GetSurvivorDistribution(1, 10, 0.5);
        var firstProb = dist[0].Probability;
        Assert.AreEqual(1.0, firstProb, 0.0001, $"First probability was {firstProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a survivor distribution with variable group success count starts at exactly 1.0.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_VariableGroupSuccess_FirstValueIsOne()
    {
        var dist = GetSurvivorDistribution(10, 0.5, 2, 3);
        var firstProb = dist[0].Probability;
        Assert.AreEqual(1.0, firstProb, 0.0001, $"First probability was {firstProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a survivor distribution with both variable trials and variable group success count starts at exactly 1.0.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_VariableTrialsAndGroup_FirstValueIsOne()
    {
        var dist = GetSurvivorDistribution(1, 10, 0.5, 2, 3);
        var firstProb = dist[0].Probability;
        Assert.AreEqual(1.0, firstProb, 0.0001, $"First probability was {firstProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a simple binomial distribution sums to exactly 1.0.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_Simple_SumsToOne()
    {
        var dist = GetBinomialDistribution(10, 0.5);
        var sum = dist.Sum(x => x.Probability);
        Assert.AreEqual(1.0, sum, 0.0001, $"Sum was {sum}, expected 1.0");
    }

    /// <summary>
    /// Tests that a binomial distribution with variable trials sums to exactly 1.0.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_VariableTrials_SumsToOne()
    {
        var dist = GetBinomialDistribution(1, 10, 0.5);
        var sum = dist.Sum(x => x.Probability);
        Assert.AreEqual(1.0, sum, 0.0001, $"Sum was {sum}, expected 1.0");
    }

    /// <summary>
    /// Tests that a binomial distribution with variable group success count sums to exactly 1.0.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_VariableGroupSuccess_SumsToOne()
    {
        var dist = GetBinomialDistribution(10, 0.5, 2, 3);
        var sum = dist.Sum(x => x.Probability);
        Assert.AreEqual(1.0, sum, 0.0001, $"Sum was {sum}, expected 1.0");
    }

    /// <summary>
    /// Tests that a binomial distribution with both variable trials and variable group success count sums to exactly 1.0.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_VariableTrialsAndGroup_SumsToOne()
    {
        var dist = GetBinomialDistribution(1, 10, 0.5, 2, 3);
        var sum = dist.Sum(x => x.Probability);
        Assert.AreEqual(1.0, sum, 0.0001, $"Sum was {sum}, expected 1.0");
    }

    /// <summary>
    /// Tests that a cumulative distribution with single groupSuccessCount > 1 reaches exactly 1.0 at its maximum value.
    /// </summary>
    [TestMethod]
    public void CumulativeDistribution_SingleGroupSuccessGreaterThan1_MaxValueIsOne()
    {
        var dist = GetCumulativeDistribution(10, 0.5, 2);
        var lastProb = dist[^1].Probability;
        Assert.AreEqual(1.0, lastProb, 0.0001, $"Last probability was {lastProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a survivor distribution with single groupSuccessCount > 1 starts at exactly 1.0.
    /// </summary>
    [TestMethod]
    public void SurvivorDistribution_SingleGroupSuccessGreaterThan1_FirstValueIsOne()
    {
        var dist = GetSurvivorDistribution(10, 0.5, 2);
        var firstProb = dist[0].Probability;
        Assert.AreEqual(1.0, firstProb, 0.0001, $"First probability was {firstProb}, expected 1.0");
    }

    /// <summary>
    /// Tests that a binomial distribution with single groupSuccessCount > 1 sums to exactly 1.0.
    /// </summary>
    [TestMethod]
    public void BinomialDistribution_SingleGroupSuccessGreaterThan1_SumsToOne()
    {
        var dist = GetBinomialDistribution(10, 0.5, 2);
        var sum = dist.Sum(x => x.Probability);
        Assert.AreEqual(1.0, sum, 0.0001, $"Sum was {sum}, expected 1.0");
    }

    #endregion
}
