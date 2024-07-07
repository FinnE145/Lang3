namespace Lang3.Utils;

static class Errors {
    static readonly List<string> errorNames = [
        "Error",    // 0 - Error
        "IO Error"  // 1 - IOError
    ];
    static readonly List<string> messageTemplates = [
        "An error occurred.",       // 0 - Error
        "`{0}` could not be {1}."   // 1 - IOError
    ];

    public static void Raise(int errorCode, string[]? msgArgs, string? file = null, int? line = null, int? start = null, int? end = null) {
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
    }
}