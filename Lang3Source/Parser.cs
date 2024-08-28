using Lang3.Utils;
using static Lang3.Utils.Errors.ErrorNames;

namespace Lang3;

class Parser(Dictionary<string, string> fileCode, Dictionary<string, List<Lexer.Token>> fileTokens) {
    public class Node {
        public string type;
        public string value;
        public List<Node> children = [];
        public Lexer.Token token;

        public Node (string type, string value, Lexer.Token token) {
            this.type = type;
            this.value = value;
            this.token = token;
        }

        public Node() {
            type = "";
            value = "";
            token = new("", "", 0, 0, 0, "");
        }

        private readonly int depthMod = 4;

        private string ChildrenToString() {
            return ChildrenToString(true);
        }

        private string ChildrenToString(bool showToken, int depth = 0) {
            if (children.Count == 0) {
                return "";
            }
            string s = ", [\n";
            foreach (Node child in children) {
                s += $"{child.ToString(showToken, depth + depthMod)}, \n";
            }
            return $"{s[..^3] + '\n'}{new string(' ', depth)}]";
        }

        private string ChildrenToString(bool showLocation, bool showFile, int depth = 0) {
            if (children.Count == 0) {
                return "";
            }
            string s = ", [\n";
            foreach (Node child in children) {
                s += $"  {child.ToString(showLocation, showFile, depth + depthMod)}, \n";
            }
            return $"{s[..^3] + '\n'}{new string(' ', depth)}]";
        }

        public override string ToString() {
            return $"Node({type}, '{value}', {ChildrenToString()}, {token})";
        }

        public string ToString(bool showToken, int depth = 0) {
            return $"{new string(' ', depth)}Node({type}{(value != "" ? ", " + value : "")}{ChildrenToString(showToken, depth)}{(showToken ? $", {token}" : "")})";
        }

        public string ToString(bool showLocation, bool showFile, int depth = 0) {
            return $"{new string(' ', depth)}Node({type}{(value != "" ? ", " + value : "")}{ChildrenToString(showLocation, showFile, depth)}, {token.ToString(showLocation, showFile)})";
        }
    }

    private readonly List<string> unaryStart = ["inc", "dec", "bNot", "not"];
    private readonly List<string> unaryEnd = ["inc", "dec"];

    readonly Errors err = new(fileCode);

    private static readonly List<string> valueTypes = ["number", "parens", "operation", "var"];

    private static readonly Dictionary<string, int> precedence = new() {
        {"add", 1},
        {"sub", 1},
        {"mul", 2},
        {"div", 2},
        {"exp", 3},
        {"root", 3},
        {"iDiv", 2},
        {"mod", 2},
        {"gt", 0},
        {"lt", 0},
        {"gte", 0},
        {"lte", 0},
        {"eq", 0},
        {"neq", 0},
        {"and", -2},
        {"or", -2},
        {"bAnd", 1},
        {"bOr", 1},
        {"xor", 1},
        {"lShift", 2},
        {"rShift", 2},
        {"assign", -5},
        {"addAssign", -5},
        {"subAssign", -5},
        {"mulAssign", -5},
        {"divAssign", -5},
        {"expAssign", -5},
        {"rootAssign", -5},
        {"iDivAssign", -5},
        {"modAssign", -5},
        {"bAndAssign", -5},
        {"bOrAssign", -5},
        {"xorAssign", -5},
        {"lShiftAssign", -5},
        {"rShiftAssign", -5},
        {"inc", 4},
        {"dec", 4},
        {"bNot", 4},
        {"not", 4}
    };

    List<int> lastIs = [];

    private void ParseParens(List<Lexer.Token> tokens, Node root, ref int i) {
        Node node = new("parens", "", tokens[i]);
        root.children.Add(node);

        i++;
        if (Parse(tokens, node, ref i, "rParen")) {
            i++;
            // Console.WriteLine(i);
            return;
        } else {
            // TODO: add a new error type for this
            err.Raise(Error, "Unmatched parenthesis", tokens[i]);
        }
    }

    private void ParsePreUnaryOps(List<Lexer.Token> tokens, Node root, ref int i) {
        Node op = new("operation", tokens[i].value, tokens[i]);
        int oldI = i;
        i++;
        if (!Parse(tokens, op, ref i, maxTokens: 1) || (op.children.Count > 0 && !valueTypes.Contains(op.children[^1].type))) {
            // TODO: add a new error type for this
            err.Raise(Error, $"Expected an expression after the {tokens[oldI].value} operator, but received {tokens[oldI+1].type}", tokens[oldI+1].type == "EOF" ? tokens[oldI] : tokens[oldI+1], false);
            i++;
        } else {
            root.children.Add(op);
        }
    }

    private void ParsePostUnaryOps(List<Lexer.Token> tokens, Node root, ref int i) {
        Node op = new("operation", tokens[i].value, tokens[i]);
        if (root.children.Count == 0 || !valueTypes.Contains(root.children[0].type)) {
            // TODO: add a new error type for this
            err.Raise(Error, $"Expected an expression before the {tokens[i].value} operator, but received {tokens[i++].type}", tokens[i], false);
        } else {
            op.children.Add(root.children[^1]);
            root.children.RemoveAt(root.children.Count - 1);
            root.children.Add(op);
            i++;
        }
    }

