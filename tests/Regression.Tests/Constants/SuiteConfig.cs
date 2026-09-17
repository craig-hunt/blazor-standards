using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BlazorStandards.Regression.Tests.Constants;

/// <summary>
/// Values the run itself needs, before any test executes.
/// </summary>
/// <remarks>
/// Two sources, in precedence order:
///
///   1. User secrets, for a developer's own machine. Stored in the user
///      profile, never in the repository.
///   2. Environment variables, which win. Cadenced and deployed runs inject
///      through the platform.
///
/// Environment variables override user secrets deliberately: a developer can
/// point one run at a deployed environment without editing stored secrets, and
/// CI never reads a developer's machine.
/// </remarks>
internal static class SuiteConfig
{
    internal const int ApplicationPort = 5280;

    /// <summary>
    /// One attribute, named once. The base class hands it to Playwright, so
    /// page objects call GetByTestId and never write the attribute again. It
    /// matches TestId.Attribute in the application, which is the whole contract.
    /// </summary>
    internal const string TestIdAttribute = "data-testid";

    internal const string BaseUrlKey = "App:BaseUrl";
    internal const string LocalHostFormat = "http://localhost:{0}";

    /// <summary>
    /// How long the fixture waits for the application to answer. A Blazor host
    /// starting cold on a build agent takes longer than a static file server,
    /// which is what the sibling suites wait on.
    /// </summary>
    internal const int StartTimeoutSeconds = 60;

    internal static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(StartTimeoutSeconds);

    private static readonly CompositeFormat LocalHostTemplate = CompositeFormat.Parse(LocalHostFormat);

    private static readonly IConfigurationRoot Configuration =
        new ConfigurationBuilder()
            .AddUserSecrets(typeof(SuiteConfig).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build();

    /// <summary>
    /// Gets where the suite points. Defaults to the bundled application, so a
    /// fresh clone runs with no configuration at all.
    /// </summary>
    internal static string BaseUrl =>
        Configuration[BaseUrlKey]
        ?? string.Format(CultureInfo.InvariantCulture, LocalHostTemplate, ApplicationPort);
}
