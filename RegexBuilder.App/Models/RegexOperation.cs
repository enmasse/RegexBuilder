namespace RegexBuilder.App.Models;

public sealed record RegexOperation(RegexOperationKind Kind, string PrimaryValue = "", string SecondaryValue = "");
