namespace BlazorStandards.App.Models;

/// <summary>A plan the signup form offers.</summary>
public enum Plan
{
    Starter,
    Growth,
    Enterprise,
}

/// <summary>
/// What the signup form holds while a visitor fills it in.
/// </summary>
/// <remarks>
/// Mutable, because a form binds to it. Plan stays nullable so an untouched
/// select reads as unchosen rather than defaulting to the first option, which
/// would let a visitor submit a plan they never picked.
/// </remarks>
public sealed class SignupForm
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Plan? Plan { get; set; }

    public int Seats { get; set; } = SeatDefaults.Minimum;

    public string Notes { get; set; } = string.Empty;

    public bool AcceptTerms { get; set; }
}

/// <summary>The seat bounds the form offers.</summary>
public static class SeatDefaults
{
    public const int Minimum = 1;
    public const int Maximum = 500;
}

/// <summary>
/// The problems a submitted form carries, keyed by the field each belongs to.
/// </summary>
public sealed record SignupProblems(IReadOnlyDictionary<string, string> ByField)
{
    public bool Any => ByField.Count > 0;
}
