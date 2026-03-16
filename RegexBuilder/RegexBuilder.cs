using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegexBuilder
{
    public sealed class RegexBuilder
    {
        private readonly List<PatternSegment> _segments = [];

        private RegexBuilder()
        {
        }

        public static RegexBuilder Create() => new();

        public RegexBuilder StartOfLine() => AppendSegment("^", isAtomic: true, canQuantify: false);

        public RegexBuilder EndOfLine() => AppendSegment("$", isAtomic: true, canQuantify: false);

        public RegexBuilder Literal(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return AppendSegment(Regex.Escape(value), isAtomic: value.Length <= 1);
        }

        public RegexBuilder Raw(string pattern)
        {
            ArgumentNullException.ThrowIfNull(pattern);
            return AppendSegment(pattern, isAtomic: false);
        }

        public RegexBuilder AnyCharacter() => AppendSegment(".", isAtomic: true);

        public RegexBuilder Digit() => AppendSegment(@"\d", isAtomic: true);

        public RegexBuilder NonDigit() => AppendSegment(@"\D", isAtomic: true);

        public RegexBuilder WordCharacter() => AppendSegment(@"\w", isAtomic: true);

        public RegexBuilder NonWordCharacter() => AppendSegment(@"\W", isAtomic: true);

        public RegexBuilder Whitespace() => AppendSegment(@"\s", isAtomic: true);

        public RegexBuilder NonWhitespace() => AppendSegment(@"\S", isAtomic: true);

        public RegexBuilder CharacterClass(string characters)
        {
            ArgumentNullException.ThrowIfNull(characters);
            return AppendSegment($"[{characters}]", isAtomic: true);
        }

        public RegexBuilder NegatedCharacterClass(string characters)
        {
            ArgumentNullException.ThrowIfNull(characters);
            return AppendSegment($"[^{characters}]", isAtomic: true);
        }

        public RegexBuilder Range(char start, char end) => AppendSegment($"[{start}-{end}]", isAtomic: true);

        public RegexBuilder Group(Action<RegexBuilder> buildGroup)
        {
            ArgumentNullException.ThrowIfNull(buildGroup);
            return AppendSegment($"({BuildNestedPattern(buildGroup)})", isAtomic: true);
        }

        public RegexBuilder NamedGroup(string name, Action<RegexBuilder> buildGroup)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(buildGroup);
            return AppendSegment($"(?<{name}>{BuildNestedPattern(buildGroup)})", isAtomic: true);
        }

        public RegexBuilder Either(Action<RegexBuilder> left, Action<RegexBuilder> right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            var leftPattern = BuildNestedPattern(left);
            var rightPattern = BuildNestedPattern(right);
            return AppendSegment($"({leftPattern}|{rightPattern})", isAtomic: true);
        }

        public RegexBuilder Optional() => ApplyQuantifier("?");

        public RegexBuilder ZeroOrMore() => ApplyQuantifier("*");

        public RegexBuilder OneOrMore() => ApplyQuantifier("+");

        public RegexBuilder Exactly(int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            return ApplyQuantifier($"{{{count}}}");
        }

        public RegexBuilder AtLeast(int minimum)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(minimum);
            return ApplyQuantifier($"{{{minimum},}}");
        }

        public RegexBuilder Between(int minimum, int maximum)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(minimum);
            ArgumentOutOfRangeException.ThrowIfNegative(maximum);

            if (maximum < minimum)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), "Maximum must be greater than or equal to minimum.");
            }

            return ApplyQuantifier($"{{{minimum},{maximum}}}");
        }

        public string Build() => string.Concat(_segments.Select(segment => segment.Pattern));

        public Regex ToRegex() => ToRegex(RegexOptions.None);

        public Regex ToRegex(RegexOptions options) => new(Build(), options);

        public string Replace(string input, string replacement)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(replacement);
            return ToRegex().Replace(input, replacement);
        }

        public string Replace(string input, MatchEvaluator evaluator)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(evaluator);
            return ToRegex().Replace(input, evaluator);
        }

        public string Replace(string input, Action<RegexReplacementBuilder> buildReplacement)
        {
            ArgumentNullException.ThrowIfNull(buildReplacement);
            return Replace(input, BuildReplacement(buildReplacement));
        }

        public string ReplaceFirst(string input, string replacement)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(replacement);
            return ToRegex().Replace(input, replacement, 1);
        }

        public string ReplaceFirst(string input, MatchEvaluator evaluator)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(evaluator);
            return ToRegex().Replace(input, evaluator, 1);
        }

        public string ReplaceFirst(string input, Action<RegexReplacementBuilder> buildReplacement)
        {
            ArgumentNullException.ThrowIfNull(buildReplacement);
            return ReplaceFirst(input, BuildReplacement(buildReplacement));
        }

        private RegexBuilder AppendSegment(string pattern, bool isAtomic, bool canQuantify = true)
        {
            _segments.Add(new PatternSegment(pattern, isAtomic, canQuantify));
            return this;
        }

        private RegexBuilder ApplyQuantifier(string quantifier)
        {
            if (_segments.Count == 0)
            {
                throw new InvalidOperationException("A quantifier requires a preceding pattern segment.");
            }

            var lastIndex = _segments.Count - 1;
            var segment = _segments[lastIndex];

            if (!segment.CanQuantify)
            {
                throw new InvalidOperationException("The last pattern segment cannot be quantified.");
            }

            var quantifiedPattern = segment.IsAtomic
                ? segment.Pattern + quantifier
                : $"({segment.Pattern}){quantifier}";

            _segments[lastIndex] = new PatternSegment(quantifiedPattern, IsAtomic: true, CanQuantify: true);
            return this;
        }

        private static string BuildNestedPattern(Action<RegexBuilder> buildAction)
        {
            var builder = Create();
            buildAction(builder);
            return builder.Build();
        }

        private static string BuildReplacement(Action<RegexReplacementBuilder> buildReplacement)
        {
            var builder = RegexReplacementBuilder.Create();
            buildReplacement(builder);
            return builder.Build();
        }

        private readonly record struct PatternSegment(string Pattern, bool IsAtomic, bool CanQuantify);
    }
}
