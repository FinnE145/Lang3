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

    public List<Token> Lex(string fp) {
        files.TryGetValue(fp, out string? code);
        if (code is null) {
            throw new FileNotFoundException($"Tried to lex {fp} but it doesn't exist.");
        }
        Console.WriteLine($"Lexing \n'{code.TrimEnd()}' from {fp}");
        return [];
    }
}