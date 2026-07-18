using PlusUi.core;

namespace PlusUi.Demo.Pages.Controls;

/// <summary>
/// A line-based C# tokenizer for <see cref="CodeEditor"/>.
/// </summary>
/// <remarks>
/// Each line is tokenized on its own and hands the next line a state, which is what makes constructs
/// that span lines work: a block comment opened on one line keeps colouring the lines below it until
/// it closes. Because the editor caches per line, editing one line of a large file re-tokenizes that
/// line - and only continues downward while the carried state keeps changing.
/// </remarks>
public static class CSharpHighlighter
{
    private const int Normal = 0;
    private const int InBlockComment = 1;
    private const int InVerbatimString = 2;

    private static readonly Color Keyword = new(86, 156, 214);
    private static readonly Color ControlKeyword = new(197, 134, 192);
    private static readonly Color TypeName = new(78, 201, 176);
    private static readonly Color MethodName = new(220, 220, 170);
    private static readonly Color StringLiteral = new(206, 145, 120);
    private static readonly Color Comment = new(106, 153, 85);
    private static readonly Color Number = new(181, 206, 168);
    private static readonly Color Preprocessor = new(155, 155, 155);

    private static readonly HashSet<string> Keywords =
    [
        "abstract", "as", "base", "bool", "byte", "char", "checked", "class", "const", "decimal",
        "delegate", "double", "enum", "event", "explicit", "extern", "false", "fixed", "float",
        "get", "implicit", "in", "init", "int", "interface", "internal", "is", "lock", "long",
        "namespace", "new", "null", "object", "operator", "out", "override", "params", "partial",
        "private", "protected", "public", "readonly", "record", "ref", "required", "sealed", "set",
        "short", "sizeof", "stackalloc", "static", "string", "struct", "this", "true", "typeof",
        "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "var", "virtual", "void",
        "volatile", "where"
    ];

    private static readonly HashSet<string> ControlKeywords =
    [
        "await", "break", "case", "catch", "continue", "default", "do", "else", "finally", "for",
        "foreach", "goto", "if", "return", "switch", "throw", "try", "while", "yield", "async"
    ];

    /// <summary>Tokenizes one line and returns the state the next line starts in.</summary>
    public static int HighlightLine(string line, int lineIndex, int state, List<StyleSpan> output)
    {
        var i = 0;

        while (i < line.Length)
        {
            if (state == InBlockComment)
            {
                state = ScanBlockComment(line, ref i, output);
                continue;
            }

            if (state == InVerbatimString)
            {
                state = ScanVerbatimString(line, ref i, output);
                continue;
            }

            var c = line[i];

            // Line comment - runs to the end of the line
            if (c == '/' && i + 1 < line.Length && line[i + 1] == '/')
            {
                output.Add(new StyleSpan(i, line.Length - i, Comment, FontStyle: FontStyle.Italic));
                return Normal;
            }

            // Block comment opens - may continue past this line
            if (c == '/' && i + 1 < line.Length && line[i + 1] == '*')
            {
                var start = i;
                i += 2;
                state = ScanBlockComment(line, ref i, output, start);
                continue;
            }

            // Preprocessor directive
            if (c == '#' && IsOnlyWhitespaceBefore(line, i))
            {
                output.Add(new StyleSpan(i, line.Length - i, Preprocessor));
                return Normal;
            }

            // Verbatim string opens - may continue past this line
            if (c == '@' && i + 1 < line.Length && line[i + 1] == '"')
            {
                var start = i;
                i += 2;
                state = ScanVerbatimString(line, ref i, output, start);
                continue;
            }

            // Regular string or char literal - always ends with the line
            if (c is '"' or '\'')
            {
                var quote = c;
                var start = i;
                i++;
                while (i < line.Length && line[i] != quote)
                {
                    i += line[i] == '\\' && i + 1 < line.Length ? 2 : 1;
                }
                if (i < line.Length) i++;
                output.Add(new StyleSpan(start, Math.Min(i, line.Length) - start, StringLiteral));
                continue;
            }

            // Number literal
            if (char.IsDigit(c))
            {
                var start = i;
                while (i < line.Length && (char.IsLetterOrDigit(line[i]) || line[i] == '.' || line[i] == '_')) i++;
                output.Add(new StyleSpan(start, i - start, Number));
                continue;
            }

            // Identifier or keyword
            if (char.IsLetter(c) || c == '_')
            {
                var start = i;
                while (i < line.Length && (char.IsLetterOrDigit(line[i]) || line[i] == '_')) i++;
                var word = line[start..i];

                if (ControlKeywords.Contains(word))
                {
                    output.Add(new StyleSpan(start, i - start, ControlKeyword));
                }
                else if (Keywords.Contains(word))
                {
                    output.Add(new StyleSpan(start, i - start, Keyword));
                }
                else if (IsFollowedByCallParenthesis(line, i))
                {
                    output.Add(new StyleSpan(start, i - start, MethodName));
                }
                else if (char.IsUpper(word[0]))
                {
                    output.Add(new StyleSpan(start, i - start, TypeName));
                }
                continue;
            }

            i++;
        }

        return state;
    }

    /// <summary>
    /// Consumes block comment text up to "*&#47;" or the end of the line.
    /// Returns Normal when the comment closed here, InBlockComment when it continues below.
    /// </summary>
    private static int ScanBlockComment(string line, ref int i, List<StyleSpan> output, int? spanStart = null)
    {
        var start = spanStart ?? i;

        while (i < line.Length)
        {
            if (line[i] == '*' && i + 1 < line.Length && line[i + 1] == '/')
            {
                i += 2;
                output.Add(new StyleSpan(start, i - start, Comment, FontStyle: FontStyle.Italic));
                return Normal;
            }
            i++;
        }

        output.Add(new StyleSpan(start, line.Length - start, Comment, FontStyle: FontStyle.Italic));
        return InBlockComment;
    }

    /// <summary>
    /// Consumes verbatim string text up to the closing quote or the end of the line, treating "" as escaped.
    /// </summary>
    private static int ScanVerbatimString(string line, ref int i, List<StyleSpan> output, int? spanStart = null)
    {
        var start = spanStart ?? i;

        while (i < line.Length)
        {
            if (line[i] == '"')
            {
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    i += 2;
                    continue;
                }
                i++;
                output.Add(new StyleSpan(start, i - start, StringLiteral));
                return Normal;
            }
            i++;
        }

        output.Add(new StyleSpan(start, line.Length - start, StringLiteral));
        return InVerbatimString;
    }

    private static bool IsOnlyWhitespaceBefore(string line, int index)
    {
        for (var i = 0; i < index; i++)
        {
            if (line[i] != ' ' && line[i] != '\t') return false;
        }
        return true;
    }

    private static bool IsFollowedByCallParenthesis(string line, int index)
    {
        while (index < line.Length && line[index] == ' ') index++;
        return index < line.Length && line[index] == '(';
    }
}
