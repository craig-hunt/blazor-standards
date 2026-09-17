namespace BlazorStandards.App;

/// <summary>
/// The paths the pipeline re-executes to when a request fails or matches
/// nothing.
/// </summary>
/// <remarks>
/// Named here rather than inline because the composition root and the routable
/// components both refer to them, and a path spelled twice drifts once.
/// </remarks>
internal static class ErrorRoutes
{
    internal const string Error = "/Error";
    internal const string NotFound = "/not-found";
}
