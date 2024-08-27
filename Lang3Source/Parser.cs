using Lang3.Utils;

namespace Lang3;

class Parser(Dictionary<string, string> fileCode, Dictionary<string, List<Lexer.Token>> fileTokens) {
    public class Node(string type, string value, Lexer.Token token) {
        public string type = type;
        public string value = value;
        public List<Node> children = [];
        public Lexer.Token token = token;

        private string ChildrenToString() {
            return ChildrenToString(true);
        }

        private string ChildrenToString(bool showToken) {
            string s = "[\n";
            foreach (Node child in children) {
                s += $"  {child.ToString(showToken)}\n";
            }
            return $"{s}]";
        }

        private string ChildrenToString(bool showLocation, bool showFile) {
            string s = "[\n";
            foreach (Node child in children) {
                s += $"  {child.ToString(showLocation, showFile)}\n";
            }
            return $"{s}]";
        }

        public override string ToString() {
            return $"Node({type}, '{value}', {ChildrenToString()}, {token})";
        }

        public string ToString(bool showToken) {
            return $"Node({type}, '{value}', {ChildrenToString(showToken)}{(showToken ? $", token.ToString()" : "")})";
        }

        public string ToString(bool showLocation, bool showFile) {
            return $"Node({type}, '{value}'{ChildrenToString(showLocation, showFile)}, {token.ToString(showLocation, showFile)})";
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
        while (i < tokens.Count) {
            Lexer.Token t = tokens[i];

            if (t.type == endType) {
                return true;
            } else if (t.type == "EOF") {
                return false;
            } else if (t.type == "int" || t.type == "float" || t.type == "string") {
                node.children.Add(new("value", t.value, t));
                i++;
            } else if (t.type == "lParen") {
                ParseParens(tokens, node, ref i);
            } else if (t.type == "operator") {
                throw new NotImplementedException();
            } else {
                // TODO: add a new error type for this
                err.Raise(Errors.ErrorNames.Error, "Unexpected token", t, false);
                i++;
            }
        }

        // Ran out of tokens, endType not found
        return false;
    }
}