namespace GodotResources.Core.Tokens;

/// <summary>
/// Represents all token types supported by the lexer.
/// </summary>
public enum TokenType
{
    EOF,

    Identifier,
    String,
    Integer,
    Float,

    True,
    False,
    Null,

    LeftBracket,
    RightBracket,

    LeftBrace,
    RightBrace,

    LeftParen,
    RightParen,

    LeftSquare,
    RightSquare,

    Equals,
    Comma,
    Colon,

    Comment,
}
