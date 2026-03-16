using System;
using System.Collections.Generic;
using System.Linq;

namespace RegexBuilder
{
    public sealed class RegexReplacementBuilder
    {
        private readonly List<string> _segments = [];

        private RegexReplacementBuilder()
        {
        }

        public static RegexReplacementBuilder Create() => new();

        public RegexReplacementBuilder Literal(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            _segments.Add(value.Replace("$", "$$", StringComparison.Ordinal));
            return this;
        }

        public RegexReplacementBuilder EntireMatch()
        {
            _segments.Add("$0");
            return this;
        }

        public RegexReplacementBuilder Group(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            _segments.Add($"${index}");
            return this;
        }

        public RegexReplacementBuilder Group(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            _segments.Add($"${{{name}}}");
            return this;
        }

        public string Build() => string.Concat(_segments);
    }
}
