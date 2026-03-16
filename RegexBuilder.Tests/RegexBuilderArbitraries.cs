using FsCheck;
using FsCheck.Fluent;

namespace RegexBuilder.Tests;

public static class RegexBuilderArbitraries
{
    private static readonly char[] LiteralCharacters = ['a', 'b', 'c', 'x', 'y', 'z', '0', '1', '2', ' ', '-', '_', '.', '*', '+', '?', '(', ')', '[', ']', '{', '}', '\\', '^', '$', '|'];
    private static readonly char[] CharacterClassCharacters = ['a', 'b', 'c', 'x', 'y', 'z', '0', '1', '2'];
    private static readonly char[] GroupNameLeadingCharacters = ['a', 'b', 'c', 'x', 'y', 'z', 'A', 'B', 'C', '_'];
    private static readonly char[] GroupNameTrailingCharacters = ['a', 'b', 'c', 'x', 'y', 'z', 'A', 'B', 'C', '0', '1', '2', '_'];

    public static Arbitrary<string> LiteralText() =>
        Arb.From(
            from characters in Gen.ListOf(Gen.Elements(LiteralCharacters))
            select new string(characters.ToArray()));

    public static Arbitrary<string> NonEmptyLiteralText() =>
        Arb.From(
            from characters in Gen.NonEmptyListOf(Gen.Elements(LiteralCharacters))
            select new string(characters.ToArray()));

    public static Arbitrary<string> ValidGroupName() =>
        Arb.From(
            from first in Gen.Elements(GroupNameLeadingCharacters)
            from rest in Gen.ListOf(Gen.Elements(GroupNameTrailingCharacters))
            select first + new string(rest.ToArray()));

    public static Arbitrary<CharacterClassSample> CharacterClassSample() =>
        Arb.From(
            from characters in Gen.NonEmptyListOf(Gen.Elements(CharacterClassCharacters))
            let members = characters.Distinct().ToArray()
            select new CharacterClassSample(new string(members), members));

    public static Arbitrary<LiteralPair> DistinctLiteralPair() =>
        Arb.From(
            from left in NonEmptyLiteralText().Generator
            from right in NonEmptyLiteralText().Generator.Where(value => value != left)
            select new LiteralPair(left, right));

    public static Arbitrary<NamedGroupSample> NamedGroupSample() =>
        Arb.From(
            from name in ValidGroupName().Generator
            from value in NonEmptyLiteralText().Generator
            select new NamedGroupSample(name, value));

    public static Arbitrary<RawPatternSample> RawPatternSample() =>
        Arb.From(
            Gen.Elements(
                new RawPatternSample(@"\d+", "123", "abc"),
                new RawPatternSample("[A-Z]{2}", "AZ", "A1"),
                new RawPatternSample("(cat|dog)", "dog", "cow"),
                new RawPatternSample(@"\w{3}", "abc", "ab")));

    public static Arbitrary<ExactQuantifierSample> ExactQuantifierSample() =>
        Arb.From(
            from count in Gen.Choose(0, 8)
            select new ExactQuantifierSample(count, new string('7', count), new string('7', count + 1)));

    public static Arbitrary<BetweenQuantifierSample> BetweenQuantifierSample() =>
        Arb.From(
            from minimum in Gen.Choose(0, 5)
            from maximum in Gen.Choose(minimum, minimum + 3)
            select new BetweenQuantifierSample(
                minimum,
                maximum,
                new string('7', minimum),
                new string('7', maximum),
                new string('7', maximum + 1)));

    public static Arbitrary<AtLeastQuantifierSample> AtLeastQuantifierSample() =>
        Arb.From(
            from minimum in Gen.Choose(1, 5)
            from additionalLength in Gen.Choose(1, 3)
            select new AtLeastQuantifierSample(
                minimum,
                new string('7', minimum),
                new string('7', minimum + additionalLength),
                new string('7', minimum - 1)));

    public static Arbitrary<RepeatedLiteralSample> RepeatedLiteralSample() =>
        Arb.From(
            from value in NonEmptyLiteralText().Generator
            from count in Gen.Choose(1, 4)
            from largerCount in Gen.Choose(count + 1, count + 3)
            select new RepeatedLiteralSample(value, count, largerCount));

    public static Arbitrary<NonEmptyLiteralSample> NonEmptyLiteralSample() =>
        Arb.From(
            from value in NonEmptyLiteralText().Generator
            select new NonEmptyLiteralSample(value));
}

public readonly record struct CharacterClassSample(string PatternCharacters, char[] Members);

public readonly record struct LiteralPair(string Left, string Right);

public readonly record struct NamedGroupSample(string Name, string Value);

public readonly record struct RawPatternSample(string Pattern, string MatchingText, string NonMatchingText);

public readonly record struct ExactQuantifierSample(int Count, string MatchingText, string NonMatchingText);

public readonly record struct BetweenQuantifierSample(int Minimum, int Maximum, string MinimumText, string MaximumText, string TooLongText);

public readonly record struct AtLeastQuantifierSample(int Minimum, string MinimumText, string LongerText, string TooShortText);

public readonly record struct RepeatedLiteralSample(string Value, int Count, int LargerCount);

public readonly record struct NonEmptyLiteralSample(string Value);
