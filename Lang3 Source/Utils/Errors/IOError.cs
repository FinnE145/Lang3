namespace Lang3.Utils.ErrorTypes;

class IOError : Error {
    static readonly string errorName = "IO Error";
    //static readonly string defaultMessage = "The file could not be found or accessed.";
    static readonly int defaultErrorCode = 2;

    public IOError(string? message = null, string[]? files = null, string[]? samples = null, int[]? lines = null, int[]? starts = null, int[]? ends = null, int? errorCode = null) {
        Raise(errorName, message, files, samples, lines, starts, ends, errorCode ?? defaultErrorCode);
    }

    public IOError(Lexer.Token[] tokens, string? message = null, string[]? files = null, string[]? samples = null, int? errorCode = null) {
        Raise(errorName, message, files, samples, tokens?.Select(t => t.line).ToArray(), tokens?.Select(t => t.start).ToArray(), tokens?.Select(t => t.end).ToArray(), errorCode);
    }
}