using Lang3.Utils;
using static Lang3.Utils.Errors.ErrorNames;

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

    private Token BucketNum(int line, int ld, string fp, ref int i) {
        string code = files[fp]!;
        int start = i++;
        string t = "int";
        while (i-- < code.Length && (char.IsDigit(code[++i]) || code[i] == '.')) {
            if (code[i++] == '.') {
                if (t == "dec") {
                    err.Raise(MalformedTokenError, "Number cannot have two decimal points", fp, line, start-ld, i-ld+1);
                }
                t = "dec";
            }
        }
        if (code[--i] == '.') {
            err.Raise(MalformedTokenError, "Number cannot end with a decimal point", fp, line, start-ld, i-ld+1);
        }
        return new Token(t, code[start..(i+1)], line, start-ld, i-ld+1, fp);
    }

    private Token BucketOp(int line, int ld, string fp, ref int i) {
        string code = files[fp]!;
        int start = i++;
        while (i < code.Length && opers.ContainsKey(code[start..(i+1)])) {
            i++;
        }
        string sample = code[start..(--i+1)];
        return new Token(sample == "++" || sample == "--" || sample == "!" ? "uOperator" : "operator", opers[sample], line, start-ld, i-ld+1, fp);
    }

    private Token BucketBracket(int line, int ld, string fp, ref int i) {
        string code = files[fp]!;
        return new Token(brackets[code[i].ToString()], code[i].ToString(), line, i-ld, i-ld+1, fp);
    }

    private Token BucketWord(int line, int ld, string fp, ref int i) {
        string code = files[fp]!;
        int start = i++;
        while (i < code.Length && (char.IsLetter(code[i]) || char.IsDigit(code[i]) || code[i] == '_')) {
            i++;
        }
        string s = code[start..i--];
        return new Token(keywords.TryGetValue(s, out string? value) ? value : "var", s, line, start-ld, i-ld+1, fp);
    }

    public List<Token> Lex(string fp) {
        int line = 0;
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
                tokens.Add(BucketNum(line, ld, fp, ref i));
                continue;
            }

            if (opStarts.Contains(c)) {
                tokens.Add(BucketOp(line, ld, fp, ref i));
                continue;
            }

            if (brackets.ContainsKey(c.ToString())) {
                tokens.Add(BucketBracket(line, ld, fp, ref i));
                continue;
            }

            if (char.IsLetter(c) || c == '_') {
                tokens.Add(BucketWord(line, ld, fp, ref i));
                continue;
            }

            string v = c != '\n' ? c.ToString() : "\\n";
        }

        tokens.Add(new Token("EOF", "", line, code.Length, code.Length, fp));

        return tokens;
    }
}