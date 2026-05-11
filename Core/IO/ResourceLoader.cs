using GodotResources.Core.Parsing;
using GodotResources.Core.Tokens;

namespace GodotResources.Core.IO;

/// <summary>
/// Loads resource files.
/// </summary>
public static class ResourceLoader
{
    public static Runtime.ResourceFile Load(string path)
    {
        string text = File.ReadAllText(path);

        Lexer lexer = new(text);

        List<Token> tokens = lexer.Lex();

        Parser parser = new(tokens);

        return parser.Parse();
    }
}
