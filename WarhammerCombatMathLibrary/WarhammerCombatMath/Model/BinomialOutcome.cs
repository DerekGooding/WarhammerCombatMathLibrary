namespace WarhammerCombatMath.Model;

/// <summary>
/// Data object for binomial data.
/// </summary>
public class BinomialOutcome : IEquatable<BinomialOutcome>
{
    /// <summary>
    /// The number of successful trials.
    /// </summary>
    public int Successes { get; set; }

    /// <summary>
    /// The probability of getting the number of successful trials.
    /// </summary>
    public double Probability { get; set; }

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

    /// <inheritdoc/>
    public override string ToString() => $"P({Successes}) = {Probability:F4}";


    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as BinomialOutcome);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Successes, Probability);

    public bool Equals(BinomialOutcome? other)
        => other != null
        && Successes == other.Successes
        && Math.Round(Probability, 4) == Math.Round(other.Probability, 4);
}
