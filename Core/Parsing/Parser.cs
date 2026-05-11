using GodotResources.Core.Runtime;
using GodotResources.Core.Tokens;

namespace GodotResources.Core.Parsing;

/// <summary>
/// Recursive descent parser.
/// </summary>
public sealed class Parser(List<Token> tokens)
{
    private readonly List<Token> _tokens = tokens;
    private int _position;

    private Token Current => Peek(0);

    private Token Peek(int offset)
    {
        int index = _position + offset;

        return index >= _tokens.Count ? _tokens[^1] : _tokens[index];
    }

    private Token Consume()
    {
        return _tokens[_position++];
    }

    private Token Expect(TokenType type)
    {
        if (Current.Type != type)
        {
            throw new ParseException(
                $"Expected {type} but got {Current.Type} at {Current.Line}:{Current.Column}"
            );
        }

        return Consume();
    }

    public ResourceFile Parse()
    {
        ResourceFile file = new();

        while (Current.Type != TokenType.EOF)
        {
            ParseSection(file);
        }

        return file;
    }

    private void ParseSection(ResourceFile file)
    {
        Expect(TokenType.LeftBracket);
        string sectionName = Expect(TokenType.Identifier).Text;

        if (sectionName == "gd_resource")
        {
            ParseHeader(file);
        }
        else if (sectionName == "ext_resource")
        {
            file.ExternalResources.Add(ParseExternalResource());
        }
        else if (sectionName == "sub_resource")
        {
            file.SubResources.Add(ParseSubResource());
        }
        else
        {
            ParseGenericSection(file, sectionName);
        }
    }

    private SubResource ParseSubResource()
    {
        SubResource sub = new();

        while (Current.Type != TokenType.RightBracket)
        {
            string key = Expect(TokenType.Identifier).Text;
            Expect(TokenType.Equals);
            Variant value = ParseValue();

            if (key == "type")
            {
                sub.Type = value.Get<string>();
            }
            else if (key == "id")
            {
                sub.Id = value.Get<string>();
            }
        }
        Expect(TokenType.RightBracket);

        while (Current.Type == TokenType.Identifier)
        {
            string key = Consume().Text;
            Expect(TokenType.Equals);
            sub[key] = ParseValue();
        }

        return sub;
    }

    private void ParseHeader(ResourceFile file)
    {
        while (Current.Type != TokenType.RightBracket)
        {
            string key = Expect(TokenType.Identifier).Text;

            Expect(TokenType.Equals);

            Variant value = ParseValue();

            switch (key)
            {
                case "type":
                    file.ResourceType = value.Get<string>();
                    break;

                case "script_class":
                    file.ScriptClass = value.Get<string>();
                    break;

                case "format":
                    file.FormatVersion = value.Get<int>();
                    break;

                case "uid":
                    file.Uid = value.Get<string>();
                    break;
            }
        }
        Expect(TokenType.RightBracket);
    }

    private ExternalResource ParseExternalResource()
    {
        ExternalResource ext = new();
        while (Current.Type != TokenType.RightBracket)
        {
            string key = Expect(TokenType.Identifier).Text;

            Expect(TokenType.Equals);

            Variant value = ParseValue();

            switch (key)
            {
                case "id":
                    ext.Id = value.Get<string>();
                    break;

                case "type":
                    ext.Type = value.Get<string>();
                    break;

                case "path":
                    ext.Path = value.Get<string>();
                    break;

                case "uid":
                    ext.Uid = value.Get<string>();
                    break;
            }
        }
        Expect(TokenType.RightBracket);
        return ext;
    }

    private void ParseGenericSection(ResourceFile file, string name)
    {
        ResourceSection section = new(name);

        Expect(TokenType.RightBracket);

        while (Current.Type == TokenType.Identifier)
        {
            string key = Consume().Text;
            Expect(TokenType.Equals);
            section[key] = ParseValue();
        }

        file.Sections.Add(section);
    }

    private Variant ParseValue()
    {
        return Current.Type switch
        {
            TokenType.String => new Variant(Consume().Text),
            TokenType.Integer => new Variant(int.Parse(Consume().Text)),
            TokenType.Float => new Variant(
                float.Parse(Consume().Text, System.Globalization.CultureInfo.InvariantCulture)
            ),

            TokenType.True => ParseBool(true),
            TokenType.False => ParseBool(false),

            TokenType.Null => ParseNull(),

            TokenType.Identifier => ParseIdentifierExpression(),

            TokenType.LeftBracket => ParseArray(),
            TokenType.LeftBrace => ParseDictionary(),

            _ => throw new ParseException($"Unexpected token {Current.Type}"),
        };
    }

    private Variant ParseBool(bool value)
    {
        Consume();
        return new Variant(value);
    }

    private Variant ParseNull()
    {
        Consume();
        return new Variant(null);
    }

    private Variant ParseIdentifierExpression()
    {
        string identifier = Consume().Text;

        if (identifier == "ExtResource")
        {
            Expect(TokenType.LeftParen);

            string id = Expect(TokenType.String).Text;

            Expect(TokenType.RightParen);

            return new Variant(new ExtResourceReference(id));
        }

        if (identifier == "SubResource")
        {
            Expect(TokenType.LeftParen);

            string id = Expect(TokenType.String).Text;

            Expect(TokenType.RightParen);

            return new Variant(new SubResourceReference(id));
        }

        return new Variant(identifier);
    }

    private Variant ParseArray()
    {
        Consume(); // consume '['
        List<object?> list = [];

        while (Current.Type != TokenType.RightBracket && Current.Type != TokenType.EOF)
        {
            list.Add(ParseValue().Value);

            if (Current.Type == TokenType.Comma)
            {
                Consume(); // s kip commas
            }
        }

        Expect(TokenType.RightBracket);
        return new Variant(list);
    }

    private Variant ParseDictionary()
    {
        Consume(); // consume '{'
        Dictionary<object, object?> dict = [];

        while (Current.Type != TokenType.RightBrace && Current.Type != TokenType.EOF)
        {
            object? key = ParseValue().Value;
            Expect(TokenType.Colon);
            object? value = ParseValue().Value;

            if (key != null)
            {
                dict[key] = value;
            }

            if (Current.Type == TokenType.Comma)
            {
                Consume();
            }
        }

        Expect(TokenType.RightBrace);
        return new Variant(dict);
    }
}
