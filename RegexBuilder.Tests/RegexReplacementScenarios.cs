using System.Text.RegularExpressions;
using TUnit.FsCheck;
using Builder = RegexBuilder.RegexBuilder;
using ReplacementBuilder = RegexBuilder.RegexReplacementBuilder;

namespace RegexBuilder.Tests;

public class RegexReplacementScenarios
{
    [Test]
    public async Task Replace_replaces_all_matches_with_a_literal_value()
    {
        var result = Builder.Create()
            .Digit()
            .OneOrMore()
            .Replace("A12B345C6", "#");

        await Assert.That(result).IsEqualTo("A#B#C#");
    }

    [Test]
    public async Task ReplaceFirst_replaces_only_the_first_match()
    {
        var result = Builder.Create()
            .Digit()
            .OneOrMore()
            .ReplaceFirst("A12B345C6", "#");

        await Assert.That(result).IsEqualTo("A#B345C6");
    }

    [Test]
    public async Task Replace_supports_match_evaluator_callbacks()
    {
        var result = Builder.Create()
            .Digit()
            .OneOrMore()
            .Replace("1 22 333", match => $"[{match.Value.Length}]");

        await Assert.That(result).IsEqualTo("[1] [2] [3]");
    }

    [Test]
    public async Task Replace_supports_fluent_replacement_templates_built_from_named_groups()
    {
        var result = Builder.Create()
            .StartOfLine()
            .NamedGroup("last", group => group.WordCharacter().OneOrMore())
            .Literal(", ")
            .NamedGroup("first", group => group.WordCharacter().OneOrMore())
            .EndOfLine()
            .Replace("Lovelace, Ada", replacement => replacement.Group("first").Literal(" ").Group("last"));

        await Assert.That(result).IsEqualTo("Ada Lovelace");
    }

    [Test]
    public async Task ReplaceFirst_supports_fluent_replacement_templates_built_from_captured_groups()
    {
        var result = Builder.Create()
            .Group(group => group.WordCharacter().OneOrMore())
            .Literal("-")
            .Group(group => group.WordCharacter().OneOrMore())
            .ReplaceFirst("left-right next-item", replacement => replacement.Group(2).Literal(":").Group(1));

        await Assert.That(result).IsEqualTo("right:left next-item");
    }

    [Test]
    public async Task ReplacementBuilder_builds_a_named_group_reference_template()
    {
        var replacement = ReplacementBuilder.Create()
            .Group("last")
            .Literal(", ")
            .Group("first")
            .Build();

        await Assert.That(replacement).IsEqualTo("${last}, ${first}");
    }

    [Test]
    public async Task ReplacementBuilder_builds_an_entire_match_reference_template()
    {
        var replacement = ReplacementBuilder.Create()
            .Literal("[")
            .EntireMatch()
            .Literal("]")
            .Build();

        await Assert.That(replacement).IsEqualTo("[$0]");
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task Replace_replaces_an_anchored_literal_match_with_the_requested_text(string value)
    {
        var result = Builder.Create()
            .StartOfLine()
            .Literal(value)
            .EndOfLine()
            .Replace(value, "#");

        await Assert.That(result).IsEqualTo("#");
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task ReplaceFirst_leaves_non_matching_text_unchanged(string value)
    {
        var input = value + "!";

        var result = Builder.Create()
            .StartOfLine()
            .Literal(value)
            .EndOfLine()
            .ReplaceFirst(input, "#");

        await Assert.That(result).IsEqualTo(input);
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task ReplaceFirst_replaces_only_the_first_generated_literal_occurrence(NonEmptyLiteralSample sample)
    {
        var input = sample.Value + "|" + sample.Value + "|" + sample.Value;

        var result = Builder.Create()
            .Literal(sample.Value)
            .ReplaceFirst(input, "#");

        await Assert.That(result).IsEqualTo("#|" + sample.Value + "|" + sample.Value);
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(RegexBuilderArbitraries)])]
    public async Task ReplacementBuilder_literal_round_trips_generated_text_through_replace(string value)
    {
        var result = Builder.Create()
            .StartOfLine()
            .Literal("token")
            .EndOfLine()
            .Replace("token", replacement => replacement.Literal(value));

        await Assert.That(result).IsEqualTo(value);
    }
}
