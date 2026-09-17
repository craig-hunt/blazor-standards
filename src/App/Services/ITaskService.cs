using BlazorStandards.App.Models;

namespace BlazorStandards.App.Services;

/// <summary>
/// The task operations a page calls.
/// </summary>
/// <remarks>
/// Defined beside the component that consumes it rather than beside the class
/// that implements it, so a page depends on the shape it needs and a second
/// implementation slots in without the page learning about it.
///
/// Every method takes a CancellationToken. This reference holds state in
/// memory and never observes one, and it takes them anyway: a signature that
/// omits the token forces every caller above it to drop cancellation too, and
/// adding it later changes every call site at once.
/// </remarks>
internal interface ITaskService
{
    public Task<TaskView> ViewAsync(TaskFilter filter, CancellationToken cancellationToken);

    public Task AddAsync(string title, CancellationToken cancellationToken);

    public Task ToggleAsync(int id, CancellationToken cancellationToken);

    public Task RemoveAsync(int id, CancellationToken cancellationToken);

    public Task ClearCompletedAsync(CancellationToken cancellationToken);
}
