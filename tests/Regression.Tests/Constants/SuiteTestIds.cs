namespace BlazorStandards.Regression.Tests.Constants;

/// <summary>
/// Every test id the suite reaches for.
/// </summary>
/// <remarks>
/// These mirror the application's TestIds class, and they match
/// cypress-standards and playwright-standards-csharp exactly, because all of
/// them drive the same three pages.
///
/// The suite keeps its own copy rather than referencing the application's.
/// A suite that compiles against the thing it tests cannot be pointed at a
/// deployed build, and the ids are a wire contract rather than a shared type:
/// if the application renames one without renaming it here, the suite should go
/// red. That is the contract working, not a duplication problem.
/// </remarks>
internal static class SuiteTestIds
{
    /// <summary>
    /// Signals the application publishes about itself rather than about any one
    /// page.
    /// </summary>
    internal static class App
    {
        /// <summary>
        /// Appears once the circuit owns the DOM. Before it appears the markup
        /// on screen came from prerendering, and anything typed into it gets
        /// discarded by the first interactive render.
        /// </summary>
        internal const string Ready = "app-ready";
    }

    internal static class Nav
    {
        internal const string Container = "primary-nav";
        internal const string Tasks = "nav-tasks";
        internal const string Signup = "nav-signup";
        internal const string Inventory = "nav-inventory";
    }

    internal static class Task
    {
        internal const string Heading = "page-heading";
        internal const string Form = "new-task-form";
        internal const string Input = "new-task-input";
        internal const string AddButton = "add-task-button";
        internal const string Count = "task-count";
        internal const string List = "task-list";
        internal const string Item = "task-item";
        internal const string ItemTitle = "task-title";
        internal const string ItemCheckbox = "task-checkbox";
        internal const string DeleteButton = "delete-task-button";
        internal const string FilterAll = "filter-all";
        internal const string FilterActive = "filter-active";
        internal const string FilterCompleted = "filter-completed";
        internal const string ClearCompletedButton = "clear-completed-button";
        internal const string EmptyState = "empty-state";
    }

    internal static class Signup
    {
        internal const string Heading = "page-heading";
        internal const string Form = "signup-form";
        internal const string FullNameInput = "full-name-input";
        internal const string FullNameError = "full-name-error";
        internal const string EmailInput = "email-input";
        internal const string EmailError = "email-error";
        internal const string PlanSelect = "plan-select";
        internal const string PlanError = "plan-error";
        internal const string SeatsInput = "seats-input";
        internal const string NotesTextarea = "notes-textarea";
        internal const string TermsCheckbox = "terms-checkbox";
        internal const string TermsError = "terms-error";
        internal const string SubmitButton = "submit-button";
        internal const string Confirmation = "signup-confirmation";
        internal const string ConfirmationHeading = "confirmation-heading";
        internal const string ConfirmationSummary = "confirmation-summary";
    }

    internal static class Inventory
    {
        internal const string Heading = "page-heading";
        internal const string Search = "inventory-search";
        internal const string ResetButton = "reset-filter-button";
        internal const string ResultCount = "result-count";
        internal const string Table = "inventory-table";
        internal const string Body = "inventory-body";
        internal const string Row = "inventory-row";
        internal const string ItemName = "item-name";
        internal const string ItemQuantity = "item-quantity";
        internal const string ItemStatus = "item-status";
        internal const string SortByName = "sort-by-name";
        internal const string SortByQuantity = "sort-by-quantity";
        internal const string SortByStatus = "sort-by-status";

        // aria-sort reports on the header cell, never on the button inside it:
        // ARIA allows the attribute on a columnheader and disallows it on a
        // button. The sibling suites asserted it on the button, which is how
        // the defect survived in those projects.
        internal const string HeaderName = "header-name";
        internal const string HeaderQuantity = "header-quantity";
        internal const string HeaderStatus = "header-status";
        internal const string NoResults = "no-results";
    }
}

/// <summary>Paths the application serves, relative to the base URL.</summary>
/// <remarks>
/// No .html extension: Blazor routes rather than serving files, which is the
/// one place this suite's paths differ from the static-site siblings.
/// </remarks>
internal static class SuiteRoutes
{
    internal const string Tasks = "/";
    internal const string Signup = "/signup";
    internal const string Inventory = "/inventory";
}

/// <summary>Platform attribute names and values the suite asserts on.</summary>
internal static class SuiteAria
{
    internal const string Invalid = "aria-invalid";
    internal const string Sort = "aria-sort";
    internal const string True = "true";
    internal const string Ascending = "ascending";
    internal const string Descending = "descending";
    internal const string None = "none";
}
