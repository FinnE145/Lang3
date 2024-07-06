namespace Lang3.Utils.Errors;

class Error {
    readonly string errorName = "Error";
    readonly string defaultMessage = "An error occurred.";
    readonly string defaultFile = "Lang3 Runtime";
    readonly string defaultSample = "";
    readonly int defaultLine = -1;
    readonly int defaultStart = -1;
    readonly int defaultEnd = -1;
    readonly int defaultErrorCode = 1;

    public Error(string? message = null, string[]? files = null, string[]? samples = null, int[]? lines = null, int[]? starts = null, int[]? ends = null, int? errorCode = null) {
        message ??= this.defaultMessage;
        files ??= [this.defaultFile];
        samples ??= [this.defaultSample];
        lines ??= [this.defaultLine];
        starts ??= [this.defaultStart];
        ends ??= [this.defaultEnd];
        errorCode ??= this.defaultErrorCode;

        if (files.Length != samples.Length || samples.Length != lines.Length || lines.Length != starts.Length || starts.Length != ends.Length) {
            throw new Exception("All arrays must be the same length");
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{errorName} [{errorCode}]");

        Console.ResetColor();
        Console.WriteLine($": {message}");

        for (int i = 0; i < files.Length; i++) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string loc = $"{Path.GetFileName(files[i])}";
            if (lines[i] != -1) {
                loc += $":{lines[i]}";
                if (starts[i] != -1) {
                    loc += $":{starts[i]}";
                    if (ends[i] != -1) {
                        loc = $"-{ends[i]}";
                    }
                }
            }
            Console.Write($"{loc}> ");

            Console.ResetColor();
            Console.WriteLine(samples[i].Trim().Contains('\n') ? samples[i].Split('\n')[lines[i]] : samples[i]);
        }
    }

    public Error(Lexer.Token[] tokens, string? message = null, string[]? files = null, string[]? samples = null, int? errorCode = null) : this(message, files, samples, tokens?.Select(t => t.line).ToArray(), tokens?.Select(t => t.start).ToArray(), tokens?.Select(t => t.end).ToArray(), errorCode) {}
}