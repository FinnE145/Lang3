namespace Lang3;

static class Lexer {
    public class Token(string type, string value, int line, int start, int end) {
        public string type = type;
        public string value = value;
        public int line = line;
        public int start = start;
        public int end = end;

        public override string ToString() {
            return $"Token({type}, {value}, {line}, {start}, {end})";
        }
    }

    public static List<Token> Lex(string code, string fp) {
        Console.WriteLine($"Lexing {code} from {fp}");
        return [];
    }
}