using System.Diagnostics;

namespace Lang3.Utils;

class Errors(Dictionary<string, string> files) {
    private Dictionary<string, string> files = files;
    static private readonly List<string> errorNames = [
        "Success",  // 0 - None
        "Error",    // 1 - Error
        "IO Error"  // 2 - IOError
    ];
    static private readonly List<string> messageTemplates = [
        "This is not an error.",    // 0 - None
        "An error occurred.",       // 1 - Error
        "`{0}` could not be {1}."   // 2 - IOError
    ];

    public int Raise(int errorCode, string[]? msgArgs, string? file = null, int? line = null, int? start = null, int? end = null, bool fatal = true) {
        string? sample = null;
        if (file is not null) {
            files.TryGetValue(file, out sample);
        }
        sample = line is not null ? sample?.Split('\n')[(int) line] : sample;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{errorNames[errorCode]} [{errorCode}]");
        Console.ResetColor();
        Console.Write(": ");

        string[] str = string.Format(messageTemplates[errorCode], msgArgs ?? []).Split('`');
        for (int i = 0; i < str.Length; i++) {
            Console.Write(str[i++]);
            Console.ForegroundColor = ConsoleColor.Green;
            if (i < str.Length) {
                Console.Write(str[i]);
            }
            Console.ResetColor();
        }
        Console.WriteLine();

        string loc = "";
        if (file is not null) {
            loc += $"{Path.GetFileName(file)}";
            if (line is not null) {
                loc += $":{line}";
                if (start is not null) {
                    loc += $":{start}";
                    if (end is not null) {
                        loc = $"-{end}";
                    }
                }
            }
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(loc);
        if (sample is not null) {
            foreach (string s in sample.Split('\n')) {
                if (s != "" && s is not null) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"> ");
                    Console.ResetColor();
                    Console.WriteLine(s);
                }
            }
            Console.WriteLine(new string(' ', loc.Length + 2) + new string('^', end - start ?? 0));
        }
        Console.ResetColor();

        if (fatal) {
            Environment.Exit(errorCode);
        }
        return errorCode;
    }

    public int Raise(int errorCode, string[]? msgArgs, Lexer.Token[]? token) {
        return Raise(errorCode, msgArgs, token?[0].file, token?[0].line, token?[0].start, token?[0].end);
    }
}