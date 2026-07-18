using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public sealed class CodeEditorTests
{
    private static CodeEditor CreateEditor(string text = "")
    {
        var editor = new CodeEditor().SetText(text);
        editor.Measure(new Size(400, 200));
        editor.Arrange(new Rect(new Point(0, 0), new Size(400, 200)));
        editor.SetSelectionStatus(true);
        return editor;
    }

    #region SliceLine

    [TestMethod]
    public void TestSliceLine_NoSpans_ReturnsSingleUnstyledFragment()
    {
        // Arrange
        var spans = Array.Empty<StyleSpan>();

        // Act
        var fragments = CodeEditor.SliceLine(lineStart: 0, lineLength: 5, spans);

        // Assert
        Assert.HasCount(1, fragments);
        Assert.IsNull(fragments[0].Style);
        Assert.AreEqual(0, fragments[0].Offset);
        Assert.AreEqual(5, fragments[0].Length);
    }

    [TestMethod]
    public void TestSliceLine_EmptyLine_ReturnsNoFragments()
    {
        // Arrange
        var spans = new[] { new StyleSpan(0, 10, Colors.Red) };

        // Act
        var fragments = CodeEditor.SliceLine(lineStart: 0, lineLength: 0, spans);

        // Assert
        Assert.IsEmpty(fragments);
    }

    [TestMethod]
    public void TestSliceLine_GapBetweenSpans_BecomesUnstyledFragment()
    {
        // Arrange: "var x = 1" -> style "var" (0..3) and "1" (8..9), gap in between
        var spans = new[]
        {
            new StyleSpan(0, 3, Colors.Blue),
            new StyleSpan(8, 1, Colors.Green)
        };

        // Act
        var fragments = CodeEditor.SliceLine(lineStart: 0, lineLength: 9, spans);

        // Assert
        Assert.HasCount(3, fragments);
        Assert.AreEqual(Colors.Blue, fragments[0].Style!.Value.Color);
        Assert.IsNull(fragments[1].Style);
        Assert.AreEqual(3, fragments[1].Offset);
        Assert.AreEqual(5, fragments[1].Length);
        Assert.AreEqual(Colors.Green, fragments[2].Style!.Value.Color);
    }

    [TestMethod]
    public void TestSliceLine_SpanCrossingLineBoundary_IsClippedToLine()
    {
        // Arrange: a block comment span covering offsets 5..25, line 2 covers 10..20
        var spans = new[] { new StyleSpan(5, 20, Colors.Green) };

        // Act
        var fragments = CodeEditor.SliceLine(lineStart: 10, lineLength: 10, spans);

        // Assert
        Assert.HasCount(1, fragments);
        Assert.AreEqual(0, fragments[0].Offset);
        Assert.AreEqual(10, fragments[0].Length);
        Assert.AreEqual(Colors.Green, fragments[0].Style!.Value.Color);
    }

    [TestMethod]
    public void TestSliceLine_OverlappingSpans_FirstWins()
    {
        // Arrange
        var spans = new[]
        {
            new StyleSpan(0, 6, Colors.Blue),
            new StyleSpan(3, 6, Colors.Red)
        };

        // Act
        var fragments = CodeEditor.SliceLine(lineStart: 0, lineLength: 9, spans);

        // Assert
        Assert.HasCount(2, fragments);
        Assert.AreEqual(Colors.Blue, fragments[0].Style!.Value.Color);
        Assert.AreEqual(6, fragments[0].Length);
        Assert.AreEqual(Colors.Red, fragments[1].Style!.Value.Color);
        Assert.AreEqual(6, fragments[1].Offset);
        Assert.AreEqual(3, fragments[1].Length);
    }

    [TestMethod]
    public void TestSliceLine_UnsortedSpans_AreOrderedByOffset()
    {
        // Arrange
        var spans = new[]
        {
            new StyleSpan(6, 3, Colors.Green),
            new StyleSpan(0, 3, Colors.Blue)
        };

        // Act
        var fragments = CodeEditor.SliceLine(lineStart: 0, lineLength: 9, spans);

        // Assert
        Assert.AreEqual(Colors.Blue, fragments[0].Style!.Value.Color);
        Assert.AreEqual(Colors.Green, fragments[^1].Style!.Value.Color);
    }

    [TestMethod]
    public void TestSliceLine_FragmentsAlwaysCoverLineExactlyWithoutOverlap()
    {
        // Arrange: messy spans - overlapping, out of order, out of range, zero length
        var spans = new[]
        {
            new StyleSpan(0, 4, Colors.Blue),
            new StyleSpan(2, 5, Colors.Red),
            new StyleSpan(30, 5, Colors.Green),
            new StyleSpan(8, 0, Colors.White),
            new StyleSpan(9, 100, Colors.Yellow)
        };
        const int lineLength = 20;

        // Act
        var fragments = CodeEditor.SliceLine(lineStart: 0, lineLength, spans);

        // Assert: contiguous from 0, no gaps, no overlaps, ends exactly at lineLength
        var expectedOffset = 0;
        foreach (var fragment in fragments)
        {
            Assert.AreEqual(expectedOffset, fragment.Offset);
            Assert.IsGreaterThan(0, fragment.Length);
            expectedOffset += fragment.Length;
        }
        Assert.AreEqual(lineLength, expectedOffset);
    }

    #endregion

    #region Tabs

    [TestMethod]
    public void TestExpandTabs_ReplacesTabWithTabSizeSpaces()
    {
        // Act
        var result = CodeEditor.ExpandTabs("a\tb", 4);

        // Assert
        Assert.AreEqual("a    b", result);
    }

    [TestMethod]
    public void TestSetText_ExpandsTabsSoIndicesMatchColumns()
    {
        // Arrange & Act
        var editor = CreateEditor("if\ttrue");

        // Assert
        Assert.AreEqual("if    true", editor.Text);
    }

    [TestMethod]
    public void TestTypingTabCharacter_InsertsSpaces()
    {
        // Arrange
        var editor = CreateEditor();

        // Act
        editor.HandleInput('\t');

        // Assert
        Assert.AreEqual("    ", editor.Text);
    }

    [TestMethod]
    public void TestTabKey_InsertsIndentAtCaret()
    {
        // Arrange
        var editor = CreateEditor();

        // Act
        editor.HandleInput(PlusKey.Tab, shift: false, ctrl: false);

        // Assert
        Assert.AreEqual("    ", editor.Text);
    }

    #endregion

    #region Indentation

    [TestMethod]
    public void TestTabKey_WithMultiLineSelection_IndentsEveryLine()
    {
        // Arrange
        var editor = CreateEditor("a\nb\nc");
        editor.HandleInput(PlusKey.A, shift: false, ctrl: true);

        // Act
        editor.HandleInput(PlusKey.Tab, shift: false, ctrl: false);

        // Assert
        Assert.AreEqual("    a\n    b\n    c", editor.Text);
    }

    [TestMethod]
    public void TestShiftTab_RemovesOneIndentStepFromEveryLine()
    {
        // Arrange
        var editor = CreateEditor("        a\n    b");
        editor.HandleInput(PlusKey.A, shift: false, ctrl: true);

        // Act
        editor.HandleInput(PlusKey.ShiftTab, shift: true, ctrl: false);

        // Assert
        Assert.AreEqual("    a\nb", editor.Text);
    }

    [TestMethod]
    public void TestShiftTab_OnUnindentedLine_LeavesTextUnchanged()
    {
        // Arrange
        var editor = CreateEditor("a");

        // Act
        editor.HandleInput(PlusKey.ShiftTab, shift: true, ctrl: false);

        // Assert
        Assert.AreEqual("a", editor.Text);
    }

    [TestMethod]
    public void TestEnter_AutoIndent_CopiesLeadingWhitespaceOfCurrentLine()
    {
        // Arrange
        var editor = CreateEditor("    foo");
        editor.HandleInput(PlusKey.End, shift: false, ctrl: false);

        // Act
        editor.HandleInput(PlusKey.Enter, shift: false, ctrl: false);

        // Assert
        Assert.AreEqual("    foo\n    ", editor.Text);
    }

    [TestMethod]
    public void TestEnter_AutoIndentDisabled_StartsAtColumnZero()
    {
        // Arrange
        var editor = CreateEditor("    foo").SetAutoIndent(false);
        editor.HandleInput(PlusKey.End, shift: false, ctrl: false);

        // Act
        editor.HandleInput(PlusKey.Enter, shift: false, ctrl: false);

        // Assert
        Assert.AreEqual("    foo\n", editor.Text);
    }

    [TestMethod]
    public void TestBackspace_InsideLeadingIndent_RemovesWholeIndentStep()
    {
        // Arrange
        var editor = CreateEditor("        ");
        editor.HandleInput(PlusKey.End, shift: false, ctrl: false);

        // Act
        editor.HandleInput(PlusKey.Backspace, shift: false, ctrl: false);

        // Assert
        Assert.AreEqual("    ", editor.Text);
    }

    [TestMethod]
    public void TestBackspace_InsideText_RemovesSingleCharacter()
    {
        // Arrange
        var editor = CreateEditor("foo");
        editor.HandleInput(PlusKey.End, shift: false, ctrl: false);

        // Act
        editor.HandleInput(PlusKey.Backspace, shift: false, ctrl: false);

        // Assert
        Assert.AreEqual("fo", editor.Text);
    }

    #endregion

    #region Highlighter

    [TestMethod]
    public void TestHighlighter_IsNotReinvokedWhileTextIsUnchanged()
    {
        // Arrange
        var callCount = 0;
        var editor = CreateEditor("abc")
            .SetHighlighter(_ =>
            {
                callCount++;
                return [new StyleSpan(0, 3, Colors.Red)];
            });

        // Act
        editor.GetSpans();
        editor.GetSpans();
        editor.GetSpans();

        // Assert
        Assert.AreEqual(1, callCount);
    }

    [TestMethod]
    public void TestHighlighter_IsReinvokedAfterTextChanges()
    {
        // Arrange
        var callCount = 0;
        var editor = CreateEditor("abc")
            .SetHighlighter(_ =>
            {
                callCount++;
                return [];
            });
        editor.GetSpans();

        // Act
        editor.HandleInput('d');
        editor.GetSpans();

        // Assert
        Assert.AreEqual(2, callCount);
    }

    [TestMethod]
    public void TestHighlighter_ThatThrows_FallsBackToUnstyledTextInsteadOfCrashing()
    {
        // Arrange
        var editor = CreateEditor("abc")
            .SetHighlighter(_ => throw new InvalidOperationException("broken tokenizer"));

        // Act
        var spans = editor.GetSpans();

        // Assert
        Assert.IsEmpty(spans);
    }

    [TestMethod]
    public void TestHighlighter_SpansSurviveEditsWithoutRemapping()
    {
        // Arrange: highlighter always colors the first word, wherever it is
        var editor = CreateEditor("var x")
            .SetHighlighter(text =>
            {
                var end = text.IndexOf(' ');
                return end <= 0 ? [] : [new StyleSpan(0, end, Colors.Blue)];
            });

        // Act: insert a character at the very start, which would break stored offsets
        editor.HandleInput(PlusKey.Home, shift: false, ctrl: true);
        editor.HandleInput('n');
        var spans = editor.GetSpans();

        // Assert: the span still covers the (now longer) first word
        Assert.HasCount(1, spans);
        Assert.AreEqual(0, spans[0].Start);
        Assert.AreEqual(4, spans[0].Length);
    }

    #endregion

    #region Line highlighter

    /// <summary>A tiny stateful highlighter: "&lt;" opens a region, "&gt;" closes it.</summary>
    private static LineHighlighter RegionHighlighter(List<int>? tokenizedLines = null) =>
        (line, index, state, output) =>
        {
            tokenizedLines?.Add(index);

            if (line.Contains('<')) state = 1;
            if (state == 1) output.Add(new StyleSpan(0, line.Length, Colors.Green));
            if (line.Contains('>')) state = 0;

            return state;
        };

    [TestMethod]
    public void TestLineHighlighter_SpansAreRelativeToTheLine()
    {
        // Arrange
        var editor = CreateEditor("abc\ndefgh")
            .SetLineHighlighter((line, _, state, output) =>
            {
                output.Add(new StyleSpan(0, line.Length, Colors.Red));
                return state;
            });

        // Act
        var spans = editor.GetLineSpans(1);

        // Assert: offset 0 means "start of line 1", not "start of document"
        Assert.HasCount(1, spans);
        Assert.AreEqual(0, spans[0].Start);
        Assert.AreEqual(5, spans[0].Length);
    }

    [TestMethod]
    public void TestLineHighlighter_StateCarriesAcrossLines()
    {
        // Arrange: line 0 opens a region, line 3 closes it
        var editor = CreateEditor("<open\ninside\nstill inside\nclose>\nafter")
            .SetLineHighlighter(RegionHighlighter());

        // Act & Assert: the middle lines are styled purely because of carried state
        Assert.IsNotEmpty(editor.GetLineSpans(1));
        Assert.IsNotEmpty(editor.GetLineSpans(2));
        Assert.IsNotEmpty(editor.GetLineSpans(3));
        Assert.IsEmpty(editor.GetLineSpans(4));
    }

    [TestMethod]
    public void TestLineHighlighter_TokenizesEachLineOnceForUnchangedText()
    {
        // Arrange
        var tokenized = new List<int>();
        var editor = CreateEditor("a\nb\nc").SetLineHighlighter(RegionHighlighter(tokenized));
        editor.GetLineSpans(0);
        tokenized.Clear();

        // Act: repeated reads must not re-tokenize anything
        editor.GetLineSpans(0);
        editor.GetLineSpans(2);

        // Assert
        Assert.IsEmpty(tokenized);
    }

    [TestMethod]
    public void TestLineHighlighter_EditingOneLineDoesNotRetokenizeEarlierLines()
    {
        // Arrange
        var tokenized = new List<int>();
        var editor = CreateEditor("aaa\nbbb\nccc").SetLineHighlighter(RegionHighlighter(tokenized));
        editor.GetLineSpans(0);
        tokenized.Clear();

        // Act: append to the last line
        editor.HandleInput(PlusKey.End, shift: false, ctrl: true);
        editor.HandleInput('x');
        editor.GetLineSpans(2);

        // Assert: lines 0 and 1 came from the cache
        CollectionAssert.DoesNotContain(tokenized, 0);
        CollectionAssert.DoesNotContain(tokenized, 1);
        CollectionAssert.Contains(tokenized, 2);
    }

    [TestMethod]
    public void TestLineHighlighter_EditingAboveStopsRetokenizingOnceStateConverges()
    {
        // Arrange: a long stateless document
        var tokenized = new List<int>();
        var editor = CreateEditor(string.Join('\n', Enumerable.Repeat("plain", 50)))
            .SetLineHighlighter(RegionHighlighter(tokenized));
        editor.GetLineSpans(49);
        tokenized.Clear();

        // Act: edit the very first line
        editor.HandleInput(PlusKey.Home, shift: false, ctrl: true);
        editor.HandleInput('x');
        editor.GetLineSpans(49);

        // Assert: only the changed line re-runs - the rest match text and incoming state
        CollectionAssert.Contains(tokenized, 0);
        Assert.HasCount(1, tokenized);
    }

    [TestMethod]
    public void TestLineHighlighter_OpeningARegionRestylesTheLinesBelow()
    {
        // Arrange
        var editor = CreateEditor("one\ntwo\nthree").SetLineHighlighter(RegionHighlighter());
        Assert.IsEmpty(editor.GetLineSpans(2), "precondition: no region open yet");

        // Act: turn line 0 into a region opener
        editor.HandleInput(PlusKey.Home, shift: false, ctrl: true);
        editor.HandleInput('<');

        // Assert: the carried state must have propagated down
        Assert.IsNotEmpty(editor.GetLineSpans(2));
    }

    [TestMethod]
    public void TestLineHighlighter_DeletingLinesShrinksTheCache()
    {
        // Arrange
        var editor = CreateEditor("a\nb\nc").SetLineHighlighter(RegionHighlighter());
        editor.GetLineSpans(2);

        // Act
        editor.HandleInput(PlusKey.A, shift: false, ctrl: true);
        editor.HandleInput('z');

        // Assert: no stale entry for a line that no longer exists
        Assert.IsEmpty(editor.GetLineSpans(2));
    }

    [TestMethod]
    public void TestLineHighlighter_ThatThrows_LeavesLineUnstyledWithoutCrashing()
    {
        // Arrange
        var editor = CreateEditor("a\nb")
            .SetLineHighlighter((_, _, _, _) => throw new InvalidOperationException("broken"));

        // Act
        var spans = editor.GetLineSpans(0);

        // Assert
        Assert.IsEmpty(spans);
    }

    [TestMethod]
    public void TestSetLineHighlighter_ReplacesADocumentHighlighter()
    {
        // Arrange
        var editor = CreateEditor("abc")
            .SetHighlighter(_ => [new StyleSpan(0, 3, Colors.Red)]);

        // Act
        editor.SetLineHighlighter((line, _, state, output) =>
        {
            output.Add(new StyleSpan(0, line.Length, Colors.Blue));
            return state;
        });

        // Assert: the two modes must not both feed the render path
        Assert.IsEmpty(editor.GetSpans());
        Assert.IsNotEmpty(editor.GetLineSpans(0));
    }

    [TestMethod]
    public void TestRender_WithLineHighlighter_DoesNotThrow()
    {
        // Arrange
        var editor = CreateEditor("<open\ninside\nclose>").SetLineHighlighter(RegionHighlighter());

        // Act & Assert
        RenderToBitmap(editor);
    }

    #endregion

    #region Editing and line math

    [TestMethod]
    public void TestGetLines_SplitsOnNewlines()
    {
        // Arrange
        var editor = CreateEditor("a\nbb\nccc");

        // Act
        var lines = editor.GetLines();

        // Assert
        Assert.HasCount(3, lines);
        Assert.AreEqual("ccc", lines[2]);
    }

    [TestMethod]
    public void TestGetLineAndColumn_ResolvesPositionAcrossLines()
    {
        // Arrange
        var editor = CreateEditor("ab\ncd");

        // Act
        var (line, column) = editor.GetLineAndColumn(4);

        // Assert
        Assert.AreEqual(1, line);
        Assert.AreEqual(1, column);
    }

    [TestMethod]
    public void TestCursorPositionFromLineColumn_ClampsToLineLength()
    {
        // Arrange
        var editor = CreateEditor("ab\nc");

        // Act
        var position = editor.GetCursorPositionFromLineColumn(1, 99);

        // Assert
        Assert.AreEqual(4, position);
    }

    [TestMethod]
    public void TestSelectAllThenType_ReplacesWholeDocument()
    {
        // Arrange
        var editor = CreateEditor("old content");

        // Act
        editor.HandleInput(PlusKey.A, shift: false, ctrl: true);
        editor.HandleInput('x');

        // Assert
        Assert.AreEqual("x", editor.Text);
    }

    [TestMethod]
    public void TestIsReadOnly_BlocksEditingButAllowsNavigation()
    {
        // Arrange
        var editor = CreateEditor("code").SetIsReadOnly(true);

        // Act
        editor.HandleInput('x');
        editor.HandleInput(PlusKey.Backspace, shift: false, ctrl: false);
        editor.HandleInput(PlusKey.End, shift: false, ctrl: false);

        // Assert
        Assert.AreEqual("code", editor.Text);
        Assert.AreEqual(4, editor.GetLineAndColumn(4).column);
    }

    [TestMethod]
    public void TestOnTextChanged_FiresOnUserEdit()
    {
        // Arrange
        string? observed = null;
        var editor = CreateEditor("a").SetOnTextChanged(t => observed = t);

        // Act
        editor.HandleInput(PlusKey.End, shift: false, ctrl: false);
        editor.HandleInput('b');

        // Assert
        Assert.AreEqual("ab", observed);
    }

    #endregion

    #region Rendering

    private static void RenderToBitmap(CodeEditor editor)
    {
        using var bitmap = new SkiaSharp.SKBitmap(400, 200);
        using var canvas = new SkiaSharp.SKCanvas(bitmap);
        editor.Render(canvas);
    }

    [TestMethod]
    public void TestRender_WithHighlighterSelectionAndGutter_DoesNotThrow()
    {
        // Arrange
        var editor = CreateEditor("public void Foo()\n{\n    Bar();\n}")
            .SetHighlighter(text => [new StyleSpan(0, 6, Colors.Blue, FontWeight.Bold)]);
        editor.HandleInput(PlusKey.A, shift: false, ctrl: true);

        // Act & Assert
        RenderToBitmap(editor);
    }

    [TestMethod]
    public void TestRender_EmptyDocument_DoesNotThrow()
    {
        // Arrange
        var editor = CreateEditor();

        // Act & Assert
        RenderToBitmap(editor);
    }

    [TestMethod]
    public void TestRender_SpanReachingPastEndOfDocument_DoesNotThrow()
    {
        // Arrange: a highlighter returning a deliberately over-long span must not index out of range
        var editor = CreateEditor("abc")
            .SetHighlighter(_ => [new StyleSpan(0, 9999, Colors.Red)]);

        // Act & Assert
        RenderToBitmap(editor);
    }

    [TestMethod]
    public void TestRender_ScrolledDocument_DoesNotThrow()
    {
        // Arrange
        var editor = CreateEditor(string.Join('\n', Enumerable.Range(0, 200).Select(i => $"line {i}")));
        editor.HandleScroll(0, -500);

        // Act & Assert
        RenderToBitmap(editor);
    }

    #endregion

    #region Scrolling

    private static CodeEditor CreateScrollableEditor()
    {
        var editor = new CodeEditor()
            .SetText(string.Join('\n', Enumerable.Range(0, 200).Select(i => $"line {i}")));
        editor.Measure(new Size(400, 200));
        editor.Arrange(new Rect(new Point(0, 0), new Size(400, 200)));
        return editor;
    }

    [TestMethod]
    public void TestHandleScroll_PositiveDeltaScrollsDown_MatchingEntryAndScrollView()
    {
        // Arrange
        var editor = CreateScrollableEditor();

        // Act
        editor.HandleScroll(0, 40);

        // Assert: the framework convention is that deltas are added, not subtracted.
        // Subtracting inverts the mouse wheel relative to every other scrollable control.
        Assert.IsGreaterThan(0, editor.VerticalScrollOffset);
    }

    [TestMethod]
    public void TestHandleScroll_HorizontalUsesTheSameSignAsVertical()
    {
        // Arrange
        var editor = CreateScrollableEditor();

        // Act
        editor.HandleScroll(30, 0);

        // Assert
        Assert.IsGreaterThan(0, editor.HorizontalScrollOffset);
    }

    [TestMethod]
    public void TestHandleScroll_DoesNotScrollAboveTheTop()
    {
        // Arrange
        var editor = CreateScrollableEditor();

        // Act
        editor.HandleScroll(-100, -100);

        // Assert
        Assert.AreEqual(0f, editor.VerticalScrollOffset);
        Assert.AreEqual(0f, editor.HorizontalScrollOffset);
    }

    [TestMethod]
    public void TestHandleScroll_DoesNotScrollPastTheLastLine()
    {
        // Arrange
        var editor = CreateScrollableEditor();

        // Act
        editor.HandleScroll(0, 100_000);
        var atBottom = editor.VerticalScrollOffset;
        editor.HandleScroll(0, 100_000);

        // Assert
        Assert.AreEqual(atBottom, editor.VerticalScrollOffset);
    }

    #endregion

    #region Tab key routing

    [TestMethod]
    public void TestHandleKeyboardInput_ClaimsTabWhileFocused()
    {
        // Arrange
        var editor = CreateEditor();

        // Act
        var handled = editor.HandleKeyboardInput(PlusKey.Tab);

        // Assert
        Assert.IsTrue(handled, "CodeEditor must claim Tab so it indents instead of moving focus");
        Assert.AreEqual("    ", editor.Text);
    }

    [TestMethod]
    public void TestHandleKeyboardInput_DoesNotClaimTabWhenReadOnly()
    {
        // Arrange
        var editor = CreateEditor().SetIsReadOnly(true);

        // Act
        var handled = editor.HandleKeyboardInput(PlusKey.Tab);

        // Assert
        Assert.IsFalse(handled, "A read-only editor must let Tab move focus away");
    }

    [TestMethod]
    public void TestHandleKeyboardInput_DoesNotClaimUnrelatedKeys()
    {
        // Arrange
        var editor = CreateEditor();

        // Act & Assert
        Assert.IsFalse(editor.HandleKeyboardInput(PlusKey.ArrowDown));
    }

    #endregion
}
