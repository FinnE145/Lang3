namespace Lang3.Utils.Errors;

class Error {
    readonly string defaultMessage = "An error occurred.";
    readonly string defaultFile = "<Lang3 Runtime>";
    readonly string defaultSample = "";
    readonly int defaultLine = -1;
    readonly int defaultStart = -1;
    readonly int defaultEnd = -1;
    readonly int errorCode = 1;

    public int Raise(string? message = null, string[]? files = null, string[]? samples = null, int[]? lines = null, int[]? starts = null, int[]? ends = null, int? code = null) {
        message ??= defaultMessage;
        files ??= [defaultFile];
        samples ??= [defaultSample];
        lines ??= [defaultLine];
        starts ??= [defaultStart];
        ends ??= [defaultEnd];

        if (files.Length != samples.Length || samples.Length != lines.Length || lines.Length != starts.Length || starts.Length != ends.Length) {
            throw new Exception("All arrays must have the same length");
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(this.GetType().Name);

        Console.ResetColor();
        Console.WriteLine($": {message}");

        for (int i = 0; i < files.Length; i++) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string loc = $"{Path.GetFileName(files[i])}:{lines[i]}:{starts[i]}-{ends[i]}> ";
            Console.Write(loc);

            Console.ResetColor();
            Console.WriteLine(samples[i]);
        }

        return code ?? errorCode;
    }

    public int Raise(string? message = null, string[]? files = null, string[]? samples = null, Lexer.Token[]? tokens = null, int? code = null) {
        return Raise(message, files, samples, tokens?.Select(t => t.line).ToArray(), tokens?.Select(t => t.start).ToArray(), tokens?.Select(t => t.end).ToArray(), code);
    }

    public int RaiseFullTexts(string? message = null, string[]? files = null, string[]? texts = null, int[]? lines = null, int[]? starts = null, int[]? ends = null, int? code = null) {
        return Raise(message, files, texts?.Select((t, i) => t.Split('\n')[lines?[i] ?? 0]).ToArray(), lines, starts, ends, code);
    }

    public int RaiseFullTexts(string? message = null, string[]? files = null, string[]? texts = null, Lexer.Token[]? tokens = null, int? code = null) {
        return Raise(message, files, texts?.Select((t, i) => t.Split('\n')[tokens?[i].line ?? 0]).ToArray(), tokens?.Select(t => t.line).ToArray(), tokens?.Select(t => t.start).ToArray(), tokens?.Select(t => t.end).ToArray(), code);
    }
}