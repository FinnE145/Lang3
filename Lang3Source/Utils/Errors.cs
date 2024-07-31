using System.Dynamic;
using System.Reflection;

namespace Lang3.Utils;

class Errors(Dictionary<string, string> files) {
    private readonly Dictionary<string, string> files = files;
    public enum ErrorNames {
        Success,                // 0
        Error,                  // 1
        IOError,                // 2
        InternalError,          // 3
        MalformedTokenError,    // 4
    }
    private readonly List<List<string>> messageTemplates = [
        ["{0}", ""],                    // 0 - None
        ["An error occurred."],         // 1 - Error
        ["`{0}` could not be {1}."],    // 2 - IOError
        ["An error occurred inside the compiler:\n{0}"],    // 3 - InternalError
        ["{0}"],                        // 4 - MalformedTokenError
    ];

    private int RaiseMain(ErrorNames errorType, string[]? msgArgs, string? file, int? line, int? start, int? end, bool? fatal) {
        // Console.WriteLine("Raise called with values:");
        // Console.WriteLine($"errorCode: {errorCode}");
        // Console.WriteLine($"msgArgs: [{(msgArgs is not null && msgArgs.Length > 0 ? string.Join(", ", msgArgs) : "")}]");/*
        // Console.WriteLine($"file: {file}");
        // Console.WriteLine($"line: {line}");
        // Console.WriteLine($"start: {start}");
        // Console.WriteLine($"end: {end}");
        // Console.WriteLine($"fatal: {fatal}"); */

        int errorCode = (int) errorType;

        string? sample = null;
        if (file is not null) {
            files.TryGetValue(file, out sample);
        }
        sample = line is not null ? sample?.Split('\n')[(int) line] : sample;

        string[] str;
        int msgOverload = 0;
        while (true) {
            try {
                string str1 = string.Format(messageTemplates[errorCode][msgOverload], msgArgs ?? []);
                //Console.WriteLine($"msg: {str1}");
                str = str1.Split('`');
                break;
            } catch (FormatException) {
                msgOverload++;
                //Console.WriteLine($"moving to next overload (format): {msgOverload}");
            }

            if (msgOverload >= messageTemplates[errorCode].Count) {
                throw new IndexOutOfRangeException("Message arguments do not match the template for that error.");
            }
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
                        loc += $"-{end}";
                    }
                }
            }
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"sample: {sample}");
        Console.Write(loc);
        if (sample is not null) {
            foreach (string s in sample.Split('\n')) {
                if (s != "") {
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

    // Explicit location and msgArgs array
    public int Raise(ErrorNames errorType, string[] msgArgs, string file, int line, int start, int end, bool fatal = true) {
        return RaiseMain(errorType, msgArgs, file, line, start, end, fatal);
    }

    // Explicit location and msgArg string
    public int Raise(ErrorNames errorType, string msgArgs, string file, int line, int start, int end, bool fatal = true) {
        return RaiseMain(errorType, [msgArgs], file, line, start, end, fatal);
    }

    // Explicit location and no msgArgs
    public int Raise(ErrorNames errorType, string file, int line, int start, int end, bool fatal = true) {
        return RaiseMain(errorType, [], file, line, start, end, fatal);
    }

    // Only line and msgArgs array
    public int Raise(ErrorNames errorType, string[] msgArgs, string file, int line, bool fatal = true) {
        return RaiseMain(errorType, msgArgs, file, line, null, null, fatal);
    }

    // Only line and msgArg string
    public int Raise(ErrorNames errorType, string msgArgs, string file, int line, bool fatal = true) {
        return RaiseMain(errorType, [msgArgs], file, line, null, null, fatal);
    }

    // Only line and no msgArgs
    public int Raise(ErrorNames errorType, string file, int line, bool fatal = true) {
        return RaiseMain(errorType, [], file, line, null, null, fatal);
    }

    // Token and msgArgs array
    public int Raise(ErrorNames errorType, string[] msgArgs, Lexer.Token token, bool fatal = true) {
        return RaiseMain(errorType, msgArgs, token?.file, token?.line, token?.start, token?.end, fatal);
    }

    // Token and msgArg string
    public int Raise(ErrorNames errorType, string msgArgs, Lexer.Token token, bool fatal = true) {
        return RaiseMain(errorType, [msgArgs], token?.file, token?.line, token?.start, token?.end, fatal);
    }

    // Token and no msgArgs
    public int Raise(ErrorNames errorType, Lexer.Token token, bool fatal = true) {
        return RaiseMain(errorType, [], token?.file, token?.line, token?.start, token?.end, fatal);
    }

    // Only file and msgArgs
    public int Raise(ErrorNames errorType, string[] msgArgs, string file, bool fatal = true) {
        return RaiseMain(errorType, msgArgs, file, null, null, null, fatal);
    }

    // Only file and msgArg string
    public int Raise(ErrorNames errorType, string msgArgs, string file, bool fatal = true) {
        return RaiseMain(errorType, [msgArgs], file, null, null, null, fatal);
    }

    /* // Only file and no msgArgs
    public int Raise(ErrorNames errorType, string file, bool fatal = true) {
        return RaiseMain(errorType, [], file, null, null, null, fatal);
    } */

    // No location and msgArgs array
    public int Raise(ErrorNames errorType, string[] msgArgs, bool fatal = true) {
        return RaiseMain(errorType, msgArgs, null, null, null, null, fatal);
    }

    // No location and msgArg string
    public int Raise(ErrorNames errorType, string msgArgs, bool fatal = true) {
        return RaiseMain(errorType, [msgArgs], null, null, null, null, fatal);
    }

    // No location and no msgArgs
    public int Raise(ErrorNames errorType, bool fatal = true) {
        return RaiseMain(errorType, null, null, null, null, null, fatal);
    }
}