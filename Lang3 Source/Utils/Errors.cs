using System.Dynamic;

namespace Lang3.Utils;

class Errors(Dictionary<string, string> files) : DynamicObject {
    private readonly Dictionary<string, string> files = files;
    private enum ErrorNames {
        Success,        // 0
        Error,          // 1
        IOError,        // 2
        InternalError,  // 3
        MalformedTokenError,  // 4
    }
    private readonly List<string> messageTemplates = [
        "This is not an error.",    // 0 - None
        "An error occurred.",       // 1 - Error
        "`{0}` could not be {1}.",  // 2 - IOError
        "An error occurred inside the compiler code:\n{0}",  // 3 - InternalError
        "Malformed token: {0}",     // 4 - MalformedTokenError
    ];

    public override IEnumerable<string> GetDynamicMemberNames() {
        return Enum.GetNames<ErrorNames>();
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result) { 
        if (!Enum.TryParse<ErrorNames>(binder.Name, out ErrorNames error)) {
            result = null;
            return false;
        } else {
            result = (int) error;
            return true;
        }
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result) {
        // Error(string[]? msgArgs, string? file = null, int? line = null, int? start = null, int? end = null, bool? fatal = true)
        // Error(string? msg, string? file = null, int? line = null, int? start = null, int? end = null, bool? fatal = true)
        // Error(string[]? msgArgs, Lexer.Token? token, bool? fatal = true)
        // Error(string msgArg..., Lexer.Token? token, bool? fatal = true)

        if (!Enum.TryParse<ErrorNames>(binder.Name, out ErrorNames error)) {
            // Error doesn't exist

            result = null;
            return false;
        } else if (args is not null && args.Length > 0) {
            // There are arguments

            if (args.Length >= 1 && args[0] is not null && args[0]?.GetType() == typeof(string)) {
                // The first argument is a string

                // Add arguments to msgArgs until a non-string is found
                List<string> msgArgs = [];
                int i;
                for (i = 0; i < args.Length; i++) {
                    object? s = args[i];
                    if ((s?.GetType() ?? typeof(string)) == typeof(string)) {
                        msgArgs.Add((string?) s ?? "");
                    } else {
                        break;
                    }
                }

                if (args.Length == i + 1 && args[i]?.GetType() != typeof(Lexer.Token)) {
                    // The first arg after the string is not a Lexer.Token
                    throw new ArgumentException("Argument 'token' must be of type Lexer.Token.");
                } else if (args.Length == i + 2 && (args[i]?.GetType() != typeof(Lexer.Token) || args[i]?.GetType() != typeof(bool))) {
                    // The first arg after the string is not a Lexer.Token or the second arg is not a bool
                    throw new ArgumentException("Arguments 'token' and 'fatal' must be of type Lexer.Token and bool, respectively.");
                }

                // Print an error with the token overload
                result = Raise((int) error, [.. msgArgs], args.Length >= i + 1 ? args[i] as Lexer.Token : null, args.Length >= i + 2 ? args[i + 1] as bool? : null);
            } else if (args.Length >= 1 && args[0] is not null && args[0]?.GetType() != typeof(string[])) {
                // 1 or more args and the first arg is not a string[]
                 throw new ArgumentException("Argument 'msgArgs' (1) must be of type string[].");
            }
            if (args.Length >= 2) {
                // 2 or more args

                if ((args[1] is null && args.Length >= 3 && args[2]?.GetType() == typeof(int)) || args[1]?.GetType() == typeof(string)) {
                    // The second arg is null and there are more args with the next one being an int, or the second arg is a string

                    if (
                        (args.Length >= 3 && args[2] is not null && args[2]?.GetType() != typeof(int)) ||
                        (args.Length >= 4 && args[3] is not null && args[3]?.GetType() != typeof(int)) ||
                        (args.Length >= 5 && args[2] is not null && args[4]?.GetType() != typeof(int))
                    ) {
                        // 3, 4, or 5 args and the 3rd, 4th, or 5th arg is not an int
                        throw new ArgumentException("Arguments 'line' (3), 'start' (4), and 'end' (5) must all be of type int.");
                    } else if (args.Length >= 6 && args[5] is not null && args[5]?.GetType() != typeof(bool)) {
                        // 6 args and the 6th arg is not a bool
                        throw new ArgumentException("Argument 'fatal' (6) must be of type bool.");
                    }

                    // Print an error with the explicit location overload
                    result = Raise((int) Enum.Parse<ErrorNames>(binder.Name), args[0] as string[], args[1] as string, args.Length >= 3 ? args[2] as int? : null, args.Length >= 4 ? args[3] as int? : null, args.Length >= 5 ? args[4] as int? : null, args.Length >= 6 ? args[5] as bool? : null);
                } else if (args[1] is null || args[1]?.GetType() == typeof(Lexer.Token)) {
                    // The second arg is null or the second arg is a Lexer.Token

                    if (args.Length > 2 && args[2] is not null && args[2]?.GetType() != typeof(bool)) {
                        // 3 args and the 3rd arg is not a bool
                        throw new ArgumentException("Argument 'fatal' (3) must be of type bool.");
                    }

                    // Print an error with the token overload
                    result = Raise((int) Enum.Parse<ErrorNames>(binder.Name), args[0] as string[], args[1] as Lexer.Token, args.Length >= 3 ? args[2] as bool? : null);
                } else {
                    // The second arg is not a string, null with more args afterwards, or a Lexer.Token
                    throw new ArgumentException("Argument 2 must be of type string ('file') or Lexer.Token ('token').");
                }
            } else {
                // 1 arg or the second arg is null

                // Raise an error with the explicit location overload, with no location
                result = Raise((int) Enum.Parse<ErrorNames>(binder.Name), args[0] as string[]);
            }
        } else {
            // There are no arguments

            // Raise an error with no msgArgs or location
            result = Raise((int) Enum.Parse<ErrorNames>(binder.Name));
        }
        return true;
    }

    private int Raise(int errorCode, string[]? msgArgs = null, string? file = null, int? line = null, int? start = null, int? end = null, bool? fatal = true) {
        string? sample = null;
        if (file is not null) {
            files.TryGetValue(file, out sample);
        }
        sample = line is not null ? sample?.Split('\n')[(int) line] : sample;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{(ErrorNames) errorCode} [{errorCode}]");
        Console.ResetColor();
        Console.Write(": ");

        string[] str;
        try {
            str = string.Format(messageTemplates[errorCode], msgArgs ?? []).Split('`');
        } catch (FormatException) {
            throw new ArgumentException("Message arguments do not match the template for that error.");
        }
        
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

    private int Raise(int errorCode, string[]? msgArgs, Lexer.Token? token, bool? fatal = true) {
        return Raise(errorCode, msgArgs, token?.file, token?.line, token?.start, token?.end, fatal);
    }
}