using System.Text.RegularExpressions;
using TUnit.FsCheck;
using Builder = RegexBuilder.RegexBuilder;

namespace RegexBuilder.Tests;

public class RegexBuilderScenarios
{
    [Test]
    public async Task Build_returns_an_anchored_literal_pattern()
    {
        var pattern = Builder.Create()
            .StartOfLine()
            .Literal("cat")
            .EndOfLine()
            .Build();

        await Assert.That(pattern).IsEqualTo("^cat$");
    }

    [Test]
    public async Task Build_returns_a_raw_pattern_segment()
    {
        var pattern = Builder.Create()
            .StartOfLine()
            .Raw("[A-Z]{3}")
            .EndOfLine()
            .Build();

        await Assert.That(pattern).IsEqualTo("^[A-Z]{3}$");
    }

    [Test]
    public async Task Build_returns_an_any_character_pattern()
    {
        var pattern = Builder.Create()
            .AnyCharacter()
            .Build();

        await Assert.That(pattern).IsEqualTo(".");
    }

    [Test]
    public async Task Build_returns_a_quantified_digit_pattern()
    {
        var pattern = Builder.Create()
            .Digit()
            .OneOrMore()
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\d+");
    }

    [Test]
    public async Task Build_returns_a_non_digit_pattern()
    {
        var pattern = Builder.Create()
            .NonDigit()
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\D");
    }

    [Test]
    public async Task Build_returns_a_word_character_pattern()
    {
        var pattern = Builder.Create()
            .WordCharacter()
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\w");
    }

    [Test]
    public async Task Build_returns_a_non_word_character_pattern()
    {
        var pattern = Builder.Create()
            .NonWordCharacter()
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\W");
    }

    [Test]
    public async Task Build_returns_a_whitespace_pattern()
    {
        var pattern = Builder.Create()
            .Whitespace()
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\s");
    }

    [Test]
    public async Task Build_returns_a_non_whitespace_pattern()
    {
        var pattern = Builder.Create()
            .NonWhitespace()
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\S");
    }

    [Test]
    public async Task Build_returns_a_character_class_pattern()
    {
        var pattern = Builder.Create()
            .CharacterClass("abc")
            .Build();

        await Assert.That(pattern).IsEqualTo("[abc]");
    }

    [Test]
    public async Task Build_returns_a_negated_character_class_pattern()
    {
        var pattern = Builder.Create()
            .NegatedCharacterClass("abc")
            .Build();

        await Assert.That(pattern).IsEqualTo("[^abc]");
    }

    [Test]
    public async Task Build_returns_a_character_range_pattern()
    {
        var pattern = Builder.Create()
            .Range('a', 'z')
            .Build();

        await Assert.That(pattern).IsEqualTo("[a-z]");
    }

    [Test]
    public async Task Build_returns_a_group_pattern()
    {
        var pattern = Builder.Create()
            .Group(group => group.Literal("cat").Digit())
            .Build();

        await Assert.That(pattern).IsEqualTo(@"(cat\d)");
    }

    [Test]
    public async Task Build_returns_named_groups_for_a_phone_number_pattern()
    {
        var pattern = Builder.Create()
            .StartOfLine()
            .NamedGroup("area", group => group.Digit().Exactly(3))
            .Literal("-")
            .NamedGroup("number", group => group.Digit().Exactly(7))
            .EndOfLine()
            .Build();

        await Assert.That(pattern).IsEqualTo(@"^(?<area>\d{3})-(?<number>\d{7})$");
    }

    [Test]
    public async Task Build_returns_an_either_pattern()
    {
        var pattern = Builder.Create()
            .Either(left => left.Literal("cat"), right => right.Literal("dog"))
            .Build();

        await Assert.That(pattern).IsEqualTo("(cat|dog)");
    }

    [Test]
    public async Task Build_returns_an_optional_pattern()
    {
        var pattern = Builder.Create()
            .Literal("colou")
            .Literal("r")
            .Optional()
            .Build();

        await Assert.That(pattern).IsEqualTo("colour?");
    }

    [Test]
    public async Task Build_returns_a_zero_or_more_pattern()
    {
        var pattern = Builder.Create()
            .Literal("ha")
            .ZeroOrMore()
            .Build();

        await Assert.That(pattern).IsEqualTo("(ha)*");
    }

    [Test]
    public async Task Build_returns_a_one_or_more_pattern()
    {
        var pattern = Builder.Create()
            .Literal("ha")
            .OneOrMore()
            .Build();

        await Assert.That(pattern).IsEqualTo("(ha)+");
    }

    [Test]
    public async Task Build_returns_an_exact_quantifier_pattern()
    {
        var pattern = Builder.Create()
            .WordCharacter()
            .Exactly(3)
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\w{3}");
    }

