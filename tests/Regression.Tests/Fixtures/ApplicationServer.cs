using System.Diagnostics;
using System.Globalization;
using System.Text;
using BlazorStandards.Regression.Tests.Constants;
using Xunit;

namespace BlazorStandards.Regression.Tests.Fixtures;

/// <summary>
/// Starts the application once for the whole run and stops it afterward.
/// </summary>
/// <remarks>
/// The sibling suites serve a static folder with a small Node script. This
/// application is a server, so the fixture starts Kestrel instead. An in-process
/// test host would not do: Interactive Server needs a real URL for a browser to
/// open a SignalR circuit against.
///
/// It reuses an already-running server rather than failing, which happens when a
/// developer left one up, and it declines to stop one it did not start.
/// </remarks>
public sealed class ApplicationServer : IAsyncLifetime
{
    private const string Command = "dotnet";
    private const string RunArgument = "run";
    private const string ProjectArgument = "--project";
    private const string UrlsArgument = "--urls";
    private const string NoBuildArgument = "--no-build";
    private const string ProjectPath = "src/App/App.csproj";
    private const string SourceFolder = "src";
    private const string AppFolder = "App";
    private const string TimeoutFormat = "The application did not answer at {0} within {1} seconds.";
    private const string MissingApplicationMessage = "No ancestor of the test binary contains src/App.";
    private const int PollMilliseconds = 250;
    private const int ProbeTimeoutSeconds = 3;

    private static readonly CompositeFormat TimeoutTemplate = CompositeFormat.Parse(TimeoutFormat);

    private Process? _server;

    /// <summary>
    /// Gets the directory holding the application, found by walking up from the
    /// test binary so the path survives whichever configuration folder the build
    /// lands in.
    /// </summary>
    private static string RepositoryRoot
    {
        get
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null
                && !Directory.Exists(Path.Combine(directory.FullName, SourceFolder, AppFolder)))
            {
                directory = directory.Parent;
            }

            return directory?.FullName
                ?? throw new DirectoryNotFoundException(MissingApplicationMessage);
        }
    }

    public async ValueTask InitializeAsync()
    {
        if (await RespondsAsync().ConfigureAwait(false))
        {
            return;
        }

        _server = Process.Start(new ProcessStartInfo(Command)
        {
            ArgumentList =
            {
                RunArgument,
                ProjectArgument,
                Path.Combine(RepositoryRoot, ProjectPath),
                NoBuildArgument,
                UrlsArgument,
                SuiteConfig.BaseUrl,
            },
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = RepositoryRoot,
        });

        Assert.NotNull(_server);
        await WaitUntilRespondingAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        // Null when this fixture reused a server somebody else started, and
        // stopping that one would surprise them.
        if (_server is null || _server.HasExited)
        {
            return ValueTask.CompletedTask;
        }

        _server.Kill(entireProcessTree: true);
        _server.Dispose();
        return ValueTask.CompletedTask;
    }

    private static async Task<bool> RespondsAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(ProbeTimeoutSeconds) };

        try
        {
            using var response = await client.GetAsync(new Uri(SuiteConfig.BaseUrl)).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return false;
        }
    }

    private static async Task WaitUntilRespondingAsync()
    {
        var deadline = DateTime.UtcNow + SuiteConfig.StartTimeout;

        while (DateTime.UtcNow < deadline)
        {
            if (await RespondsAsync().ConfigureAwait(false))
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(PollMilliseconds)).ConfigureAwait(false);
        }

        throw new TimeoutException(string.Format(
            CultureInfo.InvariantCulture,
            TimeoutTemplate,
            SuiteConfig.BaseUrl,
            SuiteConfig.StartTimeout.TotalSeconds));
    }
}

/// <summary>
/// Binds the server fixture to every spec, so one application serves the run.
/// </summary>
/// <remarks>
/// Named a definition rather than a collection because the analyzer reserves
/// the Collection suffix for types that are one, and this marks a definition
/// xUnit reads by attribute.
/// </remarks>
[CollectionDefinition(Name)]
public sealed class ApplicationFixtureDefinition : ICollectionFixture<ApplicationServer>
{
    internal const string Name = "application";
}
