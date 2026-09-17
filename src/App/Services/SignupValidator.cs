using System.Text.RegularExpressions;
using BlazorStandards.App.Constants;
using BlazorStandards.App.Models;

namespace BlazorStandards.App.Services;

/// <summary>
/// Checks a submitted signup and reports every problem at once.
/// </summary>
/// <remarks>
/// Returning on the first problem would make a visitor correct one field per
/// submission. Every check runs, so the form marks all of them together.
///
/// The test that guards this asserts over a wholly empty form, because a
/// validator that stops at the first problem still passes every single-field
/// test.
/// </remarks>
internal static partial class SignupValidator
{
    private const string EmailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";

    internal static SignupProblems Validate(SignupForm form)
    {
        var problems = new Dictionary<string, string>(StringComparer.Ordinal);

        if (form.FullName.Trim().Length == 0)
        {
            problems[SignupField.FullName] = Copy.Validation.NameRequired;
        }

        var email = form.Email.Trim();
        if (email.Length == 0)
        {
            problems[SignupField.Email] = Copy.Validation.EmailRequired;
        }
        else if (!Email().IsMatch(email))
        {
            problems[SignupField.Email] = Copy.Validation.EmailInvalid;
        }

        if (form.Plan is null)
        {
            problems[SignupField.Plan] = Copy.Validation.PlanRequired;
        }

        if (!form.AcceptTerms)
        {
            problems[SignupField.Terms] = Copy.Validation.TermsRequired;
        }

        return new SignupProblems(problems);
    }

    [GeneratedRegex(EmailPattern)]
    private static partial Regex Email();
}

/// <summary>
/// The keys a problem is filed under. The form reads them to decide which field
/// shows an error, so a typo would hide a message rather than fail.
/// </summary>
internal static class SignupField
{
    internal const string FullName = "fullName";
    internal const string Email = "email";
    internal const string Plan = "plan";
    internal const string Terms = "terms";
}
