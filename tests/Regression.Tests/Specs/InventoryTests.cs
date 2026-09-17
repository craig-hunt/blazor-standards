using BlazorStandards.Regression.Tests.Constants;
using BlazorStandards.Regression.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit;

namespace BlazorStandards.Regression.Tests.Specs;

/// <summary>
/// The stock table: filtering, sorting, and what it reports.
/// </summary>
[Collection(ApplicationFixtureDefinition.Name)]
public sealed class InventoryTests : PageObjectTest
{
    private const int NoRows = 0;

    [Fact]
    public async Task EveryItemShowsBeforeFiltering()
    {
        UseTestIdAttribute();
        await InventoryPage.GotoAsync();

        await Expect(InventoryPage.Rows).ToHaveCountAsync(SuiteData.Inventory.TotalItems);
        await Expect(InventoryPage.ResultCount).ToHaveTextAsync(
            SuiteData.ResultCountLabel(SuiteData.Inventory.TotalItems, SuiteData.Inventory.TotalItems));
    }

    [Fact]
    public async Task FilteringNarrowsTheRowsAndTheCount()
    {
        UseTestIdAttribute();
        await InventoryPage.GotoAsync();

        await InventoryPage.FilterAsync(SuiteData.Inventory.SearchTerm);

        await Expect(InventoryPage.Rows).ToHaveCountAsync(SuiteData.Inventory.SearchMatches);
        await Expect(InventoryPage.ResultCount).ToHaveTextAsync(
            SuiteData.ResultCountLabel(SuiteData.Inventory.SearchMatches, SuiteData.Inventory.TotalItems));
    }

    [Fact]
    public async Task AFilterMatchingNothingShowsTheEmptyMessage()
    {
        UseTestIdAttribute();
        await InventoryPage.GotoAsync();

        await InventoryPage.FilterAsync(SuiteData.Inventory.MissingTerm);

        await Expect(InventoryPage.Rows).ToHaveCountAsync(NoRows);
        await Expect(InventoryPage.NoResults).ToBeVisibleAsync();
    }

    [Fact]
    public async Task ResettingRestoresEveryRow()
    {
        UseTestIdAttribute();
        await InventoryPage.GotoAsync();

        await InventoryPage.FilterAsync(SuiteData.Inventory.MissingTerm);
        await InventoryPage.ResetAsync();

        await Expect(InventoryPage.Rows).ToHaveCountAsync(SuiteData.Inventory.TotalItems);
    }

    [Fact]
    public async Task NameSortsAscendingThenDescending()
    {
        UseTestIdAttribute();
        await InventoryPage.GotoAsync();

        await Expect(InventoryPage.Names.First).ToHaveTextAsync(SuiteData.Inventory.FirstNameAscending);

        await InventoryPage.SortByName.ClickAsync();

        await Expect(InventoryPage.Names.First).ToHaveTextAsync(SuiteData.Inventory.FirstNameDescending);
    }

    [Fact]
    public async Task TheSortedColumnReportsItsDirection()
    {
        UseTestIdAttribute();
        await InventoryPage.GotoAsync();

        await Expect(InventoryPage.NameHeader)
            .ToHaveAttributeAsync(SuiteAria.Sort, SuiteAria.Ascending);

        await InventoryPage.SortByName.ClickAsync();

        await Expect(InventoryPage.NameHeader)
            .ToHaveAttributeAsync(SuiteAria.Sort, SuiteAria.Descending);
    }

    [Fact]
    public async Task AnUnsortedColumnReportsNone()
    {
        UseTestIdAttribute();
        await InventoryPage.GotoAsync();

        await Expect(InventoryPage.QuantityHeader)
            .ToHaveAttributeAsync(SuiteAria.Sort, SuiteAria.None);
    }

    [Fact]
    public async Task SortingByQuantityOrdersNumerically()
    {
        UseTestIdAttribute();
        await InventoryPage.GotoAsync();

        await InventoryPage.SortByQuantity.ClickAsync();

        await Expect(InventoryPage.Names.First).ToHaveTextAsync(SuiteData.Inventory.LowestQuantityItem);
    }
}
