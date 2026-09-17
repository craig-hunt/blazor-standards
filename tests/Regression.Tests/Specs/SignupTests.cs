using BlazorStandards.Regression.Tests.Constants;
using BlazorStandards.Regression.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit;

namespace BlazorStandards.Regression.Tests.Specs;

/// <summary>
/// The signup form, including the rule that every problem shows at once.
/// </summary>
[Collection(ApplicationFixtureDefinition.Name)]
public sealed class SignupTests : PageObjectTest
{
    [Fact]
    public async Task SubmittingAnEmptyFormShowsEveryProblemTogether()
    {
        UseTestIdAttribute();
        await SignupPage.GotoAsync();

        await SignupPage.SubmitEmptyAsync();

        await Expect(SignupPage.FullNameError).ToHaveTextAsync(SuiteData.ValidationMessage.NameRequired);
        await Expect(SignupPage.EmailError).ToHaveTextAsync(SuiteData.ValidationMessage.EmailRequired);
        await Expect(SignupPage.PlanError).ToHaveTextAsync(SuiteData.ValidationMessage.PlanRequired);
        await Expect(SignupPage.TermsError).ToHaveTextAsync(SuiteData.ValidationMessage.TermsRequired);
    }

    [Fact]
    public async Task ARejectedFieldIsMarkedInvalidForAssistiveTechnology()
    {
        UseTestIdAttribute();
        await SignupPage.GotoAsync();

        await SignupPage.SubmitEmptyAsync();

        await Expect(SignupPage.FullName).ToHaveAttributeAsync(SuiteAria.Invalid, SuiteAria.True);
    }

    [Fact]
    public async Task AMalformedEmailReportsTheInvalidMessage()
    {
        UseTestIdAttribute();
        await SignupPage.GotoAsync();

        await SignupPage.Email.FillAsync(SuiteData.InvalidEmail);
        await SignupPage.SubmitEmptyAsync();

        await Expect(SignupPage.EmailError).ToHaveTextAsync(SuiteData.ValidationMessage.EmailInvalid);
    }

    [Fact]
    public async Task ACompleteFormConfirmsTheSignup()
    {
        UseTestIdAttribute();
        await SignupPage.GotoAsync();

        await SignupPage.CompleteAsync(
            SuiteData.FullName,
            SuiteData.Email,
            SuiteData.PlanGrowth,
            SuiteData.Seats,
            SuiteData.Notes);

        await Expect(SignupPage.Confirmation).ToBeVisibleAsync();
        await Expect(SignupPage.ConfirmationSummary).ToContainTextAsync(SuiteData.FullName);
        await Expect(SignupPage.ConfirmationSummary).ToContainTextAsync(SuiteData.PlanGrowth);
    }

    [Fact]
    public async Task TheFormGivesWayToTheConfirmation()
    {
        UseTestIdAttribute();
        await SignupPage.GotoAsync();

        await SignupPage.CompleteAsync(
            SuiteData.FullName,
            SuiteData.Email,
            SuiteData.PlanGrowth,
            SuiteData.Seats,
            SuiteData.Notes);

        await Expect(SignupPage.Form).ToBeHiddenAsync();
    }
}
