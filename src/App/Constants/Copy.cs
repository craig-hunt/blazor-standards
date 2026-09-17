using System.Globalization;
using System.Text;

namespace BlazorStandards.App.Constants;

/// <summary>
/// Every word the application renders.
/// </summary>
/// <remarks>
/// No Razor file contains prose. Copy written inline in markup cannot be found
/// by a reader looking for it, changes in one place while an identical string
/// stays in another, and gives a test nothing to assert against but a literal it
/// repeats. A constants class also gives localization somewhere to arrive later:
/// swapping this for resource lookups changes this file and no component.
///
/// The counter formats are methods rather than constants because they take
/// values. Their shape is named once for the same reason the words are: a test
/// asserting the wording inline hides the format from every other test.
/// </remarks>
internal static class Copy
{
    internal const string ApplicationName = "Blazor Standards";

    internal static class Nav
    {
        internal const string Tasks = "Tasks";
        internal const string Signup = "Sign up";
        internal const string Inventory = "Inventory";
        internal const string FilterGroupLabel = "Filter tasks";
    }

    internal static class Tasks
    {
        internal const string Title = "Task list";
        internal const string Heading = "Task list";
        internal const string AddLabel = "Add a task";
        internal const string AddPlaceholder = "What needs doing?";
        internal const string AddButton = "Add";
        internal const string DeleteButton = "Delete";
        internal const string FilterAll = "All";
        internal const string FilterActive = "Active";
        internal const string FilterCompleted = "Completed";
        internal const string ClearCompleted = "Clear completed";
        internal const string EmptyState = "Nothing here yet. Add a task above.";

        private const string RemainingFormat = "{0} remaining of {1}";
        private const string CompleteLabelFormat = "Mark {0} complete";
        private const string DeleteLabelFormat = "Delete {0}";

        // Parsed once. Each of these renders on every pass over the list, so the
        // format is worth holding rather than reparsing per row.
        private static readonly CompositeFormat RemainingTemplate = CompositeFormat.Parse(RemainingFormat);
        private static readonly CompositeFormat CompleteTemplate = CompositeFormat.Parse(CompleteLabelFormat);
        private static readonly CompositeFormat DeleteTemplate = CompositeFormat.Parse(DeleteLabelFormat);

        /// <summary>The counter under the input, named once.</summary>
        internal static string Remaining(int remaining, int total) =>
            string.Format(CultureInfo.InvariantCulture, RemainingTemplate, remaining, total);

        internal static string CompleteLabel(string title) =>
            string.Format(CultureInfo.InvariantCulture, CompleteTemplate, title);

        internal static string DeleteLabel(string title) =>
            string.Format(CultureInfo.InvariantCulture, DeleteTemplate, title);
    }

    internal static class Signup
    {
        internal const string Title = "Sign up";
        internal const string Heading = "Create an account";
        internal const string FullNameLabel = "Full name";
        internal const string EmailLabel = "Work email";
        internal const string PlanLabel = "Plan";
        internal const string PlanPlaceholder = "Choose a plan";
        internal const string SeatsLabel = "Seats";
        internal const string NotesLabel = "Anything we should know?";
        internal const string TermsLabel = "I accept the terms";
        internal const string SubmitButton = "Create account";
        internal const string ConfirmationHeading = "Account created";

        private const string SummaryFormat = "{0} on the {1} plan, {2} seat(s).";

        private static readonly CompositeFormat SummaryTemplate = CompositeFormat.Parse(SummaryFormat);

        internal static string Summary(string fullName, string plan, int seats) =>
            string.Format(CultureInfo.InvariantCulture, SummaryTemplate, fullName, plan, seats);
    }

    internal static class Inventory
    {
        internal const string Title = "Inventory";
        internal const string Heading = "Inventory";
        internal const string SearchLabel = "Filter by name";
        internal const string SearchPlaceholder = "Type to filter";
        internal const string ResetButton = "Reset";
        internal const string ColumnName = "Name";
        internal const string ColumnQuantity = "Quantity";
        internal const string ColumnStatus = "Status";
        internal const string NoResults = "No items match that filter.";

        private const string ResultCountFormat = "Showing {0} of {1} items";

        private static readonly CompositeFormat ResultCountTemplate = CompositeFormat.Parse(ResultCountFormat);

        internal static string ResultCount(int shown, int total) =>
            string.Format(CultureInfo.InvariantCulture, ResultCountTemplate, shown, total);
    }

    /// <summary>
    /// The sentences the signup form shows beside a field it rejected. These
    /// match the sibling projects word for word, because their suites assert on
    /// them.
    /// </summary>
    internal static class Validation
    {
        internal const string NameRequired = "Enter your full name.";
        internal const string EmailRequired = "Enter your work email.";
        internal const string EmailInvalid = "Enter a valid email address.";
        internal const string PlanRequired = "Choose a plan.";
        internal const string TermsRequired = "Accept the terms to continue.";
    }
}
