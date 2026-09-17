using BlazorStandards.App.Models;
using BlazorStandards.App.Services;
using Xunit;

namespace BlazorStandards.App.Tests.Services;

/// <summary>
/// Stock searching, ordering, and the sort toggle.
/// </summary>
/// <remarks>
/// The stability case uses two rows tying on the sort column and asserts the
/// supplied order survives. An unstable sort passes every other test here.
/// </remarks>
public sealed class InventoryQueryTests
{
    private const string Alpha = "Alpha cable";
    private const string Bravo = "Bravo cable";
    private const string Charlie = "Charlie dock";
    private const string SearchCable = "cable";
    private const string SearchUpper = "CABLE";
    private const string SearchMissing = "nothing here";
    private const string SearchUntrimmed = "  cable  ";
    private const int TiedQuantity = 5;
    private const int HigherQuantity = 9;
    private const int TotalRows = 3;
    private const int MatchingRows = 2;
    private const int NoRows = 0;

    [Fact]
    public void ApplyOrdersByNameAscendingByDefault()
    {
        var view = InventoryQuery.Apply(Sample(), string.Empty, InventoryColumn.Name, SortDirection.Ascending);

        Assert.Equal(Alpha, view.Shown[0].Name);
        Assert.Equal(Charlie, view.Shown[^1].Name);
    }

    [Fact]
    public void ApplyReversesOrderWhenDescending()
    {
        var view = InventoryQuery.Apply(Sample(), string.Empty, InventoryColumn.Name, SortDirection.Descending);

        Assert.Equal(Charlie, view.Shown[0].Name);
    }

    [Fact]
    public void ApplySortsTiedRowsStably()
    {
        var view = InventoryQuery.Apply(Sample(), string.Empty, InventoryColumn.Quantity, SortDirection.Ascending);

        Assert.Equal(Bravo, view.Shown[0].Name);
        Assert.Equal(Alpha, view.Shown[1].Name);
    }

    [Fact]
    public void ApplyOrdersByQuantityDescending()
    {
        var view = InventoryQuery.Apply(Sample(), string.Empty, InventoryColumn.Quantity, SortDirection.Descending);

        Assert.Equal(Charlie, view.Shown[0].Name);
    }

    /// <summary>
    /// The status column sorts on its text rather than on how stocked a row is,
    /// so the order reads In stock, Low, Out of stock. Asserting both ends
    /// catches an ordering that reversed, which asserting the first row alone
    /// would miss whenever the sample happens to be symmetric.
    /// </summary>
    [Fact]
    public void ApplyOrdersByStatusAscending()
    {
        var view = InventoryQuery.Apply(Sample(), string.Empty, InventoryColumn.Status, SortDirection.Ascending);

        Assert.Equal(StockStatus.InStock, view.Shown[0].Status);
        Assert.Equal(StockStatus.OutOfStock, view.Shown[^1].Status);
    }

    [Fact]
    public void ApplyOrdersByStatusDescending()
    {
        var view = InventoryQuery.Apply(Sample(), string.Empty, InventoryColumn.Status, SortDirection.Descending);

        Assert.Equal(StockStatus.OutOfStock, view.Shown[0].Status);
        Assert.Equal(StockStatus.InStock, view.Shown[^1].Status);
    }

    [Fact]
    public void ApplyMatchesNamesWithoutRegardToCase()
    {
        var view = InventoryQuery.Apply(Sample(), SearchUpper, InventoryColumn.Name, SortDirection.Ascending);

        Assert.Equal(MatchingRows, view.ShownCount);
    }

    [Fact]
    public void ApplyTrimsTheSearchTerm()
    {
        var view = InventoryQuery.Apply(Sample(), SearchUntrimmed, InventoryColumn.Name, SortDirection.Ascending);

        Assert.Equal(MatchingRows, view.ShownCount);
    }

    [Fact]
    public void ApplyReportsTotalAcrossEveryRow()
    {
        var view = InventoryQuery.Apply(Sample(), SearchCable, InventoryColumn.Name, SortDirection.Ascending);

        Assert.Equal(MatchingRows, view.ShownCount);
        Assert.Equal(TotalRows, view.Total);
    }

    [Fact]
    public void ApplyReportsNothingWhenNoNameMatches()
    {
        var view = InventoryQuery.Apply(Sample(), SearchMissing, InventoryColumn.Name, SortDirection.Ascending);

        Assert.Equal(NoRows, view.ShownCount);
        Assert.Equal(TotalRows, view.Total);
    }

    [Fact]
    public void ClickingTheSortedColumnReversesIt()
    {
        var (column, direction) = InventoryQuery.NextSort(
            InventoryColumn.Name,
            SortDirection.Ascending,
            InventoryColumn.Name);

        Assert.Equal(InventoryColumn.Name, column);
        Assert.Equal(SortDirection.Descending, direction);
    }

    [Fact]
    public void ClickingTheSortedColumnTwiceReturnsToAscending()
    {
        var (column, direction) = InventoryQuery.NextSort(
            InventoryColumn.Name,
            SortDirection.Descending,
            InventoryColumn.Name);

        Assert.Equal(InventoryColumn.Name, column);
        Assert.Equal(SortDirection.Ascending, direction);
    }

    [Fact]
    public void ClickingAnotherColumnStartsItAscending()
    {
        var (column, direction) = InventoryQuery.NextSort(
            InventoryColumn.Name,
            SortDirection.Descending,
            InventoryColumn.Quantity);

        Assert.Equal(InventoryColumn.Quantity, column);
        Assert.Equal(SortDirection.Ascending, direction);
    }

    private static IReadOnlyList<InventoryItem> Sample() =>
    [
        new InventoryItem(Bravo, TiedQuantity, StockStatus.Low),
        new InventoryItem(Alpha, TiedQuantity, StockStatus.InStock),
        new InventoryItem(Charlie, HigherQuantity, StockStatus.OutOfStock),
    ];
}
