using BlazorStandards.Regression.Tests.Constants;
using BlazorStandards.Regression.Tests.Fixtures;
using Microsoft.Playwright;

namespace BlazorStandards.Regression.Tests.Pages;

/// <summary>
/// The signup form.
/// </summary>
internal sealed class SignupPage(IPage page)
{
    internal ILocator Heading => page.GetByTestId(SuiteTestIds.Signup.Heading);

    internal ILocator Form => page.GetByTestId(SuiteTestIds.Signup.Form);

    internal ILocator FullName => page.GetByTestId(SuiteTestIds.Signup.FullNameInput);

    internal ILocator FullNameError => page.GetByTestId(SuiteTestIds.Signup.FullNameError);

    internal ILocator Email => page.GetByTestId(SuiteTestIds.Signup.EmailInput);

    internal ILocator EmailError => page.GetByTestId(SuiteTestIds.Signup.EmailError);

    internal ILocator Plan => page.GetByTestId(SuiteTestIds.Signup.PlanSelect);

    internal ILocator PlanError => page.GetByTestId(SuiteTestIds.Signup.PlanError);

    internal ILocator Seats => page.GetByTestId(SuiteTestIds.Signup.SeatsInput);

    internal ILocator Notes => page.GetByTestId(SuiteTestIds.Signup.NotesTextarea);

    internal ILocator Terms => page.GetByTestId(SuiteTestIds.Signup.TermsCheckbox);

    internal ILocator TermsError => page.GetByTestId(SuiteTestIds.Signup.TermsError);

    internal ILocator Submit => page.GetByTestId(SuiteTestIds.Signup.SubmitButton);

    internal ILocator Confirmation => page.GetByTestId(SuiteTestIds.Signup.Confirmation);

    internal ILocator ConfirmationSummary => page.GetByTestId(SuiteTestIds.Signup.ConfirmationSummary);

    internal Task GotoAsync() => page.GotoInteractiveAsync(SuiteRoutes.Signup);

    internal Task SubmitEmptyAsync() => Submit.ClickAsync();

    /// <summary>
    /// Fills the form and submits it. Takes the values rather than assuming
    /// them, so a test varying one field reads as the case it exercises.
    /// </summary>
    internal async Task CompleteAsync(string fullName, string email, string plan, int seats, string notes)
    {
        await FullName.FillAsync(fullName);
        await Email.FillAsync(email);
        await Plan.SelectOptionAsync(plan);
        await Seats.FillAsync(seats.ToString(System.Globalization.CultureInfo.InvariantCulture));
        await Notes.FillAsync(notes);
        await Terms.CheckAsync();
        await Submit.ClickAsync();
    }
}
