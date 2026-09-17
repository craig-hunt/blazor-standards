using System.Globalization;
using System.Text;

namespace BlazorStandards.Regression.Tests.Constants;

/// <summary>
/// The values the application seeds and the copy it renders.
/// </summary>
/// <remarks>
/// A test asserting these inline would break silently when the fixture changes,
/// and nothing would say why. They match the sibling projects word for word.
/// </remarks>
internal static class SuiteData
{
    internal const string NewTask = "Draft the incident postmortem";
    internal const string SecondTask = "Book the venue";

    internal const string FullName = "Dana Whitfield";
    internal const string Email = "dana.whitfield@example.com";
    internal const string InvalidEmail = "dana.whitfield.example.com";
    internal const string PlanGrowth = "Growth";
    internal const string Notes = "Migrating from a competitor next quarter.";
    internal const int Seats = 12;

    internal const string ArchitectureRecord = "Review the architecture decision record";
    internal const string VendorQuestionnaire = "Reply to the vendor questionnaire";

    internal const string RemainingFormat = "{0} remaining of {1}";
    internal const string ResultCountFormat = "Showing {0} of {1} items";

    internal static readonly string[] SeededTasks = [ArchitectureRecord, VendorQuestionnaire];

    private static readonly CompositeFormat RemainingTemplate =
        CompositeFormat.Parse(RemainingFormat);

    private static readonly CompositeFormat ResultCountTemplate =
        CompositeFormat.Parse(ResultCountFormat);

    /// <summary>
    /// The counter's shape, named once. A test asserting "2 remaining of 2"
    /// inline hides the format from every other test, so a copy change breaks
    /// several and none records what the wording used to be.
    /// </summary>
    internal static string RemainingLabel(int remaining, int total) =>
        string.Format(CultureInfo.InvariantCulture, RemainingTemplate, remaining, total);

    internal static string ResultCountLabel(int shown, int total) =>
        string.Format(CultureInfo.InvariantCulture, ResultCountTemplate, shown, total);

    internal static class ValidationMessage
    {
        internal const string NameRequired = "Enter your full name.";
        internal const string EmailRequired = "Enter your work email.";
        internal const string EmailInvalid = "Enter a valid email address.";
        internal const string PlanRequired = "Choose a plan.";
        internal const string TermsRequired = "Accept the terms to continue.";
    }

    internal static class Inventory
    {
        internal const int TotalItems = 6;
        internal const string SearchTerm = "do";
        internal const string MissingTerm = "nothing matches this";

        /// <summary>
        /// How many seeded names contain the search term. One does: "Docking
        /// station". The sibling suite asserts no number here at all, reading
        /// the row count and checking the label agrees with it, which still
        /// passes when a broken filter returns every row. Naming the expected
        /// count costs one constant and catches that.
        /// </summary>
        internal const int SearchMatches = 1;
        internal const string FirstNameAscending = "Access badge";
        internal const string FirstNameDescending = "Webcam";
        internal const string LowestQuantityItem = "Laptop sleeve";
    }
}
