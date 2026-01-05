namespace WarhammerCombatMath.Model;

/// <summary>
/// Data object for binomial data.
/// </summary>
public sealed record BinomialOutcome(int Successes, double Probability)
{
    /// <inheritdoc/>
    public override string ToString() =>
        $"P({Successes}) = {Probability:F4}";

    /// <inheritdoc/>
    public bool Equals(BinomialOutcome? other) =>
        other is not null
        && Successes == other.Successes
        && Math.Round(Probability, 4) == Math.Round(other.Probability, 4);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(Successes, Math.Round(Probability, 4));
}
