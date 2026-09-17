using System.Collections.Immutable;
using BlazorStandards.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace BlazorStandards.Analyzer.Tests;

/// <summary>
/// What the literal rule reports, and what it leaves alone.
/// </summary>
/// <remarks>
/// The analyzer runs against compiled snippets through Roslyn rather than a
/// testing harness, which keeps a second test framework out of the repository
/// and shows plainly what the compiler hands the rule.
///
/// This project references the analyzer as a library rather than as an
/// analyzer, so the rule does not run over its own test data.
/// </remarks>
public sealed class LiteralAnalyzerTests
{
    private const string CompilationName = "Snippet";
    private const string TrustedAssemblies = "TRUSTED_PLATFORM_ASSEMBLIES";

    private const string LiteralInAMethod = """
        internal static class Sample
        {
            public static string Describe() => "a meaningful value";
        }
        """;

    private const string NumberInAMethod = """
        internal static class Sample
        {
            public static int Limit() => 200;
        }
        """;

    private const string InterpolatedInAMethod = """
        internal static class Sample
        {
            public static string Greet(string name) => $"Hello {name}";
        }
        """;

    private const string TrivialValues = """
        internal static class Sample
        {
            public static string Blank() => "";
            public static int Zero() => 0;
            public static int One() => 1;
        }
        """;

    private const string NamedConstants = """
        internal static class Sample
        {
            public const string Key = "configuration-key";
            private const int Limit = 200;
            public static int Reveal() => Limit;
        }
        """;

    private const string LocalConstant = """
        internal static class Sample
        {
            public static int Limit()
            {
                const int Ceiling = 200;
                return Ceiling;
            }
        }
        """;

    private const string AttributeArgument = """
        using System;

        internal sealed class SampleAttribute : Attribute
        {
            public SampleAttribute(string name) => Name = name;

            public string Name { get; }
        }

        [Sample("configuration")]
        internal static class Decorated
        {
        }
        """;

    private const string EnumMembers = """
        internal enum Level
        {
            Low = 5,
            High = 200,
        }
        """;

    private const string DefaultParameterValue = """
        internal static class Sample
        {
            public static int Grow(int amount = 25) => amount;
        }
        """;

    private const string SwitchCaseLabel = """
        internal static class Sample
        {
            public static int Rank(string value)
            {
                switch (value)
                {
                    case "alpha": return 1;
                    default: return 0;
                }
            }
        }
        """;

    /// <summary>
    /// A component parameter's default value. Razor exposes inputs as
    /// properties, so this is where a stray literal hides in a Blazor project
    /// specifically.
    /// </summary>
    private const string PropertyDefault = """
        internal sealed class Sample
        {
            public string Label { get; set; } = "Add a task";
        }
        """;

    [Fact]
    public async Task ReportsAStringLiteralInAMethod()
    {
        var reported = await AnalyzeAsync(LiteralInAMethod);

        var single = Assert.Single(reported);
        Assert.Equal(LiteralAnalyzer.DiagnosticId, single.Id);
    }

    [Fact]
    public async Task ReportsANumberInAMethod() =>
        Assert.Single(await AnalyzeAsync(NumberInAMethod));

    [Fact]
    public async Task ReportsAnInterpolatedStringInAMethod() =>
        Assert.Single(await AnalyzeAsync(InterpolatedInAMethod));

    [Fact]
    public async Task ReportsAPropertyInitializer() =>
        Assert.Single(await AnalyzeAsync(PropertyDefault));

    [Fact]
    public async Task LeavesTrivialValuesAlone() =>
        Assert.Empty(await AnalyzeAsync(TrivialValues));

    [Fact]
    public async Task LeavesNamedConstantsAlone() =>
        Assert.Empty(await AnalyzeAsync(NamedConstants));

    [Fact]
    public async Task LeavesALocalConstantAlone() =>
        Assert.Empty(await AnalyzeAsync(LocalConstant));

    [Fact]
    public async Task LeavesAnAttributeArgumentAlone() =>
        Assert.Empty(await AnalyzeAsync(AttributeArgument));

    [Fact]
    public async Task LeavesEnumMembersAlone() =>
        Assert.Empty(await AnalyzeAsync(EnumMembers));

    [Fact]
    public async Task LeavesADefaultParameterValueAlone() =>
        Assert.Empty(await AnalyzeAsync(DefaultParameterValue));

    [Fact]
    public async Task LeavesASwitchCaseLabelAlone() =>
        Assert.Empty(await AnalyzeAsync(SwitchCaseLabel));

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source)
    {
        var trusted = (string?)AppContext.GetData(TrustedAssemblies) ?? string.Empty;
        var references = trusted
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));

        var compilation = CSharpCompilation.Create(
            CompilationName,
            [CSharpSyntaxTree.ParseText(source)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new LiteralAnalyzer());
        return await compilation
            .WithAnalyzers(analyzers)
            .GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);
    }
}
