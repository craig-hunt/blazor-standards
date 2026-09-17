using BlazorStandards.App.Models;

namespace BlazorStandards.App.Services;

/// <summary>
/// Holds the task list for one circuit.
/// </summary>
/// <remarks>
/// Registered scoped, which under Interactive Server means one instance per
/// circuit: a visitor's list belongs to that visitor and disappears when the
/// connection ends. A singleton would share one list across every browser
/// reaching the server, which is a surprising default for a demonstration and a
/// defect in anything real.
///
/// A real application would persist instead. The seam is the interface: the
/// page calls it either way.
/// </remarks>
internal sealed class TaskService : ITaskService
{
    private const int FirstId = 1;

    /// <summary>
    /// The rows a fresh circuit starts with. Named rather than written inline,
    /// because the sibling suites assert on this exact text and a reader
    /// looking for it should find it by name rather than by scrolling an array.
    /// </summary>
    private static readonly string[] SeededTitles =
    [
        SeedTitle.ArchitectureRecord,
        SeedTitle.VendorQuestionnaire,
    ];

    private readonly List<TaskItem> _tasks;

    private int _nextId;

    public TaskService()
    {
        _tasks = [.. SeededTitles.Select((title, index) => new TaskItem(FirstId + index, title, false))];
        _nextId = _tasks.Count + FirstId;
    }

    public Task<TaskView> ViewAsync(TaskFilter filter, CancellationToken cancellationToken)
    {
        var shown = _tasks.Where(task => Includes(filter, task)).ToList();
        var remaining = _tasks.Count(task => !task.Completed);

        return Task.FromResult(new TaskView(shown, remaining, _tasks.Count));
    }

    public Task AddAsync(string title, CancellationToken cancellationToken)
    {
        var trimmed = title.Trim();
        if (trimmed.Length == 0)
        {
            // The form rejects an empty title rather than storing a blank row.
            // Returning quietly matches the sibling demo, which ignores the
            // submit without surfacing an error.
            return Task.CompletedTask;
        }

        _tasks.Add(new TaskItem(_nextId, trimmed, false));
        _nextId++;
        return Task.CompletedTask;
    }

    public Task ToggleAsync(int id, CancellationToken cancellationToken)
    {
        var index = _tasks.FindIndex(task => task.Id == id);
        if (index >= 0)
        {
            _tasks[index] = _tasks[index] with { Completed = !_tasks[index].Completed };
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(int id, CancellationToken cancellationToken)
    {
        _tasks.RemoveAll(task => task.Id == id);
        return Task.CompletedTask;
    }

    public Task ClearCompletedAsync(CancellationToken cancellationToken)
    {
        _tasks.RemoveAll(task => task.Completed);
        return Task.CompletedTask;
    }

    private static bool Includes(TaskFilter filter, TaskItem task) => filter switch
    {
        TaskFilter.Active => !task.Completed,
        TaskFilter.Completed => task.Completed,
        _ => true,
    };
}

/// <summary>
/// The seeded task titles, shared verbatim with the sibling projects.
/// </summary>
internal static class SeedTitle
{
    internal const string ArchitectureRecord = "Review the architecture decision record";
    internal const string VendorQuestionnaire = "Reply to the vendor questionnaire";
}
