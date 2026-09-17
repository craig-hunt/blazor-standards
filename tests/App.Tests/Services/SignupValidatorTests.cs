using BlazorStandards.App.Constants;
using BlazorStandards.App.Models;
using BlazorStandards.App.Services;
using Xunit;

namespace BlazorStandards.App.Tests.Services;

/// <summary>
/// Signup validation, including the rule that every problem reports at once.
/// </summary>
/// <remarks>
/// The all-at-once case is the one worth guarding hardest. A validator that
/// returns on its first problem passes every single-field test here and fails
/// only the empty-form case, which is why that case exists.
/// </remarks>
public sealed class SignupValidatorTests
{
    private const string ValidName = "Dana Whitfield";
    private const string ValidEmail = "dana.whitfield@example.com";
    private const string InvalidEmail = "dana.whitfield.example.com";
    private const string Whitespace = "   ";
    private const int EmptyFormProblems = 4;
    private const int Seats = 12;

    [Fact]
    public void AnEmptyFormReportsEveryProblemAtOnce()
    {
        var problems = SignupValidator.Validate(new SignupForm());

        Assert.Equal(EmptyFormProblems, problems.ByField.Count);
        Assert.Equal(Copy.Validation.NameRequired, problems.ByField[SignupField.FullName]);
        Assert.Equal(Copy.Validation.EmailRequired, problems.ByField[SignupField.Email]);
        Assert.Equal(Copy.Validation.PlanRequired, problems.ByField[SignupField.Plan]);
        Assert.Equal(Copy.Validation.TermsRequired, problems.ByField[SignupField.Terms]);
    }

    /// <summary>
    /// A fresh form starts blank on every field. The optional ones matter as
    /// much as the required ones here: nothing else asserts what Notes holds
    /// before anyone types, so a stray default would reach the summary unseen.
    /// </summary>
    [Fact]
    public void ANewFormStartsEmpty()
    {
        var form = new SignupForm();

        Assert.Empty(form.FullName);
        Assert.Empty(form.Email);
        Assert.Empty(form.Notes);
        Assert.Null(form.Plan);
        Assert.False(form.AcceptTerms);
    }

    [Fact]
    public void ACompleteFormReportsNothing()
    {
        var problems = SignupValidator.Validate(Valid());

        Assert.False(problems.Any);
        Assert.Empty(problems.ByField);
    }

    [Fact]
    public void AWhitespaceNameCountsAsMissing()
    {
        var form = Valid();
        form.FullName = Whitespace;

        var problems = SignupValidator.Validate(form);

        Assert.Equal(Copy.Validation.NameRequired, problems.ByField[SignupField.FullName]);
    }

    [Fact]
    public void AMalformedEmailReportsTheInvalidMessage()
    {
        var form = Valid();
        form.Email = InvalidEmail;

        var problems = SignupValidator.Validate(form);

        Assert.Equal(Copy.Validation.EmailInvalid, problems.ByField[SignupField.Email]);
    }

    [Fact]
    public void AMissingEmailReportsTheRequiredMessageRatherThanInvalid()
    {
        var form = Valid();
        form.Email = string.Empty;

        var problems = SignupValidator.Validate(form);

        Assert.Equal(Copy.Validation.EmailRequired, problems.ByField[SignupField.Email]);
    }

    [Theory]
    [InlineData(Plan.Starter)]
    [InlineData(Plan.Growth)]
    [InlineData(Plan.Enterprise)]
    public void EveryPlanPasses(Plan plan)
    {
        var form = Valid();
        form.Plan = plan;

        Assert.False(SignupValidator.Validate(form).Any);
    }

    [Fact]
    public void AnUnchosenPlanReportsAProblem()
    {
        var form = Valid();
        form.Plan = null;

        var problems = SignupValidator.Validate(form);

        Assert.Equal(Copy.Validation.PlanRequired, problems.ByField[SignupField.Plan]);
    }

    [Fact]
    public void UnacceptedTermsReportAProblem()
    {
        var form = Valid();
        form.AcceptTerms = false;

        var problems = SignupValidator.Validate(form);

        Assert.Equal(Copy.Validation.TermsRequired, problems.ByField[SignupField.Terms]);
    }

    [Fact]
    public void TheSummaryNamesThePersonPlanAndSeats()
    {
        var summary = Copy.Signup.Summary(ValidName, nameof(Plan.Growth), Seats);

        Assert.Contains(ValidName, summary, StringComparison.Ordinal);
        Assert.Contains(nameof(Plan.Growth), summary, StringComparison.Ordinal);
    }

    private static SignupForm Valid() => new()
    {
        FullName = ValidName,
        Email = ValidEmail,
        Plan = Models.Plan.Growth,
        Seats = Seats,
        AcceptTerms = true,
    };
}
