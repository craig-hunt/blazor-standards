using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace BlazorStandards.App.Tests.Conventions;

/// <summary>
/// The half of the no-literals rule an analyzer cannot reach.
/// </summary>
/// <remarks>
/// Razor markup compiles to generated C#, and the literal analyzer skips
/// generated code deliberately: reporting a generated file would point a
/// developer at something they cannot edit. So markup is checked here instead,
/// as text, against the .razor files somebody actually writes.
///
/// Three rules, each one a way the test id contract or the copy rule breaks in
/// practice:
///
///   1. No component writes the attribute name. It lives in TestId.Attribute,
///      and a component spelling it again defeats the single definition.
///   2. Every TestId.For argument names a constant. A raw string there passes
///      the first rule while putting an unsearchable id in the markup.
///   3. No component carries visible prose. Copy lives in constants, so a text
///      node holding words is copy that escaped.
///
/// The scan covers the folders this repository authors. The framework template
/// writes the layout, the reconnect modal, and the error pages, and holding
/// somebody else's generated copy to this rule would mean either editing files
/// the tooling regenerates or weakening the rule until it caught nothing.
///
/// This file names its own patterns and messages for the same reason it exists.
/// The analyzer checks it too: a rule its author exempts themselves from is
/// advice.
/// </remarks>
public sealed class RazorMarkupTests
{
    private const string ComponentsFolder = "Components";
    private const string SourceFolder = "src";
    private const string AppFolder = "App";
    private const string PagesFolder = "Pages";
    private const string SharedFolder = "Shared";
    private const string RazorPattern = "*.razor";
    private const string AttributeName = "data-testid";
    private const string ConstantsPrefix = "TestIds.";
    private const int None = 0;

    /// <summary>
    /// The parameter name components use for a test id a caller supplies. Named
    /// so the check below reads as a rule rather than a magic suffix.
    /// </summary>
    private const string SuppliedTestIdParameter = "ElementTestId";

    private const string ErrorPage = "Error.razor";
    private const string NotFoundPage = "NotFound.razor";

    private const string TestIdCallPattern = @"TestId\.For\(\s*([^)]+?)\s*\)";

    /// <summary>
    /// A Razor directive line: @page, @inject, @using, @inherits, @attribute,
    /// @implements, @typeparam, @layout, @namespace, @rendermode. None of them
    /// renders text.
    /// </summary>
    private const string DirectivePattern =
        @"^\s*@(?:page|inject|using|inherits|attribute|implements|typeparam|layout|namespace|rendermode|preservewhitespace)\b.*$";

    /// <summary>
    /// Razor control flow and its C# head: @foreach (...), @if (...), @else,
    /// @switch, @for, @while, @try, @lock. The braces around them are markup,
    /// the head is code.
    /// </summary>
    private const string ControlFlowPattern =
        @"@(?:foreach|if|else\s+if|else|switch|for|while|do|try|catch|finally|lock)\b(?:\s*\([^)]*\))?";

    /// <summary>
    /// The remaining constructs carrying no prose: comments, expressions, and
    /// tags. Applied after directives and control flow are gone.
    /// </summary>
    private const string NonProsePattern =
        @"@\*.*?\*@|<!--.*?-->|@\([^)]*\)|@[A-Za-z_][\w\.]*(?:\([^)]*\))?|<[^>]*>";

    private const string CodeBlockOpening = "@code";
    private const string WordPattern = @"\p{L}{2,}";
    private const string Separator = ", ";
    private const string MissingComponentsMessage =
        "No ancestor of the test binary contains src/App/Components.";
    private const string RawTestIdFormat =
        "{0} passes {1} to TestId.For rather than a TestIds constant.";
    private const string ProseFormat =
        "{0} renders prose directly: {1}. Copy belongs in the Copy constants class.";

    /// <summary>
    /// Files the framework template owns. Authored components live in Pages and
    /// Shared; these arrive scaffolded and regenerate, so holding them to the
    /// copy rule would mean editing files the tooling rewrites.
    /// </summary>
    private static readonly string[] TemplateOwned = [ErrorPage, NotFoundPage];

