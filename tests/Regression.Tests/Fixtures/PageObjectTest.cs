using BlazorStandards.Regression.Tests.Constants;
using BlazorStandards.Regression.Tests.Pages;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit.v3;

namespace BlazorStandards.Regression.Tests.Fixtures;

/// <summary>
/// Delivers page objects to the tests that need them.
/// </summary>
/// <remarks>
/// Playwright for .NET has no fixture-injection mechanism, so a base class
/// carries the role: PageTest supplies a fresh Page for each test, and these
/// properties build a page object over whichever Page is current.
///
/// The properties construct on every access rather than caching. A cached page
/// object would hold the first test's Page, and every later test would fail
/// naming a closed browser rather than the real cause.
/// </remarks>
public abstract class PageObjectTest : PageTest
{
    internal TaskPage TaskPage => new(Page);

    internal SignupPage SignupPage => new(Page);

    internal InventoryPage InventoryPage => new(Page);

    /// <summary>
    /// Points every relative navigation at the application, and names the test
    /// id attribute once so page objects call GetByTestId and never write it.
    /// </summary>
    public override BrowserNewContextOptions ContextOptions() =>
        new() { BaseURL = SuiteConfig.BaseUrl };

    protected void UseTestIdAttribute() =>
        Playwright.Selectors.SetTestIdAttribute(SuiteConfig.TestIdAttribute);
}