    private static bool SwitchArgs(Node op, Node lastOp, Node? root = null) {

        // Console.WriteLine("Possibly switching " + lastOp.ToString(false) + " and " + op.ToString(false));
        bool addNode = true;

        if (lastOp.children.Count == 0 || lastOp.type != "operation") {

            // Console.WriteLine("NOT AN OP - " + lastOp.ToString(false) + (lastOp.type != "operator" ? " is a " + lastOp.type : "has no children"));

            op.children.Add(lastOp);
            root?.children.RemoveAt(root.children.Count - 1);
        } else if (precedence[op.value] > precedence[lastOp.value]) {

            // Console.WriteLine("WRONG ORDER - " + op.value + " is higher precedence than " + lastOp.value);

            if (lastOp.children[^1].children.Count != 0 && lastOp.children[^1].type == "operation" && !ReferenceEquals(lastOp.children[^1], op)) {
                SwitchArgs(op, lastOp.children[^1], lastOp);
            } else {
                op.children.Add(lastOp.children[^1]);
                lastOp.children.RemoveAt(lastOp.children.Count - 1);
                lastOp.children.Add(op);
            }

            addNode = false;
        } else {
            // Console.WriteLine("RIGHT ORDER - " + op.value + " comes after " + lastOp.value);

            op.children.Add(lastOp);
            root?.children.RemoveAt(root.children.Count - 1);
        }

        return addNode;
    }

    private void ParseBinaryOps(List<Lexer.Token> tokens, Node root, ref int i) {
        Node op = new("operation", tokens[i].value, tokens[i]);
        i++;
        if (root.children.Count == 0 || !valueTypes.Contains(root.children[0].type)) {
            // TODO: add a new error type for this
            err.Raise(Error, $"Expected an expression before the {tokens[i-1].value} operator, but received {(i-2 >= 0 ? tokens[i-2].type : "nothing")}", tokens[i-2 >= 0 ? i-2 : i-1], false);
        } else {
            bool addNode = SwitchArgs(op, root.children[^1], root);

            Node argHolder = new("argHolder", "", new("", "", 0, 0, 0, ""));
            int oldI = i;
            bool parseResult = Parse(tokens, argHolder, ref i, maxTokens: 1);

            if (parseResult && argHolder.children.Count != 0 && valueTypes.Contains(argHolder.children[^1].type)) {
                op.children.Add(argHolder.children[^1]);
                if (addNode) {
                    root.children.Add(op);
                }
            } else {
                // TODO: add a new error type for this
                err.Raise(Error, $"Expected an expression after the {tokens[oldI-1].value} operator, but received {tokens[oldI].type}", tokens[oldI], false);
            }
        }
    }

    public Node Parse(string fp) {
        List<Lexer.Token> tokens = fileTokens[fp];

        Node root = new("root", "", new("", "", 0, 0, 0, fp));
        int i = 0;
        Parse(tokens, root, ref i);
        return root;
    }

    private bool Parse(List<Lexer.Token> tokens, Node node, ref int i, string endType = "<NOT_A_TYPE>", int maxTokens = -1) {
        int startI = i;

        while (i < tokens.Count) {
            Lexer.Token t = tokens[i];

            if (t.type == endType || (maxTokens != -1 && i - startI >= maxTokens)) {
                return true;
            }
            
            if (t.type == "EOF") {
                return false;
            }
            
            if (t.type == "int" || t.type == "dec") {
                node.children.Add(new("number", t.value, t));
                i++;
                continue;
            }
            
            if (t.type == "lParen") {
                ParseParens(tokens, node, ref i);
                continue;
            }

            if (t.type == "rParen") {
                err.Raise(Error, "Unexpected right parenthesis", t, false);
                return false;
            }
            
            if (t.type == "operator") {
                if (unaryStart.Contains(t.value) && !unaryEnd.Contains(t.value)) {
                    // Console.WriteLine($"{t.ToString(false)} is start and not end");
                    ParsePreUnaryOps(tokens, node, ref i);
                } else if (!unaryStart.Contains(t.value) && unaryEnd.Contains(t.value)) {
                    // Console.WriteLine($"{t.ToString(false)} is end and not start");
                    ParsePostUnaryOps(tokens, node, ref i);
                } else if (unaryStart.Contains(t.value) && unaryEnd.Contains(t.value)) {
                    // Console.WriteLine($"{t.ToString(false)} is start and end");

                    if (node.children.Count > 0 && valueTypes.Contains(node.children[^1].type)) {
                        ParsePostUnaryOps(tokens, node, ref i);
                    } else {
                        ParsePreUnaryOps(tokens, node, ref i);
                    }
                    /* } else if (preValue && postValue) {
                        // TODO: add a new error type for this
                        err.Raise(Error, "Unary operators cannot be both pre and post. Use parentheses to remove ambiguity.", t, false);
                        i++;
                    } else {
                        // TODO: add a new error type for this
                        err.Raise(Error, "Expected expression before or after unary operator", t, false);
                        i++;
                    } */
                } else {
                    ParseBinaryOps(tokens, node, ref i);
                }
                continue;
            }

            if (t.type == "var") {
                node.children.Add(new("var", t.value, t));
                i++;
                continue;
            }

            if (t.type == "comma") {
                i++;
                continue;
            }

            if (t.type == "dot") {
                i++;
                continue;
            }

            err.Raise(InternalError, $"Unexpected token {t.ToString(false)} at {i}", t, false);
            i++;

            if (lastIs.Count == 3 && lastIs[0] == lastIs[1] && lastIs[1] == lastIs[2] && lastIs[2] == i) {
                throw new Exception("Last 3 i's have been the same");
            } else if (lastIs.Count == 3) {
                lastIs.RemoveAt(0);
                lastIs.Add(i);
            } else {
                lastIs.Add(i);
            }

            return false;
        }

        // Ran out of tokens, endType not found
        return false;
    }
}