using System.Text;
using System.Text.RegularExpressions;
using global::RegexBuilder.App.Models;
using BuilderLib = RegexBuilder.RegexBuilder;

namespace RegexBuilder.App.Services;

public static class RegexWorkbench
{
    public static RegexPreview BuildPreview(IReadOnlyList<RegexOperation> operations, RegexOptions options, string? testInput)
    {
        var codeSnippet = BuildCodeSnippet(operations, options);

        try
        {
            var builder = BuildBuilder(operations);
            var pattern = builder.Build();
            var regex = builder.ToRegex(options);
            var input = testInput ?? string.Empty;
            var matches = regex.Matches(input)
                .Cast<Match>()
                .Select(match => MapMatch(regex, match))
                .ToArray();

            return new RegexPreview(pattern, codeSnippet, matches, null);
        }
        catch (Exception ex)
        {
            return new RegexPreview(string.Empty, codeSnippet, Array.Empty<RegexMatchPreview>(), ex.Message);
        }
    }

    private static BuilderLib BuildBuilder(IEnumerable<RegexOperation> operations)
    {
        var builder = BuilderLib.Create();

        foreach (var operation in operations)
        {
            ApplyOperation(builder, operation);
        }

        return builder;
    }

    private static void ApplyOperation(BuilderLib builder, RegexOperation operation)
    {
        switch (operation.Kind)
        {
            case RegexOperationKind.StartOfLine:
                builder.StartOfLine();
                break;
            case RegexOperationKind.EndOfLine:
                builder.EndOfLine();
                break;
            case RegexOperationKind.Literal:
                builder.Literal(operation.PrimaryValue);
                break;
            case RegexOperationKind.Raw:
                builder.Raw(operation.PrimaryValue);
                break;
            case RegexOperationKind.AnyCharacter:
                builder.AnyCharacter();
                break;
            case RegexOperationKind.Digit:
                builder.Digit();
                break;
            case RegexOperationKind.NonDigit:
                builder.NonDigit();
                break;
            case RegexOperationKind.WordCharacter:
                builder.WordCharacter();
                break;
            case RegexOperationKind.NonWordCharacter:
                builder.NonWordCharacter();
                break;
            case RegexOperationKind.Whitespace:
                builder.Whitespace();
                break;
            case RegexOperationKind.NonWhitespace:
                builder.NonWhitespace();
                break;
            case RegexOperationKind.CharacterClass:
                builder.CharacterClass(operation.PrimaryValue);
                break;
            case RegexOperationKind.NegatedCharacterClass:
                builder.NegatedCharacterClass(operation.PrimaryValue);
                break;
            case RegexOperationKind.Range:
                builder.Range(operation.PrimaryValue[0], operation.SecondaryValue[0]);
                break;
            case RegexOperationKind.GroupRaw:
                builder.Group(group => group.Raw(operation.PrimaryValue));
                break;
            case RegexOperationKind.NamedGroupRaw:
                builder.NamedGroup(operation.PrimaryValue, group => group.Raw(operation.SecondaryValue));
                break;
            case RegexOperationKind.EitherRaw:
                builder.Either(left => left.Raw(operation.PrimaryValue), right => right.Raw(operation.SecondaryValue));
                break;
            case RegexOperationKind.Optional:
                builder.Optional();
                break;
            case RegexOperationKind.ZeroOrMore:
                builder.ZeroOrMore();
                break;
            case RegexOperationKind.OneOrMore:
                builder.OneOrMore();
                break;
            case RegexOperationKind.Exactly:
                builder.Exactly(int.Parse(operation.PrimaryValue));
                break;
            case RegexOperationKind.AtLeast:
                builder.AtLeast(int.Parse(operation.PrimaryValue));
                break;
            case RegexOperationKind.Between:
                builder.Between(int.Parse(operation.PrimaryValue), int.Parse(operation.SecondaryValue));
                break;
            default:
                throw new InvalidOperationException($"Unsupported operation kind: {operation.Kind}");
        }
    }

    private static RegexMatchPreview MapMatch(Regex regex, Match match)
    {
        var groupNumbers = regex.GetGroupNumbers();
        var groupNames = regex.GetGroupNames();
        var groups = new List<RegexGroupPreview>(groupNumbers.Length);

        for (var index = 0; index < groupNumbers.Length; index++)
        {
            var groupNumber = groupNumbers[index];
            var group = match.Groups[groupNumber];
            var groupName = groupNames[index];
            var displayName = groupName == groupNumber.ToString() ? groupNumber.ToString() : $"{groupNumber} ({groupName})";
            groups.Add(new RegexGroupPreview(displayName, group.Success, group.Value));
        }

        return new RegexMatchPreview(match.Value, match.Index, match.Length, groups);
    }

