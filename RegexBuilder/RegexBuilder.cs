using System;
using System.Text.RegularExpressions;

namespace RegexBuilder
{
    public sealed class RegexBuilder
    {
        public static RegexBuilder Create() => throw new NotImplementedException();

        public RegexBuilder StartOfLine() => throw new NotImplementedException();

        public RegexBuilder EndOfLine() => throw new NotImplementedException();

        public RegexBuilder Literal(string value) => throw new NotImplementedException();

        public RegexBuilder Raw(string pattern) => throw new NotImplementedException();

        public RegexBuilder AnyCharacter() => throw new NotImplementedException();

        public RegexBuilder Digit() => throw new NotImplementedException();

        public RegexBuilder NonDigit() => throw new NotImplementedException();

        public RegexBuilder WordCharacter() => throw new NotImplementedException();

        public RegexBuilder NonWordCharacter() => throw new NotImplementedException();

        public RegexBuilder Whitespace() => throw new NotImplementedException();

        public RegexBuilder NonWhitespace() => throw new NotImplementedException();

        public RegexBuilder CharacterClass(string characters) => throw new NotImplementedException();

        public RegexBuilder NegatedCharacterClass(string characters) => throw new NotImplementedException();

        public RegexBuilder Range(char start, char end) => throw new NotImplementedException();

        public RegexBuilder Group(Action<RegexBuilder> buildGroup) => throw new NotImplementedException();

        public RegexBuilder NamedGroup(string name, Action<RegexBuilder> buildGroup) => throw new NotImplementedException();

        public RegexBuilder Either(Action<RegexBuilder> left, Action<RegexBuilder> right) => throw new NotImplementedException();

        public RegexBuilder Optional() => throw new NotImplementedException();

        public RegexBuilder ZeroOrMore() => throw new NotImplementedException();

        public RegexBuilder OneOrMore() => throw new NotImplementedException();

        public RegexBuilder Exactly(int count) => throw new NotImplementedException();

        public RegexBuilder AtLeast(int minimum) => throw new NotImplementedException();

        public RegexBuilder Between(int minimum, int maximum) => throw new NotImplementedException();

        public string Build() => throw new NotImplementedException();

        public Regex ToRegex() => throw new NotImplementedException();

        public Regex ToRegex(RegexOptions options) => throw new NotImplementedException();

        public string Replace(string input, string replacement) => throw new NotImplementedException();

        public string Replace(string input, MatchEvaluator evaluator) => throw new NotImplementedException();

        public string Replace(string input, Action<RegexReplacementBuilder> buildReplacement) => throw new NotImplementedException();

        public string ReplaceFirst(string input, string replacement) => throw new NotImplementedException();

        public string ReplaceFirst(string input, MatchEvaluator evaluator) => throw new NotImplementedException();

        public string ReplaceFirst(string input, Action<RegexReplacementBuilder> buildReplacement) => throw new NotImplementedException();
    }
}
