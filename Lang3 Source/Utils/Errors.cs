using System.Dynamic;
using System.Reflection;

namespace Lang3.Utils;

class Errors(Dictionary<string, string> files) : DynamicObject {
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
        if (!Enum.TryParse<ErrorNames>(binder.Name, out ErrorNames error)) {
            result = null;
            return false;
        }

        args ??= [];
        if ((args[0]?.GetType() ?? typeof(string[])) != typeof(string[])) {
            List<string> msgArgs = [];
            int i = 0;
            for (i = 0; i < args.Length; i++) {
                object? s = args[i];
                if ((s?.GetType() ?? typeof(string)) == typeof(string)) {
                    msgArgs.Add((string?)s ?? "");
                } else if (s?.GetType() == typeof(int)){
                    msgArgs.RemoveAt(i--);
                    break;
                } else {
                    break;
                }
            }

            args = [msgArgs.ToArray(), ..args[i++..]];
        }

        //Console.WriteLine($"args: [{string.Join(", ", args)}]");

        //result = GetType().GetMethod("Raise", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, args.Select(a => a?.GetType()).Where(a => a is not null).ToArray()!)?.Invoke(this, [(int) error, msgArgs.ToArray(), args[i..]]);

        if (args.Length == 0) {
            //Console.WriteLine("Trying to raise error with no arguments...");
            result = Raise((int) error);
            return true;
        } else if (args.Length == 1 && args[0] is bool) {
            //Console.WriteLine("Trying to raise error with only fatal...");
            result = Raise((int) error, args[0] as bool?);
            return true;
        } else if (args.Length == 1) {
            //Console.WriteLine("Trying to raise error with only msgArgs...");
            result = Raise((int) error, args[0] as string[]);
        } else if (args.Length == 2 && args[1] is bool) {
            //Console.WriteLine("Trying to raise error with msgArgs and fatal...");
            result = Raise((int) error, args[0] as string[], args[1] as bool?);
            return true;
        } else if (args.Length == 2 && args[1] is string) {
            //Console.WriteLine("Trying to raise error with msgArgs and file...");
            result = Raise((int) error, args[0] as string[], args[1] as string);
            return true;
        } else if (args.Length == 5 && args[1] is string && args[2] is int && args[3] is int && args[4] is int) {
            //Console.WriteLine("Trying to raise error with msgArgs, file, line, start, and end...");
            result = Raise((int) error, args[0] as string[], args[1] as string, args[2] as int?, args[3] as int?, args[4] as int?);
            return true;
        } else if (args.Length == 6 && args[1] is string && args[2] is int && args[3] is int && args[4] is int && args[5] is bool) {
            //Console.WriteLine("Trying to raise error with msgArgs, file, line, start, end, and fatal...");
            result = Raise((int) error, args[0] as string[], args[1] as string, args[2] as int?, args[3] as int?, args[4] as int?, args[5] as bool?);
            return true;
        } else if (args.Length == 2 && args[1] is Lexer.Token) {
            //Console.WriteLine("Trying to raise error with msgArgs and token...");
            result = Raise((int) error, args[0] as string[], args[1] as Lexer.Token);
            return true;
        } else if (args.Length == 3 && args[2] is bool) {
            //Console.WriteLine("Trying to raise error with msgArgs, token, and fatal...");
            result = Raise((int) error, args[0] as string[], args[1] as Lexer.Token, args[2] as bool?);
            return true;
        } else {
            throw new ArgumentException("Provided arguments do not match any overload of Raise");
        }

        /* List<object?[]> overloadOptions = [
            [
                (int) error,
                args.Length >= 1 ? args[0] as string[] : null,
                args.Length >= 2 ? args[1] as string : null,
                args.Length >= 3 ? args[2] as int? : null,
                args.Length >= 4 ? args[3] as int? : null,
                args.Length >= 5 ? args[4] as int? : null,
                args.Length >= 6 ? args[5] as bool? : null
            ],
            [
                (int) error,
                args.Length >= 1 ? args[0] as string[] : null,
                args.Length >= 2 ? args[1] as Lexer.Token : null,
                args.Length >= 3 ? args[2] as bool? : null
            ],
            [
                (int) error,
                msgArgs.ToArray(),
                args.Length >= i + 1 ? args[i] as string : null,
                args.Length >= i + 2 ? args[i + 1] as int? : null,
                args.Length >= i + 3 ? args[i + 2] as int? : null,
                args.Length >= i + 4 ? args[i + 3] as int? : null,
                args.Length >= i + 5 ? args[i + 4] as bool? : null
            ],
            [
                (int) error,
                msgArgs.ToArray(),
                args.Length >= i + 1 ? args[i] as Lexer.Token : null,
                args.Length >= i + 2 ? args[i + 1] as bool? : null
            ],
            [
                (int) error,
                args.Length >= 1 ? args[0] as string[] : null
            ],
            [
                (int) error
            ]
        ]; */

        return result != null;
    }

    public  bool _TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result) {
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