    private static string BuildCodeSnippet(IReadOnlyList<RegexOperation> operations, RegexOptions options)
    {
        var builder = new StringBuilder();
        builder.AppendLine("var regex = RegexBuilder.RegexBuilder.Create()");

        if (operations.Count == 0)
        {
            builder.Append("    .ToRegex(").Append(FormatOptions(options)).AppendLine(");");
            return builder.ToString();
        }

        foreach (var operation in operations)
        {
            builder.Append("    ").AppendLine(ToCode(operation));
        }

        builder.Append("    .ToRegex(").Append(FormatOptions(options)).AppendLine(");");
        return builder.ToString();
    }

    private static string ToCode(RegexOperation operation)
    {
        return operation.Kind switch
        {
            RegexOperationKind.StartOfLine => ".StartOfLine()",
            RegexOperationKind.EndOfLine => ".EndOfLine()",
            RegexOperationKind.Literal => $".Literal(\"{EscapeForCSharp(operation.PrimaryValue)}\")",
            RegexOperationKind.Raw => $".Raw(\"{EscapeForCSharp(operation.PrimaryValue)}\")",
            RegexOperationKind.AnyCharacter => ".AnyCharacter()",
            RegexOperationKind.Digit => ".Digit()",
            RegexOperationKind.NonDigit => ".NonDigit()",
            RegexOperationKind.WordCharacter => ".WordCharacter()",
            RegexOperationKind.NonWordCharacter => ".NonWordCharacter()",
            RegexOperationKind.Whitespace => ".Whitespace()",
            RegexOperationKind.NonWhitespace => ".NonWhitespace()",
            RegexOperationKind.CharacterClass => $".CharacterClass(\"{EscapeForCSharp(operation.PrimaryValue)}\")",
            RegexOperationKind.NegatedCharacterClass => $".NegatedCharacterClass(\"{EscapeForCSharp(operation.PrimaryValue)}\")",
            RegexOperationKind.Range => $".Range('{EscapeChar(operation.PrimaryValue[0])}', '{EscapeChar(operation.SecondaryValue[0])}')",
            RegexOperationKind.GroupRaw => $".Group(group => group.Raw(\"{EscapeForCSharp(operation.PrimaryValue)}\"))",
            RegexOperationKind.NamedGroupRaw => $".NamedGroup(\"{EscapeForCSharp(operation.PrimaryValue)}\", group => group.Raw(\"{EscapeForCSharp(operation.SecondaryValue)}\"))",
            RegexOperationKind.EitherRaw => $".Either(left => left.Raw(\"{EscapeForCSharp(operation.PrimaryValue)}\"), right => right.Raw(\"{EscapeForCSharp(operation.SecondaryValue)}\"))",
            RegexOperationKind.Optional => ".Optional()",
            RegexOperationKind.ZeroOrMore => ".ZeroOrMore()",
            RegexOperationKind.OneOrMore => ".OneOrMore()",
            RegexOperationKind.Exactly => $".Exactly({operation.PrimaryValue})",
            RegexOperationKind.AtLeast => $".AtLeast({operation.PrimaryValue})",
            RegexOperationKind.Between => $".Between({operation.PrimaryValue}, {operation.SecondaryValue})",
            _ => throw new InvalidOperationException($"Unsupported operation kind: {operation.Kind}")
        };
    }

    private static string FormatOptions(RegexOptions options)
    {
        if (options == RegexOptions.None)
        {
            return "RegexOptions.None";
        }

        var values = Enum.GetValues<RegexOptions>()
            .Where(value => value != RegexOptions.None && options.HasFlag(value))
            .Select(value => $"RegexOptions.{value}");

        return string.Join(" | ", values);
    }

    private static string EscapeForCSharp(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("\"", "\\\"", StringComparison.Ordinal);

    private static string EscapeChar(char value) => value switch
    {
        '\\' => "\\\\",
        '\'' => "\\'",
        _ => value.ToString()
    };
}

public sealed record RegexPreview(string Pattern, string CodeSnippet, IReadOnlyList<RegexMatchPreview> Matches, string? ErrorMessage);

public sealed record RegexMatchPreview(string Value, int Index, int Length, IReadOnlyList<RegexGroupPreview> Groups);

public sealed record RegexGroupPreview(string DisplayName, bool Success, string Value);
