namespace PlusUi.core;

/// <summary>
/// A styled range of characters within a document, addressed by absolute character offsets.
/// Produced by a <see cref="CodeEditor"/> highlighter callback.
/// Properties left null inherit from the editor.
/// </summary>
/// <param name="Start">Absolute character index into the document where the span begins.</param>
/// <param name="Length">Number of characters covered by the span.</param>
/// <param name="Color">Foreground color, or null to inherit the editor's TextColor.</param>
/// <param name="FontWeight">Font weight, or null to inherit the editor's FontWeight.</param>
/// <param name="FontStyle">Font style, or null to inherit the editor's FontStyle.</param>
public readonly record struct StyleSpan(
    int Start,
    int Length,
    Color? Color = null,
    FontWeight? FontWeight = null,
    FontStyle? FontStyle = null)
{
    /// <summary>Exclusive end offset of the span.</summary>
    public int End => Start + Length;
}

/// <summary>
/// Highlights a single line of a <see cref="CodeEditor"/> document.
/// </summary>
/// <remarks>
/// Lines are tokenized in order and each call receives the state the previous line ended in, which
/// is what lets constructs that span lines - block comments, verbatim strings - be highlighted
/// correctly. State 0 is the state at the start of the document.
///
/// The editor caches the result per line and only re-runs lines whose text or incoming state
/// changed, so editing one line of a large file re-tokenizes one line rather than the whole document.
/// For that cache to converge, the same (lineText, stateIn) must always produce the same result.
/// </remarks>
/// <param name="lineText">The text of the line, without its newline.</param>
/// <param name="lineIndex">Zero-based index of the line in the document.</param>
/// <param name="stateIn">The state returned for the previous line, or 0 for the first line.</param>
/// <param name="output">
/// A cleared list to fill with spans. Offsets are relative to the start of this line.
/// The list is owned by the editor and reused across calls - do not keep a reference to it.
/// </param>
/// <returns>The state the next line should start in.</returns>
public delegate int LineHighlighter(string lineText, int lineIndex, int stateIn, List<StyleSpan> output);
