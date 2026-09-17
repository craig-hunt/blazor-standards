namespace BlazorStandards.App.Constants;

/// <summary>
/// Every test id the regression suite reaches for.
/// </summary>
/// <remarks>
/// These match the sibling projects exactly, because cypress-standards,
/// playwright-standards, and playwright-standards-csharp all drive the same
/// three pages. An id renamed here without renaming it there breaks a suite in
/// another repository, which is the point: the id is a contract between the
/// application and every suite that tests it.
///
/// The application owns this file and the suite mirrors it. A test id invented
/// by a suite is a test id the application never promised.
/// </remarks>
internal static class TestIds
{
    /// <summary>
    /// Signals the application publishes about itself rather than about any one
    /// page.
    /// </summary>
    internal static class App
    {
        /// <summary>
        /// Marks the moment the circuit owns the DOM. Interactive Server serves
        /// prerendered markup first and replaces it when the circuit connects,
        /// so a suite that types into the prerendered copy loses what it typed.
        /// Waiting on this element removes that race for every spec at once.
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
        internal const string Filters = "filters";
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

        // The header cells carry aria-sort, so the suite needs to reach them.
        // ARIA permits that attribute on a columnheader and not on the button
        // inside it, which an accessibility check catches and a suite asserting
        // on the button does not. These ids are additions to the shared
        // contract rather than renames: every sibling id still means what it
        // meant.
        internal const string HeaderName = "header-name";
        internal const string HeaderQuantity = "header-quantity";
        internal const string HeaderStatus = "header-status";
        internal const string NoResults = "no-results";
    }
}
