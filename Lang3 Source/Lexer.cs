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

    private readonly Dictionary<string, string> keywords = new() {
        {"if", "if"},
        {"else", "else"},
        {"while", "while"},
        {"for", "for"},
        {"return", "return"},
        {"?", "blank"}
    };

    private readonly Dictionary<string, string> opers = new() {
        {"+", "add"},
        {"-", "sub"},
        {"*", "mul"},
        {"/", "div"},
        {"**", "exp"},
        {"/^", "root"},
        {"//", "iDiv"},
        {"%", "mod"},
        {">", "gt"},
        {"<", "lt"},
        {">=", "gte"},
        {"<=", "lte"},
        {"==", "eq"},
        {"!=", "neq"},
        {"&&", "and"},
        {"||", "or"},
        {"&", "bAnd"},
        {"|", "bOr"},
        {"^", "xor"},
        {"<<", "lShift"},
        {">>", "rShift"},
        {"=", "assign"},
        {"+=", "addAssign"},
        {"-=", "subAssign"},
        {"*=", "mulAssign"},
        {"/=", "divAssign"},
        {"**=", "expAssign"},
        {"/^=", "rootAssign"},
        {"//=", "iDivAssign"},
        {"%=", "modAssign"},
        {"&&=", "andAssign"},
        {"||=", "orAssign"},
        {"&=", "bAndAssign"},
        {"|=", "bOrAssign"},
        {"^=", "xorAssign"},
        {"<<=", "lShiftAssign"},
        {">>=", "rShiftAssign"},
        {"++", "inc"},
        {"--", "dec"},
        {"!", "not"}
    };

    private readonly Dictionary<string, string> brackets = new() {
        {"(", "lParen"},
        {")", "rParen"},
        {"[", "lBracket"},
        {"]", "rBracket"},
        {"{", "lBrace"},
        {"}", "rBrace"},
        {":", "lCode"},
        {";", "rCode"}
    };

    internal static readonly char[] opStarts = ['+', '-', '*', '/', '%', '>', '<', '=', '&', '|', '^', '!'];

    private Token BucketNum(string code, int line, int ld, string fp, ref int i) {
        int start = i++;
        string t = "int";
        while (i-- < code.Length && (char.IsDigit(code[++i]) || code[i] == '.')) {
            if (code[i++] == '.') {
                if (t == "dec") {
                    throw new Exception("Number cannot have two decimal points");
                }
                t = "dec";
            }
        }
        if (code[--i] == '.') {
            throw new Exception("Number cannot end with a decimal point");
        }
        return new Token(t, code[start..(i+1)], line, start-ld, i-ld+1, fp);
    }

    public List<Token> Lex(string fp) {
        int line = 1;
        int ld = 0;

        files.TryGetValue(fp, out string? code);
        if (code is null) {
            throw new FileNotFoundException($"Tried to lex {fp} but it doesn't exist.");
        }

        List<Token> tokens = [];

        for (int i = 0; i < code.Length; i++) {
            char c = code[i];

            if (c == ' ' || c == '\t') {
                continue;
            }

            if (c == '\n') {
                line++;
                ld = i+1;
                continue;
            }

            if (char.IsDigit(c)) {
                tokens.Add(BucketNum(code, line, ld, fp, ref i));
            }

            string v = c != '\n' ? c.ToString() : "\\n";
        }

        tokens.Add(new Token("EOF", "", line, code.Length, code.Length, fp));

        return tokens;
    }
}