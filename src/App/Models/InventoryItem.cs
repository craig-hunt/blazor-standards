namespace BlazorStandards.App.Models;

/// <summary>A column the stock table sorts on.</summary>
public enum InventoryColumn
{
    Name,
    Quantity,
    Status,
}

/// <summary>Which way a sort runs.</summary>
public enum SortDirection
{
    Ascending,
    Descending,
}

/// <summary>One row of the stock table.</summary>
public sealed record InventoryItem(string Name, int Quantity, string Status);

/// <summary>
/// What a stock query answers: the rows that matched, how many matched, and how
/// many rows exist.
/// </summary>
public sealed record InventoryView(IReadOnlyList<InventoryItem> Shown, int ShownCount, int Total);
