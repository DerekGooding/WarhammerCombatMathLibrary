namespace WarhammerCombatMathLibrary.CombatMath;

internal static class Constants
{
    /// <summary>
    /// The number of possible results of rolling a six-sided die.
    /// </summary>
    internal const int POSSIBLE_RESULTS_SIX_SIDED_DIE = 6;

    /// <summary>
    /// A result of 1 on a die is always considered a failure.
    /// </summary>
    internal const int AUTOMATIC_FAIL_RESULT = 1;

    /// <summary>
    /// The maximum result of a die is always considered a success.
    /// </summary>
    internal const int AUTOMATIC_SUCCESS_RESULT = 6;
}
