using BlazorStandards.App.Models;

namespace BlazorStandards.App.Services;

/// <summary>
/// Reads the stock rows.
/// </summary>
/// <remarks>
/// Searching and sorting stay out of here deliberately. The store answers with
/// rows; InventoryQuery decides which of them a reader sees and in what order,
/// so a second implementation cannot quietly answer with a different order.
/// </remarks>
internal interface IInventoryService
{
    public Task<IReadOnlyList<InventoryItem>> ItemsAsync(CancellationToken cancellationToken);
}
