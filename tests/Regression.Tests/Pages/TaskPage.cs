using BlazorStandards.Regression.Tests.Constants;
using BlazorStandards.Regression.Tests.Fixtures;
using Microsoft.Playwright;

namespace BlazorStandards.Regression.Tests.Pages;

/// <summary>
/// The task list.
/// </summary>
/// <remarks>
/// Properties return ILocator rather than resolved strings, because a locator
/// carries Playwright's auto-waiting into the assertion. A property returning a
/// resolved value hands the test a stale snapshot, which is the most common
/// source of flake in a page-object suite, and matters more here than against a
/// static page: an Interactive Server render round-trips over the circuit, so
/// the DOM changes after the click returns.
///
/// No assertion appears in this file. Tests assert; page objects reach and act.
/// </remarks>
internal sealed class TaskPage(IPage page)
{
    internal ILocator Heading => page.GetByTestId(SuiteTestIds.Task.Heading);

    internal ILocator NewTaskInput => page.GetByTestId(SuiteTestIds.Task.Input);

    internal ILocator AddButton => page.GetByTestId(SuiteTestIds.Task.AddButton);

    internal ILocator TaskCount => page.GetByTestId(SuiteTestIds.Task.Count);

    internal ILocator TaskItems => page.GetByTestId(SuiteTestIds.Task.Item);

    internal ILocator Titles => page.GetByTestId(SuiteTestIds.Task.ItemTitle);

    internal ILocator EmptyState => page.GetByTestId(SuiteTestIds.Task.EmptyState);

    internal ILocator ClearCompletedButton => page.GetByTestId(SuiteTestIds.Task.ClearCompletedButton);

    internal Task GotoAsync() => page.GotoInteractiveAsync(SuiteRoutes.Tasks);

    /// <summary>
    /// One task, found by its visible text. Every row renders the same test id,
    /// so reaching a single row needs a second dimension. Filtering by the text
    /// a user reads survives reordering; an index does not.
    /// </summary>
    internal ILocator ItemFor(string title) =>
        TaskItems.Filter(new LocatorFilterOptions { HasText = title });

    internal ILocator CheckboxFor(string title) =>
        ItemFor(title).GetByTestId(SuiteTestIds.Task.ItemCheckbox);

    internal async Task AddAsync(string title)
    {
        await NewTaskInput.FillAsync(title);
        await AddButton.ClickAsync();
    }

    internal Task ToggleAsync(string title) => CheckboxFor(title).ClickAsync();

    internal Task RemoveAsync(string title) =>
        ItemFor(title).GetByTestId(SuiteTestIds.Task.DeleteButton).ClickAsync();

    internal Task ShowAllAsync() => page.GetByTestId(SuiteTestIds.Task.FilterAll).ClickAsync();

    internal Task ShowActiveAsync() => page.GetByTestId(SuiteTestIds.Task.FilterActive).ClickAsync();

    internal Task ShowCompletedAsync() => page.GetByTestId(SuiteTestIds.Task.FilterCompleted).ClickAsync();

    internal Task ClearCompletedAsync() => ClearCompletedButton.ClickAsync();
}
