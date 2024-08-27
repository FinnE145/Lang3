using Lang3.Utils;

namespace Lang3;

class Parser(Dictionary<string, string> fileCode, Dictionary<string, List<Lexer.Token>> fileTokens) {
    public class Node(string type, string value, Lexer.Token token) {
        public string type = type;
        public string value = value;
        public List<Node> children = [];
        public Lexer.Token token = token;

        private readonly int depthMod = 4;

        private string ChildrenToString() {
            return ChildrenToString(true);
        }

        private string ChildrenToString(bool showToken, int depth = 2) {
            if (children.Count == 0) {
                return "";
            }
            string s = ", [\n";
            foreach (Node child in children) {
                s += $"{child.ToString(showToken, depth + depthMod)}, \n";
            }
            return $"{s[..^3] + '\n'}{new string(' ', depth)}]";
        }

        private string ChildrenToString(bool showLocation, bool showFile, int depth = 2) {
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
            return $"{new string(' ', depth)}Node({type}{(value != "" ? ", " + value : "")}{ChildrenToString(showToken, depth)}{(showToken ? $", token.ToString()" : "")})";
        }

        public string ToString(bool showLocation, bool showFile, int depth = 0) {
            return $"{new string(' ', depth)}Node({type}{(value != "" ? ", " + value : "")}{ChildrenToString(showLocation, showFile, depth)}, {token.ToString(showLocation, showFile)})";
        }
    }

    readonly Errors err = new(fileCode);

    private void ParseParens(List<Lexer.Token> tokens, Node root, ref int i) {
        Node node = new("parens", "", tokens[i]);
        root.children.Add(node);

        i++;
        if (Parse(tokens, node, ref i, "rParen")) {
            i++;
            return;
        } else {
            // TODO: add a new error type for this
            err.Raise(Errors.ErrorNames.Error, "Unmatched parenthesis", tokens[i]);
        }
    }

    public Node Parse(string fp) {
        List<Lexer.Token> tokens = fileTokens[fp];

        Node root = new("root", "", new("", "", 0, 0, 0, fp));
        int i = 0;
        Parse(tokens, root, ref i);
        return root;
    }

    private bool Parse(List<Lexer.Token> tokens, Node node, ref int i, string endType = "<NOT_A_TYPE>") {
        List<int> lastIs = [];

        while (i < tokens.Count) {
            Lexer.Token t = tokens[i];

            if (t.type == endType) {
                return true;
            } else if (t.type == "EOF") {
                return false;
            } else if (t.type == "int" || t.type == "dec") {
                node.children.Add(new("number", t.value, t));
                i++;
            } else if (t.type == "lParen") {
                ParseParens(tokens, node, ref i);
            } else if (t.type == "operator") {
                throw new NotImplementedException();
            } else {
                err.Raise(Errors.ErrorNames.InternalError, "Unexpected token", t, false);
                i++;
            }

            if (lastIs.Count == 3 && lastIs[0] == lastIs[1] && lastIs[1] == lastIs[2] && lastIs[2] == i) {
                throw new Exception("Last 3 i's have been the same");
            } else if (lastIs.Count == 3) {
                lastIs.RemoveAt(0);
                lastIs.Add(i);
            } else {
                lastIs.Add(i);
            }
        }

        // Ran out of tokens, endType not found
        return false;
    }
}