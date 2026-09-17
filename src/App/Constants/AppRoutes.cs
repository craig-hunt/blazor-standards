namespace BlazorStandards.App.Constants;

/// <summary>
/// Every path the application serves.
/// </summary>
/// <remarks>
/// The sibling static-site projects serve /signup.html and /inventory.html
/// because they ship files. Blazor routes rather than serving files, so the
/// extension goes. That divergence is deliberate and the README records it: a
/// suite written against this application uses these paths, and a suite written
/// against the static demo uses those.
/// </remarks>
internal static class AppRoutes
{
    internal const string Tasks = "/";
    internal const string Signup = "/signup";
    internal const string Inventory = "/inventory";
}
