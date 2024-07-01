namespace Lang3.Utils.Errors;

class Error {
    string defaultMessage = "An error occurred.";
    string defaultFile = "<Lang3 Runtime>";
    string defaultSample = "";
    int defaultLine = -1;
    int defaultStart = -1;
    int defaultEnd = -1;
    int errorCode = 1;

    public int Raise(string? message = null, string[]? files = null, string[]? samples = null, int[]? lines = null, int[]? starts = null, int[]? ends = null, int? code = null) {
        string msg = message ?? defaultMessage;
        string[] f = files ?? [defaultFile];
        string[] sp = samples ?? [defaultSample];
        int[] l = lines ?? [defaultLine];
        int[] st = starts ?? [defaultStart];
        int[] e = ends ?? [defaultEnd];

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(this.GetType().Name);

        Console.ResetColor();
        Console.WriteLine($": {msg}");

        for (int i = 0; i < f.Length; i++) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string loc = $"{Path.GetFileName(f[i])}:{l[i]}:{st[i]}-{e[i]}> ";
            Console.Write(loc);

            Console.ResetColor();
            Console.WriteLine(sp[i]);
        }

        return code ?? errorCode;
    }
}