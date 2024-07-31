using System.Dynamic;
using System.Reflection;

namespace Lang3.Utils;

class ErrorsStatic(Dictionary<string, string> files) {
    private readonly Dictionary<string, string> files = files;
    private enum ErrorNames {
        Success,                // 0
        Error,                  // 1
        IOError,                // 2
        InternalError,          // 3
        MalformedTokenError,    // 4
    }
    private readonly List<List<string>> messageTemplates = [
        ["Code ran without error"],      // 0 - None
        ["An error occurred."],         // 1 - Error
        ["`{0}` could not be {1}."],    // 2 - IOError
        ["An error occurred inside the compiler:\n{0}"],    // 3 - InternalError
        ["{0}"],                        // 4 - MalformedTokenError
    ];

    public int Raise(int errorCode, string[]? msgArgs = null, string? file = null, int? line = null, int? start = null, int? end = null, bool? fatal = true) {
        /* Console.WriteLine("Raise called with values:");
        Console.WriteLine($"errorCode: {errorCode}");
        Console.Write($"msgArgs: [");
        Array.ForEach(msgArgs?[..^1] ?? [], s => Console.Write($"'{s}', "));
        Console.WriteLine($"{msgArgs?[^1]}]");
        Console.WriteLine($"file: {file}");
        Console.WriteLine($"line: {line}");
        Console.WriteLine($"start: {start}");
        Console.WriteLine($"end: {end}");
        Console.WriteLine($"fatal: {fatal}"); */

        string? sample = null;
        if (file is not null) {
            files.TryGetValue(file, out sample);
        }
        sample = line is not null ? sample?.Split('\n')[(int) line] : sample;

        string[] str;
        int msgOverload = 0;
        try {
            while (true) {
                try {
                    str = string.Format(messageTemplates[errorCode][msgOverload], msgArgs ?? []).Split('`');
                    break;
                } catch (FormatException) {
                    msgOverload++;
                }
            }
        } catch (IndexOutOfRangeException) {
            throw new ArgumentException("Message arguments do not match the template for that error.");
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{(ErrorNames) errorCode} [{errorCode}]");
        Console.ResetColor();
        Console.Write(": ");
        
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

        if (fatal ?? true) {
            Environment.Exit(errorCode);
        }
        return errorCode;
    }

    public int Raise(int errorCode, string[]? msgArgs, Lexer.Token? token, bool? fatal = true) {
        return Raise(errorCode, msgArgs, token?.file, token?.line, token?.start, token?.end, fatal);
    }

    public int Raise(int errorCode, string[]? msgArgs, bool? fatal) {
        return Raise(errorCode, msgArgs, null, null, null, null, fatal);
    }

    public int Raise(int errorCode, bool? fatal) {
        return Raise(errorCode, null, null, null, null, null, fatal);
    }
}