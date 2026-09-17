using System.Globalization;
using System.Text;
using BlazorStandards.Regression.Tests.Constants;
using BlazorStandards.Regression.Tests.Fixtures;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Xunit;

namespace BlazorStandards.Regression.Tests.Specs;

/// <summary>
/// WCAG 2.2 AA, checked automatically on every page.
/// </summary>
/// <remarks>
/// An automated pass finds a minority of real accessibility problems, and it
/// finds the mechanical ones reliably: a missing label, a control with no
/// accessible name, contrast below the threshold. Those are the ones that creep
/// back in during ordinary work, so a suite that catches them keeps a manual
/// review focused on the judgment calls it alone can make.
///
/// The run fails on any violation rather than reporting a count, because a
/// threshold above zero becomes the number nobody drives down.
/// </remarks>
[Collection(ApplicationFixtureDefinition.Name)]
public sealed class AccessibilityTests : PageObjectTest
{
    private const int NoViolations = 0;
    private const string TagRunType = "tag";
    private const string Separator = ", ";
    private const string Wcag2A = "wcag2a";
    private const string Wcag2Aa = "wcag2aa";
    private const string Wcag21Aa = "wcag21aa";
    private const string Wcag22Aa = "wcag22aa";
    private const string ViolationFormat = "{0} has {1} accessibility violations: {2}";

    private static readonly CompositeFormat ViolationTemplate = CompositeFormat.Parse(ViolationFormat);

    public static TheoryData<string> Routes() =>
        new(SuiteRoutes.Tasks, SuiteRoutes.Signup, SuiteRoutes.Inventory);

    [Theory]
    [MemberData(nameof(Routes))]
    public async Task EveryPageMeetsWcagAa(string route)
    {
        UseTestIdAttribute();
        await Page.GotoInteractiveAsync(route);

        var results = await Page.RunAxe(new AxeRunOptions
        {
            RunOnly = new RunOnlyOptions
            {
                Type = TagRunType,
                Values = [Wcag2A, Wcag2Aa, Wcag21Aa, Wcag22Aa],
            },
        });

        Assert.True(
            results.Violations.Length == NoViolations,
            string.Format(
                CultureInfo.InvariantCulture,
                ViolationTemplate,
                route,
                results.Violations.Length,
                string.Join(Separator, results.Violations.Select(violation => violation.Id))));
    }
}
