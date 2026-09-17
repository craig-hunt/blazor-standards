using BlazorStandards.App.Models;

namespace BlazorStandards.App.Services;

/// <summary>
/// Serves the stock rows a fresh circuit starts with.
/// </summary>
/// <remarks>
/// These six rows match the sibling projects exactly, because their suites
/// assert on the names, the quantities, and which row sorts first in each
/// direction. Naming each value rather than writing the table inline means a
/// suite in another repository and this application refer to the same thing by
/// the same name.
/// </remarks>
internal sealed class InventoryService : IInventoryService
{
    private static readonly IReadOnlyList<InventoryItem> Seeded =
    [
        new InventoryItem(SeedItem.AccessBadge, SeedItem.AccessBadgeQuantity, StockStatus.InStock),
        new InventoryItem(SeedItem.DockingStation, SeedItem.DockingStationQuantity, StockStatus.Low),
        new InventoryItem(SeedItem.LaptopSleeve, SeedItem.LaptopSleeveQuantity, StockStatus.OutOfStock),
        new InventoryItem(SeedItem.MonitorArm, SeedItem.MonitorArmQuantity, StockStatus.InStock),
        new InventoryItem(SeedItem.Headset, SeedItem.HeadsetQuantity, StockStatus.Low),
        new InventoryItem(SeedItem.Webcam, SeedItem.WebcamQuantity, StockStatus.InStock),
    ];

    public Task<IReadOnlyList<InventoryItem>> ItemsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Seeded);
}

/// <summary>The seeded stock rows, shared verbatim with the sibling projects.</summary>
internal static class SeedItem
{
    internal const string AccessBadge = "Access badge";
    internal const string DockingStation = "Docking station";
    internal const string LaptopSleeve = "Laptop sleeve";
    internal const string MonitorArm = "Monitor arm";
    internal const string Headset = "Noise-cancelling headset";
    internal const string Webcam = "Webcam";

    internal const int AccessBadgeQuantity = 240;
    internal const int DockingStationQuantity = 12;
    internal const int LaptopSleeveQuantity = 0;
    internal const int MonitorArmQuantity = 58;
    internal const int HeadsetQuantity = 4;
    internal const int WebcamQuantity = 31;
}

/// <summary>The words the status column shows.</summary>
internal static class StockStatus
{
    internal const string InStock = "In stock";
    internal const string Low = "Low";
    internal const string OutOfStock = "Out of stock";
}