    private static readonly Regex TestIdCall = new(
        TestIdCallPattern,
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex Directive = new(
        DirectivePattern,
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex ControlFlow = new(
        ControlFlowPattern,
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex NonProse = new(
        NonProsePattern,
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

    /// <summary>Two or more letters in a row reads as a word rather than punctuation.</summary>
    private static readonly Regex Word = new(
        WordPattern,
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly CompositeFormat RawTestIdTemplate = CompositeFormat.Parse(RawTestIdFormat);
    private static readonly CompositeFormat ProseTemplate = CompositeFormat.Parse(ProseFormat);

    /// <summary>
    /// Gets the application's Components folder, found by walking up from the
    /// test binary so the path survives whichever configuration folder the
    /// build lands in.
    /// </summary>
    private static string ComponentsRoot
    {
        get
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                var candidate = Path.Combine(directory.FullName, SourceFolder, AppFolder, ComponentsFolder);
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException(MissingComponentsMessage);
        }
    }

    public static TheoryData<string> Components()
    {
        var data = new TheoryData<string>();
        foreach (var file in AuthoredComponents())
        {
            data.Add(Path.GetRelativePath(ComponentsRoot, file));
        }

        return data;
    }

    [Fact]
    public void TheScanFindsComponentsToCheck()
    {
        // Guards the suite itself. A path that stopped resolving would leave
        // every theory below passing over an empty set, reporting green while
        // checking nothing.
        Assert.NotEmpty(AuthoredComponents());
    }

    [Theory]
    [MemberData(nameof(Components))]
    public void NoComponentWritesTheTestIdAttribute(string component)
    {
        var markup = ReadComponent(component);

        Assert.DoesNotContain(AttributeName, markup, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [MemberData(nameof(Components))]
    public void EveryTestIdComesFromTheConstantsClass(string component)
    {
        var markup = ReadComponent(component);

        foreach (Match match in TestIdCall.Matches(markup))
        {
            var argument = match.Groups[1].Value;
            var named = argument.StartsWith(ConstantsPrefix, StringComparison.Ordinal)
                || string.Equals(argument, SuppliedTestIdParameter, StringComparison.Ordinal);

            Assert.True(named, string.Format(CultureInfo.InvariantCulture, RawTestIdTemplate, component, argument));
        }
    }

    [Theory]
    [MemberData(nameof(Components))]
    public void NoComponentCarriesVisibleProse(string component)
    {
        var markup = WithoutCodeBlock(ReadComponent(component));
        markup = Directive.Replace(markup, string.Empty);
        markup = ControlFlow.Replace(markup, string.Empty);

        var words = Word.Matches(NonProse.Replace(markup, string.Empty));

        Assert.True(
            words.Count == None,
            string.Format(
                CultureInfo.InvariantCulture,
                ProseTemplate,
                component,
                string.Join(Separator, words.Select(word => word.Value))));
    }

    /// <summary>
    /// The components this repository authors: everything under Pages and
    /// Shared, less the pages the framework template generates.
    /// </summary>
    private static List<string> AuthoredComponents()
    {
        List<string> files = [];

        foreach (var folder in new[] { PagesFolder, SharedFolder })
        {
            var path = Path.Combine(ComponentsRoot, folder);
            if (!Directory.Exists(path))
            {
                continue;
            }

            files.AddRange(
                Directory.EnumerateFiles(path, RazorPattern, SearchOption.AllDirectories)
                    .Where(file => !TemplateOwned.Contains(Path.GetFileName(file), StringComparer.Ordinal)));
        }

        return files;
    }

    /// <summary>
    /// Drops everything from the @code directive onward.
    /// </summary>
    /// <remarks>
    /// A brace-counting regex cannot follow a code block that nests a class
    /// inside a method, and the C# it leaves behind reads as prose to the word
    /// check. The directive always sits last in these components, so cutting at
    /// it is both simpler and correct.
    /// </remarks>
    private static string WithoutCodeBlock(string markup)
    {
        var start = markup.IndexOf(CodeBlockOpening, StringComparison.Ordinal);
        return start < None ? markup : markup[..start];
    }

    private static string ReadComponent(string relativePath) =>
        File.ReadAllText(Path.Combine(ComponentsRoot, relativePath));
}
