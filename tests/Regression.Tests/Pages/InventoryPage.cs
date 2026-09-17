using BlazorStandards.Regression.Tests.Constants;
using BlazorStandards.Regression.Tests.Fixtures;
using Microsoft.Playwright;

namespace BlazorStandards.Regression.Tests.Pages;

/// <summary>
/// The stock table.
/// </summary>
internal sealed class InventoryPage(IPage page)
{
    internal ILocator Heading => page.GetByTestId(SuiteTestIds.Inventory.Heading);

    internal ILocator Search => page.GetByTestId(SuiteTestIds.Inventory.Search);

    internal ILocator ResetButton => page.GetByTestId(SuiteTestIds.Inventory.ResetButton);

    internal ILocator ResultCount => page.GetByTestId(SuiteTestIds.Inventory.ResultCount);

    internal ILocator Rows => page.GetByTestId(SuiteTestIds.Inventory.Row);

    internal ILocator Names => page.GetByTestId(SuiteTestIds.Inventory.ItemName);

    internal ILocator NoResults => page.GetByTestId(SuiteTestIds.Inventory.NoResults);

    internal ILocator SortByName => page.GetByTestId(SuiteTestIds.Inventory.SortByName);

    internal ILocator SortByQuantity => page.GetByTestId(SuiteTestIds.Inventory.SortByQuantity);

    internal ILocator NameHeader => page.GetByTestId(SuiteTestIds.Inventory.HeaderName);

    internal ILocator QuantityHeader => page.GetByTestId(SuiteTestIds.Inventory.HeaderQuantity);

    internal Task GotoAsync() => page.GotoInteractiveAsync(SuiteRoutes.Inventory);

    internal Task FilterAsync(string term) => Search.FillAsync(term);

    internal Task ResetAsync() => ResetButton.ClickAsync();
}
