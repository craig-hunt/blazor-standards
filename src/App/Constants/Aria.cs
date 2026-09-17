namespace BlazorStandards.App.Constants;

/// <summary>
/// Platform attribute names and values the markup sets and the suite asserts.
/// </summary>
/// <remarks>
/// Named for the same reason domain strings are: a typo in a literal produces
/// markup that looks right and an assertion that checks nothing.
/// </remarks>
internal static class Aria
{
    internal static class Attribute
    {
        internal const string Label = "aria-label";
        internal const string Live = "aria-live";
        internal const string Invalid = "aria-invalid";
        internal const string Sort = "aria-sort";
        internal const string Hidden = "hidden";
        internal const string Role = "role";
    }

    internal static class Value
    {
        internal const string True = "true";
        internal const string Polite = "polite";
        internal const string Group = "group";
        internal const string Status = "status";
        internal const string Ascending = "ascending";
        internal const string Descending = "descending";
        internal const string None = "none";
    }
}
