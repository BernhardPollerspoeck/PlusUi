using System.Collections;

namespace PlusUi.core.Binding;

/// <summary>
/// A resolved binding path: the property names from the root context to the bound leaf, plus
/// reflection-free accessor delegates used to traverse the intermediate objects when subscribing
/// to their change notifications.
/// </summary>
/// <remarks>
/// Enumerating a <see cref="BindingPath"/> yields its <see cref="Segments"/>, and it converts
/// implicitly to <c>string[]</c>, so it is a drop-in replacement for the plain segment array that
/// the binding APIs used previously.
/// </remarks>
public sealed class BindingPath(string[] segments, Func<object, object?>?[] segmentAccessors) : IEnumerable<string>
{
    /// <summary>Property names from the root context to the bound leaf property.</summary>
    public string[] Segments { get; } = segments;

    /// <summary>
    /// One entry per segment: <c>SegmentAccessors[i]</c> reads <c>Segments[i]</c> from the object at
    /// depth <c>i</c> and returns the object at depth <c>i+1</c>. The leaf segment and any segment that
    /// cannot be resolved from a simple member chain (complex expressions) are <c>null</c>.
    /// Accessors are built from the expression's member metadata, so traversal needs no runtime
    /// reflection and stays Native-AOT / trimming safe.
    /// </summary>
    public Func<object, object?>?[] SegmentAccessors { get; } = segmentAccessors;

    public static BindingPath Empty { get; } = new([], []);

    public static implicit operator string[](BindingPath path) => path.Segments;

    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)Segments).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Segments.GetEnumerator();
}
