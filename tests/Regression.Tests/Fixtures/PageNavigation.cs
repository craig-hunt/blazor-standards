using BlazorStandards.Regression.Tests.Constants;
using Microsoft.Playwright;

namespace BlazorStandards.Regression.Tests.Fixtures;

/// <summary>
/// Navigation that lands on an application ready to be used.
/// </summary>
/// <remarks>
/// Every suite driving Interactive Server needs this and the static-site
/// siblings do not, which makes it the one standard this repository adds rather
/// than mirrors.
///
/// A plain GotoAsync resolves when the document loads. At that moment the page
/// holds prerendered HTML: it reads correctly, it has every test id, and none of
/// its controls are wired, because the circuit has not connected. A click
/// dispatched then is lost. Worse, text typed then is discarded when the circuit
/// renders over it, so the failure surfaces later as an empty field submitted by
/// a click that worked, which points the investigation at the wrong page.
///
/// Waiting for the readiness marker removes that race for every spec at once.
/// It waits for attachment rather than visibility because the marker is hidden:
/// it exists to be found, not to be read.
/// </remarks>
internal static class PageNavigation
{
    internal static async Task GotoInteractiveAsync(this IPage page, string route)
    {
        await page.GotoAsync(route);
        await page.GetByTestId(SuiteTestIds.App.Ready)
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached });
    }
}
