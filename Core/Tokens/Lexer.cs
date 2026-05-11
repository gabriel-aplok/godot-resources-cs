using System.Text;

namespace GodotResources.Core.Tokens;

/// <summary>
/// Converts source text into a list of tokens.
/// </summary>
public sealed class Lexer(string text)
{
    private readonly string _text = text;
    private int _position;

    private int _line = 1;
    private int _column = 1;

    private char Current => _position >= _text.Length ? '\0' : _text[_position];

    private char Peek(int offset = 1)
    {
        int index = _position + offset;

        return index >= _text.Length ? '\0' : _text[index];
    }

    private void Advance()
    {
        if (Current == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        _position++;
    }

    public List<Token> Lex()
    {
        List<Token> tokens = [];

        while (Current != '\0')
        {
            if (char.IsWhiteSpace(Current))
            {
                Advance();
                continue;
            }

            int line = _line;
            int column = _column;

            switch (Current)
            {
                case '[':
                    tokens.Add(new(TokenType.LeftBracket, "[", line, column));
                    Advance();
                    break;

                case ']':
                    tokens.Add(new(TokenType.RightBracket, "]", line, column));
                    Advance();
                    break;

                case '{':
                    tokens.Add(new(TokenType.LeftBrace, "{", line, column));
                    Advance();
                    break;

                case '}':
                    tokens.Add(new(TokenType.RightBrace, "}", line, column));
                    Advance();
                    break;

                case '(':
                    tokens.Add(new(TokenType.LeftParen, "(", line, column));
                    Advance();
                    break;

                case ')':
                    tokens.Add(new(TokenType.RightParen, ")", line, column));
                    Advance();
                    break;

                case '=':
                    tokens.Add(new(TokenType.Equals, "=", line, column));
                    Advance();
                    break;

                case ':':
                    tokens.Add(new(TokenType.Colon, ":", line, column));
                    Advance();
                    break;

                case ',':
                    tokens.Add(new(TokenType.Comma, ",", line, column));
                    Advance();
                    break;

                case '#':
                    tokens.Add(ReadComment());
                    break;

                case '"':
                    tokens.Add(ReadString());
                    break;

                default:
                {
                    if (char.IsDigit(Current) || Current == '-')
                    {
                        tokens.Add(ReadNumber());
                    }
                    else if (IsIdentifierStart(Current))
                    {
                        tokens.Add(ReadIdentifier());
                    }
                    else
                    {
                        throw new Exception(
                            $"Unexpected character '{Current}' at {_line}:{_column}"
                        );
                    }

                    break;
                }
            }
        }

        tokens.Add(new(TokenType.EOF, string.Empty, _line, _column));

        return tokens;
    }

    private Token ReadComment()
    {
        int line = _line;
        int column = _column;

        Advance();

        StringBuilder sb = new();

        while (Current != '\n' && Current != '\0')
        {
            sb.Append(Current);
            Advance();
        }

        return new(TokenType.Comment, sb.ToString(), line, column);
    }

    private Token ReadString()
    {
        int line = _line;
        int column = _column;

        Advance();

        StringBuilder sb = new();

        while (Current != '"' && Current != '\0')
        {
            if (Current == '\\')
            {
                Advance();

                sb.Append(
                    Current switch
                    {
                        'n' => '\n',
                        't' => '\t',
                        '"' => '"',
                        '\\' => '\\',
                        _ => Current,
                    }
                );

                Advance();
                continue;
            }

            sb.Append(Current);
            Advance();
        }

        if (Current != '"')
        {
            throw new Exception("Unterminated string literal.");
        }

        Advance();

        return new(TokenType.String, sb.ToString(), line, column);
    }

    private Token ReadNumber()
    {
        int line = _line;
        int column = _column;

        StringBuilder sb = new();

        bool hasDot = false;

        if (Current == '-')
        {
            sb.Append(Current);
            Advance();
        }

        while (char.IsDigit(Current) || Current == '.')
        {
            if (Current == '.')
            {
                if (hasDot)
                {
                    break;
                }

                hasDot = true;
            }

            sb.Append(Current);
            Advance();
        }

        return new(hasDot ? TokenType.Float : TokenType.Integer, sb.ToString(), line, column);
    }

    private Token ReadIdentifier()
    {
        int line = _line;
        int column = _column;

        StringBuilder sb = new();

        while (IsIdentifierPart(Current))
        {
            sb.Append(Current);
            Advance();
        }

        string text = sb.ToString();

        return text switch
        {
            "true" => new(TokenType.True, text, line, column),
            "false" => new(TokenType.False, text, line, column),
            "null" => new(TokenType.Null, text, line, column),
            _ => new(TokenType.Identifier, text, line, column),
        };
    }

    private static bool IsIdentifierStart(char c)
    {
        return char.IsLetter(c) || c == '_' || c == '/';
    }

    private static bool IsIdentifierPart(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '/' || c == '.' || c == ':' || c == '@';
    }
}
