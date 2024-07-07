using System.Security;

namespace Lang3.Utils.ErrorTypes;

class Error {
    static readonly string errorName = "Error";
    static readonly string defaultMessage = "An error occurred.";
    static readonly string defaultFile = "Lang3 Runtime";
    static readonly int defaultErrorCode = 1;

    public Error(string? message = null, string[]? files = null, string[]? samples = null, int[]? lines = null, int[]? starts = null, int[]? ends = null, int? errorCode = null) {
        Raise(errorName, message, files, samples, lines, starts, ends, errorCode);
    }

    public static void Raise(string? errorName = "Error", string? message = null, string[]? files = null, string[]? samples = null, int[]? lines = null, int[]? starts = null, int[]? ends = null, int? errorCode = null) {
        message ??= defaultMessage;
        files ??= [defaultFile];
        errorCode ??= defaultErrorCode;

        List<Array?> nonNulls = new([lines, starts, ends]);
        nonNulls.RemoveAll(a => a is null);

        if (nonNulls.Any(a => a?.Length != nonNulls?[0]?.Length)) {
            throw new Exception("All arrays must be the same length");
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{errorName} [{errorCode}]");

        Console.ResetColor();
        Console.WriteLine($": {message}");

        for (int i = 0; i < files.Length; i++) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string loc = $"{Path.GetFileName(files[i])}";
            if (lines?[i] is not null) {
                loc += $":{lines[i]}";
                if (starts?[i] is not null) {
                    loc += $":{starts[i]}";
                    if (ends?[i] is not null) {
                        loc = $"-{ends[i]}";
                    }
                }
            }
            Console.Write($"{loc}> ");

            Console.ResetColor();
            if (samples?[i] is not null) {
                Console.WriteLine(samples[i].Trim().Contains('\n') ? samples[i].Split('\n')[lines?[i] ?? 0] : samples?[i]);
            }
        }
    }

    public Error(Lexer.Token[] tokens, string? message = null, string[]? files = null, string[]? samples = null, int? errorCode = null) {
        Raise(errorName, message, files, samples, tokens?.Select(t => t.line).ToArray(), tokens?.Select(t => t.start).ToArray(), tokens?.Select(t => t.end).ToArray(), errorCode);
    }
}