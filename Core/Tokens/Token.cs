namespace GodotResources.Core.Tokens;

/// <summary>
/// Represents a lexical token.
/// </summary>
public readonly record struct Token(TokenType Type, string Text, int Line, int Column);
