namespace RegexBuilder.App.Models;

public enum RegexOperationKind
{
    StartOfLine,
    EndOfLine,
    Literal,
    Raw,
    AnyCharacter,
    Digit,
    NonDigit,
    WordCharacter,
    NonWordCharacter,
    Whitespace,
    NonWhitespace,
    CharacterClass,
    NegatedCharacterClass,
    Range,
    GroupRaw,
    NamedGroupRaw,
    EitherRaw,
    Optional,
    ZeroOrMore,
    OneOrMore,
    Exactly,
    AtLeast,
    Between
}
