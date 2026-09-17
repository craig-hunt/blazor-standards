using System.Collections.ObjectModel;

namespace BlazorStandards.App.Components;

/// <summary>
/// Renders the test id attribute onto an element.
/// </summary>
/// <remarks>
/// A component writes this:
///
///     &lt;button @attributes="TestId.For(TestIds.Task.AddButton)"&gt;
///
/// and never writes the attribute name. The name lives in one constant here, so
/// changing it changes this file and nothing else. Without a helper, the name
/// appears in every element a test reaches, and a rename becomes a search across
/// the markup that quietly misses one.
///
/// The returned dictionary is read-only and built per call. Blazor splats it
/// through @attributes, and the allocation is a dictionary of one entry on a
/// render pass that already allocates a frame per attribute.
/// </remarks>
internal static class TestId
{
    /// <summary>
    /// The attribute the suites bind to, named once for the whole application.
    /// </summary>
    /// <remarks>
    /// The regression suite hands this same value to Playwright through
    /// SetTestIdAttribute, so the application and the suite cannot disagree
    /// about which attribute carries the contract.
    /// </remarks>
    internal const string Attribute = "data-testid";

    internal static IReadOnlyDictionary<string, object> For(string id) =>
        new ReadOnlyDictionary<string, object>(
            new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [Attribute] = id,
            });
}
