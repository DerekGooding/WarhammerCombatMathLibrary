namespace WarhammerCombatMathLibrary.Model;

/// <summary>
/// Data object for binomial data.
/// </summary>
public class BinomialOutcome
{
    #region Properties

    /// <summary>
    /// The number of successful trials.
    /// </summary>
    public int Successes { get; set; }

    /// <summary>
    /// The probability of getting the number of successful trials.
    /// </summary>
    public double Probability { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Parameterless constructor. Sets properties to default values.
    /// </summary>
    public BinomialOutcome()
    {
        Successes = 0;
        Probability = 0;
    }

    /// <summary>
    /// Constructs binomial data with given property values.
    /// </summary>
    /// <param name="successes"></param>
    /// <param name="probability"></param>
    public BinomialOutcome(int successes, double probability)
    {
        Successes = successes;
        Probability = probability;
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public override string ToString() => $"P({Successes}) = {Probability:F4}";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is BinomialOutcome other
            && Successes == other.Successes
            && Math.Round(Probability, 4) == Math.Round(other.Probability, 4);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Successes, Probability);

    #endregion
}
