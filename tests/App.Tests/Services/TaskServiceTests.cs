using BlazorStandards.App.Models;
using BlazorStandards.App.Services;
using Xunit;

namespace BlazorStandards.App.Tests.Services;

/// <summary>
/// The task rules, asserted at their boundaries.
/// </summary>
/// <remarks>
/// The counting cases run under a filter that hides rows, because that is the
/// only arrangement where counting the shown list differs from counting the
/// whole set. A test that only ever asks for every task cannot tell the two
/// apart.
/// </remarks>
public sealed class TaskServiceTests
{
    private const string NewTitle = "Draft the incident postmortem";
    private const string SecondTitle = "Book the venue";
    private const string Untrimmed = "   Draft the incident postmortem   ";
    private const int SeededCount = 2;
    private const int FirstSeededId = 1;
    private const int SecondSeededId = 2;
    private const int AfterAdding = 3;
    private const int AfterAddingTwice = 4;

    [Fact]
    public async Task ANewServiceSeedsTheSharedTitles()
    {
        var view = await ViewAsync(new TaskService(), TaskFilter.All);

        Assert.Equal(SeededCount, view.Total);
        Assert.Equal(SeededCount, view.Remaining);
        Assert.Equal(SeededCount, view.Shown.Count);
    }

    [Fact]
    public async Task AddingTrimsTheTitle()
    {
        var service = new TaskService();

        await service.AddAsync(Untrimmed, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.Contains(view.Shown, task => task.Title == NewTitle);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddingAnEmptyTitleStoresNothing(string title)
    {
        var service = new TaskService();

        await service.AddAsync(title, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.Equal(SeededCount, view.Total);
    }

    [Fact]
    public async Task AddingGivesTheNewTaskTheNextIdentifier()
    {
        var service = new TaskService();

        await service.AddAsync(NewTitle, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.Equal(AfterAdding, view.Shown[^1].Id);
    }

    /// <summary>
    /// Two additions, because one cannot tell a counter that advances from a
    /// counter that stands still: the first task takes the next identifier
    /// either way, and only the second one collides.
    /// </summary>
    [Fact]
    public async Task AddingTwiceGivesEachTaskItsOwnIdentifier()
    {
        var service = new TaskService();

        await service.AddAsync(NewTitle, TestContext.Current.CancellationToken);
        await service.AddAsync(SecondTitle, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.Contains(view.Shown, task => task.Id == AfterAdding);
        Assert.Contains(view.Shown, task => task.Id == AfterAddingTwice);
    }

    [Fact]
    public async Task AnAddedTaskStartsActive()
    {
        var service = new TaskService();

        await service.AddAsync(NewTitle, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.False(view.Shown[^1].Completed);
        Assert.Equal(AfterAdding, view.Remaining);
    }

    [Fact]
    public async Task TogglingMarksTheTaskCompleted()
    {
        var service = new TaskService();

        await service.ToggleAsync(FirstSeededId, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.True(view.Shown.Single(task => task.Id == FirstSeededId).Completed);
    }

    [Fact]
    public async Task TogglingTwiceReturnsTheTaskToActive()
    {
        var service = new TaskService();

        await service.ToggleAsync(FirstSeededId, TestContext.Current.CancellationToken);
        await service.ToggleAsync(FirstSeededId, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.False(view.Shown.Single(task => task.Id == FirstSeededId).Completed);
    }

    [Fact]
    public async Task TogglingAnUnknownIdentifierChangesNothing()
    {
        var service = new TaskService();

        await service.ToggleAsync(AfterAdding, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.Equal(SeededCount, view.Remaining);
    }

    [Fact]
    public async Task CountsDescribeEveryTaskWhileTheFilterNarrowsTheList()
    {
        var service = new TaskService();
        await service.ToggleAsync(FirstSeededId, TestContext.Current.CancellationToken);

        var view = await ViewAsync(service, TaskFilter.Completed);

        Assert.Single(view.Shown);
        Assert.Equal(SeededCount, view.Total);
        Assert.Equal(SeededCount - 1, view.Remaining);
    }

    [Fact]
    public async Task TheActiveFilterShowsOnlyIncompleteTasks()
    {
        var service = new TaskService();
        await service.ToggleAsync(FirstSeededId, TestContext.Current.CancellationToken);

        var view = await ViewAsync(service, TaskFilter.Active);

        Assert.All(view.Shown, task => Assert.False(task.Completed));
    }

    [Fact]
    public async Task RemovingDropsTheTask()
    {
        var service = new TaskService();

        await service.RemoveAsync(FirstSeededId, TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.DoesNotContain(view.Shown, task => task.Id == FirstSeededId);
        Assert.Equal(SeededCount - 1, view.Total);
    }

    [Fact]
    public async Task ClearingCompletedKeepsTheActiveTasks()
    {
        var service = new TaskService();
        await service.ToggleAsync(SecondSeededId, TestContext.Current.CancellationToken);

        await service.ClearCompletedAsync(TestContext.Current.CancellationToken);
        var view = await ViewAsync(service, TaskFilter.All);

        Assert.Single(view.Shown);
        Assert.Equal(FirstSeededId, view.Shown[0].Id);
    }

    private static Task<TaskView> ViewAsync(TaskService service, TaskFilter filter) =>
        service.ViewAsync(filter, TestContext.Current.CancellationToken);
}