    [Test]
    public async Task Build_returns_an_at_least_quantifier_pattern()
    {
        var pattern = Builder.Create()
            .Digit()
            .AtLeast(2)
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\d{2,}");
    }

    [Test]
    public async Task Build_returns_a_between_quantifier_pattern()
    {
        var pattern = Builder.Create()
            .Digit()
            .Between(2, 4)
            .Build();

        await Assert.That(pattern).IsEqualTo(@"\d{2,4}");
    }

    [Test]
    public async Task Build_combines_multiple_segments_into_a_single_pattern()
    {
        var pattern = Builder.Create()
            .StartOfLine()
            .CharacterClass("abc")
            .Range('0', '9')
            .Whitespace()
            .Literal("end")
            .EndOfLine()
            .Build();

        await Assert.That(pattern).IsEqualTo("^[abc][0-9]\\send$");
    }

    [Test]
    public async Task ToRegex_applies_regex_options_to_the_built_pattern()
    {
        var regex = Builder.Create()
            .Literal("abc")
            .ToRegex(RegexOptions.IgnoreCase);

        await Assert.That(regex.IsMatch("ABC")).IsTrue();
    }

    [Test]
    public async Task ToRegex_builds_a_regex_that_matches_the_generated_pattern()
    {
        var regex = Builder.Create()
            .StartOfLine()
            .Literal("cat")
            .Digit()
            .EndOfLine()
            .ToRegex();

        await Assert.That(regex.IsMatch("cat7")).IsTrue();
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task Literal_escapes_any_generated_text(string value)
    {
        var pattern = Builder.Create()
            .Literal(value)
            .Build();

        await Assert.That(pattern).IsEqualTo(Regex.Escape(value));
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task Literal_round_trips_through_an_anchored_regex(string value)
    {
        var regex = Builder.Create()
            .StartOfLine()
            .Literal(value)
            .EndOfLine()
            .ToRegex();

        await Assert.That(regex.IsMatch(value)).IsTrue();
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task ToRegex_string_representation_matches_the_built_pattern(string value)
    {
        var builder = Builder.Create()
            .Literal(value);

        await Assert.That(builder.ToRegex().ToString()).IsEqualTo(builder.Build());
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task Raw_uses_the_provided_pattern_for_matching(RawPatternSample sample)
    {
        var regex = Builder.Create()
            .StartOfLine()
            .Raw(sample.Pattern)
            .EndOfLine()
            .ToRegex();

        await Assert.That(regex.IsMatch(sample.MatchingText)).IsTrue();
        await Assert.That(regex.IsMatch(sample.NonMatchingText)).IsFalse();
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task CharacterClass_matches_every_generated_member(CharacterClassSample sample)
    {
        var regex = Builder.Create()
            .StartOfLine()
            .CharacterClass(sample.PatternCharacters)
            .EndOfLine()
            .ToRegex();

        foreach (var member in sample.Members)
        {
            await Assert.That(regex.IsMatch(member.ToString())).IsTrue();
        }
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task Either_matches_values_from_both_generated_branches(LiteralPair sample)
    {
        var regex = Builder.Create()
            .StartOfLine()
            .Either(left => left.Literal(sample.Left), right => right.Literal(sample.Right))
            .EndOfLine()
            .ToRegex();

        await Assert.That(regex.IsMatch(sample.Left)).IsTrue();
        await Assert.That(regex.IsMatch(sample.Right)).IsTrue();
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task NamedGroup_captures_the_generated_literal_value(NamedGroupSample sample)
    {
        var regex = Builder.Create()
            .StartOfLine()
            .NamedGroup(sample.Name, group => group.Literal(sample.Value))
            .EndOfLine()
            .ToRegex();

        var match = regex.Match(sample.Value);

        await Assert.That(match.Success).IsTrue();
        await Assert.That(match.Groups[sample.Name].Value).IsEqualTo(sample.Value);
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task Exactly_matches_only_text_of_the_requested_length(ExactQuantifierSample sample)
    {
        var regex = Builder.Create()
            .StartOfLine()
            .Digit()
            .Exactly(sample.Count)
            .EndOfLine()
            .ToRegex();

        await Assert.That(regex.IsMatch(sample.MatchingText)).IsTrue();
        await Assert.That(regex.IsMatch(sample.NonMatchingText)).IsFalse();
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task Between_matches_only_text_within_the_requested_range(BetweenQuantifierSample sample)
    {
        var regex = Builder.Create()
            .StartOfLine()
            .Digit()
            .Between(sample.Minimum, sample.Maximum)
            .EndOfLine()
            .ToRegex();

        await Assert.That(regex.IsMatch(sample.MinimumText)).IsTrue();
        await Assert.That(regex.IsMatch(sample.MaximumText)).IsTrue();
        await Assert.That(regex.IsMatch(sample.TooLongText)).IsFalse();
    }
}
