namespace GodotResources.Core;

public class Logger
{
    public enum Level
    {
        Info,
        Warn,
        Error,
        Init,
        Verbose,
        Netcode,
    }

    private static bool _fileMode;
    private static string _debugFileName = "";
    private static string _debugFilePath = "";

    // private static readonly Lock loggerLock = new Lock(); dotnet 9.+
    private static readonly object _loggerLock = new();

    private static readonly string[] _prefixes =
    {
        " INFO  ",
        " WARN  ",
        " ERROR ",
        " INIT  ",
        " VERBOSE ",
        " NET   ",
    };

    public static void EnableFileMode()
    {
        _fileMode = true;
        string logDirectory = "logs";

        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        _debugFileName = $"DEBUG-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
        _debugFilePath = Path.Combine(logDirectory, _debugFileName);
    }

    public static void Print(string message, Level level)
    {
        string prefix = _prefixes[(int)level];

        // using (loggerLock.EnterScope()) { dotnet 9.+
        lock (_loggerLock)
        {
            switch (level)
            {
                case Level.Info:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case Level.Warn:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case Level.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case Level.Init:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case Level.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case Level.Netcode:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
            }

            Console.Write(prefix);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($" {message}");

            if (_fileMode && !string.IsNullOrEmpty(_debugFilePath))
            {
                try
                {
                    File.AppendAllText(_debugFilePath, $"{prefix} {message}\n");
                }
                catch { }
            }
        }
    }

    public static void Info(string message)
    {
        Print(message, Level.Info);
    }

    public static void Warn(string message)
    {
        Print(message, Level.Warn);
    }

    public static void Error(string message)
    {
        Print(message, Level.Error);
    }

    public static void Init(string message)
    {
        Print(message, Level.Init);
    }

    public static void NetInfo(string message)
    {
        Print(message, Level.Netcode);
    }

    public static void Verbose(string message)
    {
        if (_fileMode)
        {
            Print(message, Level.Verbose);
        }
    }

    public static void CrashReport(string logMessage)
    {
        string logDirectory = "logs";
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        string logFileName = $"CRASH-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
        string logFilePath = Path.Combine(logDirectory, logFileName);

        try
        {
            File.WriteAllText(logFilePath, logMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL] Failed to write crash report: {ex.Message}");
        }
    }
}
