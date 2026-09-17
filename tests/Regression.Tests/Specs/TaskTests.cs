using BlazorStandards.Regression.Tests.Constants;
using BlazorStandards.Regression.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit;

namespace BlazorStandards.Regression.Tests.Specs;

/// <summary>
/// The task list, as a visitor meets it.
/// </summary>
/// <remarks>
/// Every reach goes through a test id. Not one selector here names a class or a
/// tag, which is what lets the styling change without a test noticing.
/// </remarks>
[Collection(ApplicationFixtureDefinition.Name)]
public sealed class TaskTests : PageObjectTest
{
    private const int SeededCount = 2;
    private const int AfterAdding = 3;
    private const int OneRemaining = 1;

    [Fact]
    public async Task TheListShowsTheSeededTasks()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await Expect(TaskPage.TaskItems).ToHaveCountAsync(SeededCount);
        await Expect(TaskPage.Titles).ToContainTextAsync(SuiteData.SeededTasks);
    }

    [Fact]
    public async Task TheCounterReportsWhatRemains()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await Expect(TaskPage.TaskCount)
            .ToHaveTextAsync(SuiteData.RemainingLabel(SeededCount, SeededCount));
    }

    [Fact]
    public async Task AddingATaskShowsItInTheList()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await TaskPage.AddAsync(SuiteData.NewTask);

        await Expect(TaskPage.TaskItems).ToHaveCountAsync(AfterAdding);
        await Expect(TaskPage.ItemFor(SuiteData.NewTask)).ToBeVisibleAsync();
    }

    [Fact]
    public async Task CompletingATaskLeavesTheCountsOverTheWholeList()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await TaskPage.ToggleAsync(SuiteData.SeededTasks[0]);

        await Expect(TaskPage.TaskCount)
            .ToHaveTextAsync(SuiteData.RemainingLabel(OneRemaining, SeededCount));
    }

    [Fact]
    public async Task TheActiveFilterHidesCompletedTasks()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await TaskPage.ToggleAsync(SuiteData.SeededTasks[0]);
        await TaskPage.ShowActiveAsync();

        await Expect(TaskPage.TaskItems).ToHaveCountAsync(OneRemaining);
        await Expect(TaskPage.ItemFor(SuiteData.SeededTasks[0])).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task TheCompletedFilterShowsOnlyFinishedTasks()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await TaskPage.ToggleAsync(SuiteData.SeededTasks[0]);
        await TaskPage.ShowCompletedAsync();

        await Expect(TaskPage.TaskItems).ToHaveCountAsync(OneRemaining);
        await Expect(TaskPage.ItemFor(SuiteData.SeededTasks[0])).ToBeVisibleAsync();
    }

    [Fact]
    public async Task DeletingATaskRemovesIt()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await TaskPage.RemoveAsync(SuiteData.SeededTasks[0]);

        await Expect(TaskPage.ItemFor(SuiteData.SeededTasks[0])).ToHaveCountAsync(0);
        await Expect(TaskPage.TaskItems).ToHaveCountAsync(OneRemaining);
    }

    [Fact]
    public async Task ClearingCompletedKeepsTheActiveTasks()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await TaskPage.ToggleAsync(SuiteData.SeededTasks[0]);
        await TaskPage.ClearCompletedAsync();

        await Expect(TaskPage.TaskItems).ToHaveCountAsync(OneRemaining);
        await Expect(TaskPage.ItemFor(SuiteData.SeededTasks[1])).ToBeVisibleAsync();
    }

    [Fact]
    public async Task AnEmptyTitleAddsNothing()
    {
        UseTestIdAttribute();
        await TaskPage.GotoAsync();

        await TaskPage.AddButton.ClickAsync();

        await Expect(TaskPage.TaskItems).ToHaveCountAsync(SeededCount);
    }
}
