namespace BlazorStandards.App.Models;

/// <summary>Which tasks a filter shows.</summary>
public enum TaskFilter
{
    All,
    Active,
    Completed,
}

/// <summary>One task on the list.</summary>
public sealed record TaskItem(int Id, string Title, bool Completed);

/// <summary>
/// What the task page renders: the tasks the filter shows, how many remain
/// incomplete across every task, and how many tasks exist.
/// </summary>
/// <remarks>
/// Remaining and Total count the whole set rather than the filtered view, so
/// the counter stays steady while a reader switches filters. A count taken from
/// the shown list would drop to zero under the Completed filter and read as
/// though the work had vanished.
/// </remarks>
public sealed record TaskView(IReadOnlyList<TaskItem> Shown, int Remaining, int Total);
