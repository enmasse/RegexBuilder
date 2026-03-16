# RegexBuilder

A fluent C# API for composing regular expressions from named, readable building blocks rather than raw pattern strings.

## Overview

RegexBuilder lets you construct `.NET` regular expressions by chaining descriptive method calls. The resulting pattern is identical to one you would write by hand, but the intent of each part is immediately clear from the code.

```csharp
var regex = RegexBuilder.Create()
    .StartOfLine()
    .Literal("cat")
    .Digit()
    .EndOfLine()
    .ToRegex();

regex.IsMatch("cat7"); // true
```

## Projects

| Project | Description |
|---|---|
| `RegexBuilder` | Core library (class library, .NET) |
| `RegexBuilder.App` | Interactive [Blazor WebAssembly](#regexbuilder-lab) workbench |
| `RegexBuilder.Tests` | Unit and property-based tests (TUnit + FsCheck) |

## Installation

Add a reference to the `RegexBuilder` project (or build and reference the assembly directly). The library targets .NET and has no external dependencies.

## API Reference

### Creating a builder

```csharp
var builder = RegexBuilder.Create();
```

### Anchors

| Method | Pattern |
|---|---|
| `StartOfLine()` | `^` |
| `EndOfLine()` | `$` |

### Characters

| Method | Pattern | Description |
|---|---|---|
| `Literal(string value)` | Escaped literal | Matches the exact string, escaping all special regex characters |
| `Raw(string pattern)` | Raw pattern | Appends a raw regex fragment without escaping |
| `AnyCharacter()` | `.` | Matches any single character (except newline by default) |
| `Digit()` | `\d` | Matches a decimal digit |
| `NonDigit()` | `\D` | Matches any character that is not a digit |
| `WordCharacter()` | `\w` | Matches a word character (`[a-zA-Z0-9_]`) |
| `NonWordCharacter()` | `\W` | Matches a non-word character |
| `Whitespace()` | `\s` | Matches a whitespace character |
| `NonWhitespace()` | `\S` | Matches a non-whitespace character |

### Character classes

| Method | Pattern | Description |
|---|---|---|
| `CharacterClass(string characters)` | `[characters]` | Matches any character in the set |
| `NegatedCharacterClass(string characters)` | `[^characters]` | Matches any character **not** in the set |
| `Range(char start, char end)` | `[start-end]` | Matches any character in the given range |

### Groups and alternation

| Method | Pattern | Description |
|---|---|---|
| `Group(Action<RegexBuilder> buildGroup)` | `(…)` | Capturing group built from nested builder calls |
| `NamedGroup(string name, Action<RegexBuilder> buildGroup)` | `(?<name>…)` | Named capturing group |
| `Either(Action<RegexBuilder> left, Action<RegexBuilder> right)` | `(left\|right)` | Matches either of two alternatives |

### Quantifiers

Quantifiers are applied to the **preceding** segment.

| Method | Pattern | Description |
|---|---|---|
| `Optional()` | `?` | Zero or one occurrence |
| `ZeroOrMore()` | `*` | Zero or more occurrences |
| `OneOrMore()` | `+` | One or more occurrences |
| `Exactly(int count)` | `{n}` | Exactly `n` occurrences |
| `AtLeast(int minimum)` | `{n,}` | At least `n` occurrences |
| `Between(int minimum, int maximum)` | `{n,m}` | Between `n` and `m` occurrences (inclusive) |

### Building the result

| Method | Return type | Description |
|---|---|---|
| `Build()` | `string` | Returns the composed regex pattern string |
| `ToRegex()` | `Regex` | Compiles the pattern into a `Regex` object with default options |
| `ToRegex(RegexOptions options)` | `Regex` | Compiles the pattern with the given `RegexOptions` |

### Replacement

| Method | Description |
|---|---|
| `Replace(string input, string replacement)` | Replaces all matches with a fixed string |
| `Replace(string input, MatchEvaluator evaluator)` | Replaces all matches using a delegate |
| `Replace(string input, Action<RegexReplacementBuilder> buildReplacement)` | Replaces all matches using a fluent replacement template |
| `ReplaceFirst(string input, string replacement)` | Replaces only the first match with a fixed string |
| `ReplaceFirst(string input, MatchEvaluator evaluator)` | Replaces only the first match using a delegate |
| `ReplaceFirst(string input, Action<RegexReplacementBuilder> buildReplacement)` | Replaces only the first match using a fluent replacement template |

#### RegexReplacementBuilder

Build replacement templates fluently when using the `Action<RegexReplacementBuilder>` overloads.

| Method | Description |
|---|---|
| `Literal(string value)` | Inserts a literal string into the replacement (dollar signs are escaped automatically) |
| `EntireMatch()` | Inserts the entire matched text (`$0`) |
| `Group(int index)` | Inserts the value of a numbered capture group |
| `Group(string name)` | Inserts the value of a named capture group |
| `Build()` | Returns the composed replacement string |

## Usage examples

### Match a simple pattern

```csharp
bool isMatch = RegexBuilder.Create()
    .StartOfLine()
    .Literal("hello")
    .Whitespace()
    .WordCharacter()
    .OneOrMore()
    .EndOfLine()
    .ToRegex()
    .IsMatch("hello world"); // true
```

### Named capture groups

```csharp
var regex = RegexBuilder.Create()
    .StartOfLine()
    .NamedGroup("area", b => b.Digit().Exactly(3))
    .Literal("-")
    .NamedGroup("number", b => b.Digit().Exactly(7))
    .EndOfLine()
    .ToRegex();

var match = regex.Match("123-4567890");
Console.WriteLine(match.Groups["area"].Value);   // 123
Console.WriteLine(match.Groups["number"].Value); // 4567890
```

### Case-insensitive matching

```csharp
var regex = RegexBuilder.Create()
    .Literal("cat")
    .ToRegex(RegexOptions.IgnoreCase);

regex.IsMatch("CAT"); // true
```

### Replacement with a fluent template

```csharp
string result = RegexBuilder.Create()
    .NamedGroup("first", b => b.WordCharacter().OneOrMore())
    .Whitespace()
    .NamedGroup("last", b => b.WordCharacter().OneOrMore())
    .Replace("John Smith", r => r.Group("last").Literal(", ").Group("first"));

Console.WriteLine(result); // Smith, John
```

## RegexBuilder Lab

The `RegexBuilder.App` project is an interactive Blazor WebAssembly workbench that lets you:

- Compose builder steps visually using a dropdown selector and value inputs.
- Preview the generated regex pattern in real time.
- See the equivalent `RegexBuilder` C# code snippet.
- Toggle `RegexOptions` flags (IgnoreCase, Multiline, Singleline, IgnorePatternWhitespace).
- Test the pattern against arbitrary input and inspect all match details including capture groups.

The app is published to **GitHub Pages** on every push to `master` via the included workflow.

## Running the tests

```bash
dotnet test ./RegexBuilder.Tests/RegexBuilder.Tests.csproj
```

The test suite uses [TUnit](https://tunit.dev) for example-based tests and [FsCheck](https://fscheck.github.io/FsCheck/) for property-based tests.
