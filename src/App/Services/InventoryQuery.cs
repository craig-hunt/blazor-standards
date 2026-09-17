using BlazorStandards.App.Models;

namespace BlazorStandards.App.Services;

/// <summary>
/// Filters and orders the stock rows.
/// </summary>
/// <remarks>
/// Pure and static, so the page holds the search term and the sort while the
/// rules for applying them live somewhere a test reaches without a browser.
/// This is the code the mutation gate covers.
///
/// The order is stable: rows tying on the sort column keep the order the store
/// returned rather than shuffling between renders.
/// </remarks>
internal static class InventoryQuery
{
    /// <summary>
    /// Decides the sort after a header click. Clicking the column already
    /// sorted reverses it; clicking a different column starts that one
    /// ascending rather than inheriting the previous direction, which would
    /// leave a reader sorted descending without having asked.
    /// </summary>
    internal static (InventoryColumn Column, SortDirection Direction) NextSort(
        InventoryColumn current,
        SortDirection direction,
        InventoryColumn clicked) =>
        clicked == current
            ? (current, Reverse(direction))
            : (clicked, SortDirection.Ascending);

    internal static InventoryView Apply(
        IReadOnlyList<InventoryItem> items,
        string search,
        InventoryColumn column,
        SortDirection direction)
    {
        var matched = items
            .Where(item => item.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        var ordered = direction == SortDirection.Descending
            ? OrderDescending(matched, column)
            : OrderAscending(matched, column);

        var shown = ordered.ToList();
        return new InventoryView(shown, shown.Count, items.Count);
    }

    private static SortDirection Reverse(SortDirection direction) =>
        direction == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;

    private static IOrderedEnumerable<InventoryItem> OrderAscending(
        List<InventoryItem> items,
        InventoryColumn column) => column switch
        {
            InventoryColumn.Quantity => items.OrderBy(item => item.Quantity),
            InventoryColumn.Status => items.OrderBy(item => item.Status, StringComparer.Ordinal),
            _ => items.OrderBy(item => item.Name, StringComparer.Ordinal),
        };

    private static IOrderedEnumerable<InventoryItem> OrderDescending(
        List<InventoryItem> items,
        InventoryColumn column) => column switch
        {
            InventoryColumn.Quantity => items.OrderByDescending(item => item.Quantity),
            InventoryColumn.Status => items.OrderByDescending(item => item.Status, StringComparer.Ordinal),
            _ => items.OrderByDescending(item => item.Name, StringComparer.Ordinal),
        };
}
