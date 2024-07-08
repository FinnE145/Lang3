using Lang3.Utils;

namespace Lang3;

class Lexer(Dictionary<string, string> files) {
    public class Token(string type, string value, int line, int start, int end, string file) {
        public string type = type;
        public string value = value;
        public int line = line;
        public int start = start;
        public int end = end;
        public string file = file;

        public override string ToString() {
            return $"Token({type}, {value}, {line}, {start}, {end}, {file})";
        }
    }

    readonly Dictionary<string, string> files = files;

    readonly Errors err = new(files);

    public List<Token> Lex(string fp) {
        files.TryGetValue(fp, out string? code);
        if (code is null) {
            //err.Raise(2, [fp, "found"]);
        }
        Console.WriteLine($"Lexing {code} from {fp}");
        return [];
    }
}